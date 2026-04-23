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
        _gameIntensityParam = SFXUtilities.AssignParamID("Intensity", musicEventPath);
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
        if (newThreatLevel <= 0)
        {
            UpdateGameState(1);
            return;
        }

        UpdateGameIntensity(newThreatLevel);
        UpdateGameState(2);
    }

    void UpdateGameState(int gameState)
    {
        Debug.Log("Update Game State");
        float value;

        _musicEventInstance.setParameterByID(_gameStateParam, gameState);
        Debug.Log(_musicEventInstance.getParameterByID(_gameStateParam, out value));
        Debug.Log("State " + value);
    }

    void UpdateGameIntensity(int threatLevel)
    {

    }

    void OnPlayerTargeted(PlayerTargetedEvent ctx)
    {
        Debug.Log("Update threat");

        _playerThreat += 1;

        OnThreatLevelUpdate(_playerThreat);
    }

    void OnPlayerUntargeted(PlayerLostTargetEvent ctx)
    {
        _playerThreat -= 1;

        OnThreatLevelUpdate(_playerThreat);
    }
}
