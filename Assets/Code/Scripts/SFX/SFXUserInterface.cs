using UnityEngine;
using FMOD;
using FMODUnity;
using UnityEngine.EventSystems;

public class SFXUserInterface : MonoBehaviour
{
    public EventReference enterSFX;
    public EventReference selectSFX;

    public void OnOptionEnter()
    {
        RuntimeManager.PlayOneShot(enterSFX);
    }

    public void OnOptionExit()
    {

    }

    public void OnOptionSelect()
    {
        RuntimeManager.PlayOneShot(selectSFX);
    }
}
