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
        if(isPaused)
        {
            PauseUpdate();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    void PauseUpdate()
    {
        Time.timeScale = 0f;
    }
}
