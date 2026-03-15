using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionChange : MonoBehaviour
{
    public Vector2Int resolution = new Vector2Int(1280, 720);
    public bool isFullScreen = false;
    
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    void OnEnable()
    {
        FixSettings();
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, isFullScreen);
    }

    public void UpdateResolution()
    {
        int resolutionFromDropdown = resolutionDropdown.value;
        
        switch (resolutionFromDropdown)
        {
            case 0:
                resolution = new Vector2Int(3840, 2160);
                break;
            case 1:
                resolution = new Vector2Int(2560, 1440);
                break;
            case 2:
                resolution = new Vector2Int(1920, 1080);
                break;
            case 3:
                resolution = new Vector2Int(1280, 720);
                break;
            case 4:
                resolution = new Vector2Int(848, 480);
                break;
            case 5:
                resolution = new Vector2Int(432, 240);
                break;
            
            default:
                resolution = new Vector2Int(432, 240);
                break;
        }
        
        SetResolution(resolution.x, resolution.y);
    }

    private int GetResolutionIndex()
    {
        int[] currentRes = {Screen.width, Screen.height};
        switch (currentRes[1])
        {
            case 2160:
                return 0;
                break;
            case 1440:
                return 1;
            case 1080:
                return 2;
            case 720:
                return 3;
            case 480:
                return 4;
            case 240:
                return 5;
        }
        return 5;
    }

    private void FixSettings()
    {
        fullscreenToggle.isOn = Screen.fullScreen;

        resolutionDropdown.value = GetResolutionIndex();
    }

    public void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen;
        
        Screen.fullScreen = isFullScreen;
    }
}
