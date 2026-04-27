using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PausingFunc : MonoBehaviour
{
    public bool isPaused;

    public void TogglePause()
    {
        isPaused = !isPaused;
    }
    void Update()
    {
        
    }
}
