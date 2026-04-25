using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using SFXUtil;

public class MusicManager : MonoBehaviour
{
    public EventReference musicEventPath;
    private EventInstance _musicEventInstance;

    private PARAMETER_ID _gameStateParam;
    private PARAMETER_ID _gameIntensityParam;
    private PARAMETER_ID _playerStateParam;

    private int _playerThreat;
    private bool _isPlaying;

    private EventBindings<PlayerTargetedEvent> _onPlayerTargetedEventListener;
    private EventBindings<PlayerLostTargetEvent> _onPlayerTargetLostEventListener;

    void Start()
    {
        _gameStateParam = SFXUtilities.AssignParamID("GameState", musicEventPath);
        _gameIntensityParam = SFXUtilities.AssignParamID("Threat", musicEventPath);
        _playerStateParam = SFXUtilities.AssignParamID("PlayerState", musicEventPath);

        _musicEventInstance = SFXUtilities.CreateEventInstance(musicEventPath, this.gameObject);

        PlayMusic();
    }

    void Awake()
    {
        _onPlayerTargetedEventListener = new EventBindings<PlayerTargetedEvent>(OnPlayerTargeted);
        _onPlayerTargetLostEventListener = new EventBindings<PlayerLostTargetEvent>(OnPlayerUntargeted);
    }

    private void OnEnable()
    {
        EventBus<PlayerTargetedEvent>.Register(_onPlayerTargetedEventListener);
        EventBus<PlayerLostTargetEvent>.Register(_onPlayerTargetLostEventListener);
    }

    private void OnDisable()
    {
        EventBus<PlayerTargetedEvent>.Unregister(_onPlayerTargetedEventListener);
        EventBus<PlayerLostTargetEvent>.Unregister(_onPlayerTargetLostEventListener);
    }

    void ResetParameters()
    {

    }

    void PlayMusic()
    {
        if (_isPlaying) return;

        _musicEventInstance.start();
        UpdateGameState(1);
        _isPlaying = true;
    }

    void StopMusic()
    {
        if (!_isPlaying) return;

        _musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _isPlaying = false;
    }

    void UpdatePlayerState()
    {

    }

    void OnThreatLevelUpdate(int newThreatLevel)
    {
        _musicEventInstance.setParameterByID(_gameIntensityParam, newThreatLevel);

        if (newThreatLevel <= 0)
        {
            UpdateGameState(1);
            return;
        }

        UpdateGameState(2);
    }

    void UpdateGameState(int gameState)
    {
        _musicEventInstance.setParameterByID(_gameStateParam, gameState);
    }

    void OnPlayerTargeted(PlayerTargetedEvent ctx)
    {
        _playerThreat += ctx.threatLevel;

        OnThreatLevelUpdate(_playerThreat);
    }

    void OnPlayerUntargeted(PlayerLostTargetEvent ctx)
    {
        _playerThreat -= ctx.threatLevel;

        OnThreatLevelUpdate(_playerThreat);
    }

    void OnPlayerCriticalHealth()
    {

    }
}
