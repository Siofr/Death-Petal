using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionChange : MonoBehaviour
{
    public Vector2Int resolution = new Vector2Int(1280, 720);
    public bool isFullScreen = false;
    
    public TMP_Dropdown resolutionDropdown;

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

    public void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen;
        
        Screen.fullScreen = isFullScreen;
    }
}
