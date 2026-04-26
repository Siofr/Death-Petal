using UnityEngine;
using FMODUnity;
using FMOD;
using FMOD.Studio;
using UnityEngine.UI;

public class SFXVolumeSlider : MonoBehaviour
{
    public string busPath;
    private Bus _bus;
    private Slider _slider;

    public void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
        _bus = RuntimeManager.GetBus("bus:/" + busPath);
        OnVolumeChanged(_slider.value);
    }

    public void OnVolumeChanged(float sliderValue)
    {
        _bus.setVolume(sliderValue);
    }
}