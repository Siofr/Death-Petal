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
    
    //Serialized Fields
    private List<WallPiece> _wallPieces = new List<WallPiece>();
    
    //Events
    private EventBindings<ActiveTargetEvent> _onTargetListener;

    protected override void Awake()
    {
        base.Awake();

        foreach (var wall in _wallPiecesObjects)
        {
            _wallPieces.Add(new WallPiece(wall.GetComponentInChildren<Weakness>(), wall));
        }
        
        if (_isHidden)
        {
            _onTargetListener = new EventBindings<ActiveTargetEvent>(OnTargeted);
        }
    }

    private void Start()
    {
        CheckWallPieces();
    }

    private void CheckWallPieces()
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
    
    private void OnEnable()
    {
        if(_isHidden) EventBus<ActiveTargetEvent>.Register(_onTargetListener);
    }

    private void OnDisable()
    {
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