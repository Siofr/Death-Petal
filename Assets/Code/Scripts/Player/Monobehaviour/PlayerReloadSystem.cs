using System.Collections;
using UnityEngine;

public enum ReloadState
{
    IDLE,
    RELOADING,
}

public class PlayerReloadSystem : MonoBehaviour
{
    public ReloadState currentState = ReloadState.IDLE;

    public void HandleReload()
    {
        currentState = ReloadState.RELOADING;
    }

    public void HandleReloadCancel()
    {
        currentState = ReloadState.IDLE;
    }

    public void HandleHotkey()
    {
        switch (currentState)
        {
            case ReloadState.IDLE:
                break;
            case ReloadState.RELOADING:
                AddBullet();
                break;
        }
    }

    private void AddBullet()
    {

    }

    private void ResetBullets()
    {

    }

    private void RemoveBullet()
    {

    }
}
