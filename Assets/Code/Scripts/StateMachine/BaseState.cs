using UnityEngine;

public abstract class BaseState : MonoBehaviour
{
    public abstract void FrameUpdate();

    public abstract void LateFrameUpdate();

    public abstract void EnterState();

    public abstract void ExitState();
}
