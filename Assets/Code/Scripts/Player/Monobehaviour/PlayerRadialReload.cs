using UnityEngine;
using FMOD.Studio;
using SFXUtil;
using FMODUnity;

public class PlayerRadialReload : MonoBehaviour
{
    public EventReference shootSfxEventPath;
    private EventInstance _shootEvent;
    private PARAMETER_ID _bulletsLeft;

    public EventReference AddRemoveSFXEvent;
    private EventInstance _addRemoveEvent;
    private PARAMETER_ID _addRemove;

    private BulletSO[] bulletArray = new BulletSO[6];
    private BulletSO[] lastBulletArray = new BulletSO[6];
    private int bulletIndex = 0;
    private int lastBulletIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
