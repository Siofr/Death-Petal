using FMODUnity;
using UnityEngine.UI;
using UnityEngine;

public class SFXSliderInterface : SFXUserInterface
{
    public EventReference sliderSFX;

    public void OnSliderChanged()
    {
        RuntimeManager.PlayOneShot(sliderSFX);
    }
}
