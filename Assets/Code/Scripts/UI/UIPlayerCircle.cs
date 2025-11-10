using State_Machine;
using UnityEngine;

public class UIPlayerCircle : MonoBehaviour
{
    private Transform UICircle;
    private Transform playerTransform;

    public EventBindings<AimEvent> activateCircleEventListener;

    private void Awake()
    {
        activateCircleEventListener = new EventBindings<AimEvent>(OnPlayerAim);
    }

    private void OnEnable()
    {
        EventBus<AimEvent>.Register(activateCircleEventListener);
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
