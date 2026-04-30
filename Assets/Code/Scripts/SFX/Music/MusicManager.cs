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
    private float _previousGameState;

    private EventBindings<PlayerTargetedEvent> _onPlayerTargetedEventListener;
    private EventBindings<PlayerLostTargetEvent> _onPlayerTargetLostEventListener;

    private EventBindings<PlayerLowHealthEvent> _lowHealthListener;
    private EventBindings<PlayerHealedEvent> _recoveredListener;

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

        _lowHealthListener = new EventBindings<PlayerLowHealthEvent>(OnPlayerCriticalHealth);
        _recoveredListener = new EventBindings<PlayerHealedEvent>(OnPlayerRecovered);
    }

    private void OnEnable()
    {
        EventBus<PlayerTargetedEvent>.Register(_onPlayerTargetedEventListener);
        EventBus<PlayerLostTargetEvent>.Register(_onPlayerTargetLostEventListener);

        EventBus<PlayerLowHealthEvent>.Register(_lowHealthListener);
        EventBus<PlayerHealedEvent>.Register(_recoveredListener);
    }

    private void OnDisable()
    {
        EventBus<PlayerTargetedEvent>.Unregister(_onPlayerTargetedEventListener);
        EventBus<PlayerLostTargetEvent>.Unregister(_onPlayerTargetLostEventListener);

        EventBus<PlayerLowHealthEvent>.Unregister(_lowHealthListener);
        EventBus<PlayerHealedEvent>.Unregister(_recoveredListener);
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

    void OnPlayerCriticalHealth(PlayerLowHealthEvent ctx)
    {
        _musicEventInstance.setParameterByID(_playerStateParam, 0.00f);
    }

    void OnPlayerRecovered(PlayerHealedEvent ctx)
    {
        _musicEventInstance.setParameterByID(_playerStateParam, 1.0f);
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
}
