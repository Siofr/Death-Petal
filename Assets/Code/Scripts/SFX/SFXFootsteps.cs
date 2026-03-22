using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using SFXUtil;
using System.Collections.Generic;

[System.Serializable]
public class TerrainParam
{
    public PhysicsMaterial physicsMaterial;
    public int sfxParam;
}

public class SFXFootsteps : MonoBehaviour
{
    public List<TerrainParam> terrainParams;
    private Dictionary<PhysicsMaterial, int> terrainDict = new Dictionary<PhysicsMaterial, int>();

    public EventReference eventPath;
    private EventInstance eventInstance;

    private PARAMETER_ID _terrainParam;
    private PARAMETER_ID _walkParam;

    private LayerMask ground;

    private Animator anim;
    private float _lastFootstep;

    private void Start()
    {
        foreach (var item in terrainParams)
        {
            terrainDict.Add(item.physicsMaterial, item.sfxParam);
        }

        _walkParam = SFXUtilities.AssignParamID("WalkRun", eventPath);
        _terrainParam = SFXUtilities.AssignParamID("Terrain", eventPath);

        eventInstance = SFXUtilities.CreateEventInstance(eventPath, this.gameObject);

        ground = LayerMask.GetMask("Ground");
    }

    private void OnValidate()
    {
        if (!anim) anim = GetComponent<Animator>();
    }

    private void Update()
    {
        var footstep = anim.GetFloat("Footstep");
        if (Mathf.Abs(footstep) < 0.0001f) footstep = 0f;

        if (_lastFootstep > 0 && footstep < 0 || _lastFootstep < 0 && footstep > 0) PlayWalkSFX();

        _lastFootstep = footstep;
    }

    public void PlayWalkSFX()
    {
        Debug.Log("Play Footstep");

        eventInstance.setParameterByID(_terrainParam, GetFloorID());

        // EventBus<SFXEventTrigger>.Raise(new SFXEventTrigger(eventInstance, this.gameObject));
    }

    private int GetFloorID()
    {
        if (terrainParams.Count <= 1) return 0;

        RaycastHit hit;
        PhysicsMaterial mat = null;

        if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.5f, ground))
        {
            mat = hit.collider.sharedMaterial;
        }

        int output = 0;

        if (!mat) return output;

        if (terrainDict.ContainsKey(mat))
        {
            Debug.Log("Success");
            output = terrainDict[mat];
        }

        return output;
    }
}
