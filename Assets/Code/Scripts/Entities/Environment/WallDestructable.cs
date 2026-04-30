using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WallPiece
{
    public Weakness weakness;
    public GameObject wall;

    public WallPiece(Weakness weakness, GameObject wall)
    {
        this.weakness = weakness;
        this.wall = wall;
    }
}

public class WallDestructable: EntityBase
{
    [Header("Wall Fields")]
    [SerializeField] private bool _isHidden;

    [SerializeField] private GameObject[] _wallPiecesObjects;
    [SerializeField] private GameObject _particleContainer;
    
    //Serialized Fields
    private List<WallPiece> _wallPieces = new List<WallPiece>();
    
    //Events
    private EventBindings<ActiveTargetEvent> _onTargetListener;
    private EventBindings<LevelLoadedEvent> _levelLoadedListener;

    public EventReference panelDestroyedSFX;
    
    protected override void Awake()
    {
        base.Awake();
        
        _levelLoadedListener = new EventBindings<LevelLoadedEvent>(CheckWallPieces);
        
        foreach (var wall in _wallPiecesObjects)
        {
            _wallPieces.Add(new WallPiece(wall.GetComponentInChildren<Weakness>(), wall));
        }
        
        if (_isHidden)
        {
            _onTargetListener = new EventBindings<ActiveTargetEvent>(OnTargeted);
        }
    }

    protected virtual void Start()
    {
        base.Start();
        
        InitialiseWallPieces();
    }

    private void InitialiseWallPieces()
    {
        if (Weaknesses.Count == _wallPieces.Count) return;

        var temp = new List<Weakness>();
        
        foreach (var weakness in Weaknesses)
        {
            foreach (var associatedWeakness in _wallPieces)
            {
                if (associatedWeakness.weakness == weakness && !temp.Contains(weakness))
                {
                    continue;
                }
                
                temp.Add(weakness);
            }
        }

        for (int i = temp.Count - 1; i >= 0; i--)
        {
            Destroy(temp[i].transform.parent.gameObject);
        }
    }

    private void CheckWallPieces()
    {
        for (int i = _wallPieces.Count - 1; i >= 0; i--)
        {
            if (Weaknesses.Contains(_wallPieces[i].weakness)) continue;
            
            Destroy(_wallPieces[i].wall);
        }
    }
    
    protected virtual void OnEnable()
    {
        base.OnEnable();
        
        EventBus<LevelLoadedEvent>.Register(_levelLoadedListener);
        
        if (_isHidden)
        {
            EventBus<ActiveTargetEvent>.Register(_onTargetListener);
        }
    }

    protected virtual void OnDisable()
    {
        base.OnDisable();
        
        EventBus<LevelLoadedEvent>.Unregister(_levelLoadedListener);
        
        if(_isHidden) EventBus<ActiveTargetEvent>.Unregister(_onTargetListener);
    }
    
    private void OnTargeted(ActiveTargetEvent context)
    {
        if (!_isHidden) return;

        if (context.activeTarget == null)
        {
            ToggleAllWeaknessIcons(false);
            return;
        }
        
        foreach (var weakness in Weaknesses)
        {
            if (weakness.transform == context.activeTarget)
            {
                weakness.ToggleIcon(true);
                continue;
            }
            
            weakness.ToggleIcon(false);
        }
    }
    
    public override void OnShot(Weakness weakness, WeakTypes damageType)
    {
        int weaknessCount = Weaknesses.Count;
        
        if (!Weaknesses.Contains(weakness)) return;

        Debug.Log("Panel Destruction");
        RuntimeManager.PlayOneShot(panelDestroyedSFX);

        if (weakness.WeakType != damageType)
        {
            EventBus<WrongShotEvent>.Raise(new WrongShotEvent());
            return;
        }

        for (int i = _wallPieces.Count - 1; i >= 0; i--)
        {
            if (_wallPieces[i].weakness != weakness) continue; 
            
            Destroy(_wallPieces[i].wall);
            Weaknesses.Remove(weakness);
            
            // make new container so it doesn't die when the game object is destroyed
            var newParticleContainer = Instantiate(_particleContainer);
            newParticleContainer.transform.position = _wallPieces[i].weakness.transform.position;
            foreach (ParticleSystem componentsInChild in newParticleContainer.GetComponentsInChildren<ParticleSystem>())
            {
                componentsInChild.Play();
            }
            
            
            Destroy(_wallPieces[i].weakness.transform.parent.gameObject);
            _wallPieces.RemoveAt(i);
            break;
        }
        
        if (Weaknesses.Count < 1)
        {
            Destroy(gameObject);
        }
        
        if (!__sequentialWeaknesses) return;
        
        if (Weaknesses.Count < weaknessCount && Weaknesses.Count > 0)
        {
            defaultWeaknessTypes.RemoveAt(0);
            Weaknesses[0].ToggleHitbox(true);
            Weaknesses[0].SetWeakType(defaultWeaknessTypes[0]);
        }
    }
}