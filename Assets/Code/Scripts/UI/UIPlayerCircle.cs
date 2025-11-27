using UnityEngine;

public class UIPlayerCircle : MonoBehaviour
{
    private Transform UICircle;
    private Transform playerTransform;

    public EventBindings<AimEvent> activateCircleEventListener;
    private EventBindings<TransmitPlayerInfo> transmitPlayerInfoListener;

    private void Awake()
    {
        activateCircleEventListener = new EventBindings<AimEvent>(OnPlayerAim);
        transmitPlayerInfoListener = new EventBindings<TransmitPlayerInfo>(AssignPlayerInfo);
    }

    private void OnEnable()
    {
        EventBus<AimEvent>.Register(activateCircleEventListener);
        EventBus<TransmitPlayerInfo>.Register(transmitPlayerInfoListener);
    }

    private void OnDisable()
    {
        EventBus<AimEvent>.Unregister(activateCircleEventListener);
        EventBus<TransmitPlayerInfo>.Unregister(transmitPlayerInfoListener);
    }

    private void Start()
    {
        UICircle = transform.GetChild(0);
        // playerTransform = PlayerManager.Instance.transform;
    }
    void Update()
    {
        if(UICircle.gameObject.activeSelf)
        {
            transform.position = playerTransform.position;
            transform.rotation = playerTransform.rotation;
        }
    }

    private void OnPlayerAim()
    {
        UICircle?.gameObject.SetActive(!UICircle.gameObject.activeSelf);
    }

    private void AssignPlayerInfo(TransmitPlayerInfo ctx)
    {
        playerTransform = ctx.player;
    }
}
