using UnityEngine;
using UnityEngine.Events;

public class UnityEventInvoker : MonoBehaviour
{

    public UnityEvent[] eventsToActivate;

    public void InvokePublicEvent(int index)
    {
        eventsToActivate[index].Invoke();
    }

}
