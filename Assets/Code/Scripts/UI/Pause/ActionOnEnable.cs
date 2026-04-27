using UnityEngine;
using UnityEngine.Events;

public class ActionOnEnable : MonoBehaviour
{
    public UnityEvent onEnableActions;
    void OnEnable()
    {
        onEnableActions.Invoke();
    }
}
