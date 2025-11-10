using State_Machine;
using UnityEngine;

public struct ActivateCircleEvent : IEvent { }

public class UIPlayerCircle : MonoBehaviour
{
    private Transform UICircle;
    private Transform playerTransform;

    public EventBindings<ActivateCircleEvent> activateCircleEventListener;

    private void Awake()
    {
        activateCircleEventListener = new EventBindings<ActivateCircleEvent>(OnPlayerAim);
    }

    private void OnEnable()
    {
        EventBus<ActivateCircleEvent>.Register(activateCircleEventListener);
    }

    private void Start()
    {
        UICircle = transform.GetChild(0);
        playerTransform = PlayerManager.Instance.transform;
    }
    void Update()
    {
        transform.position = playerTransform.position;
        transform.rotation = playerTransform.rotation;
    }

    private void OnPlayerAim()
    {
        UICircle.gameObject.SetActive(!UICircle.gameObject.activeInHierarchy);
    }
}
