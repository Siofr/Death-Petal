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

        LoadBusVolume();

        InnitSetVolumeSlider();

        OnVolumeChanged(_slider.value);
    }

    public void OnVolumeChanged(float sliderValue)
    {
        _bus.setVolume(sliderValue);

        SaveBusVolume();
    }

    public void InnitSetVolumeSlider()
    {
        float currentVol;
        _bus.getVolume(out currentVol);

        _slider.value = currentVol;
    }

    public void SaveBusVolume()
    {
        float currentVol;
        _bus.getVolume(out currentVol);

        PlayerPrefs.SetFloat("bus:/" + busPath, currentVol);
    }
    public void LoadBusVolume()
    {
        _bus.setVolume(PlayerPrefs.GetFloat("bus:/" + busPath, 1.0f));
    }
}