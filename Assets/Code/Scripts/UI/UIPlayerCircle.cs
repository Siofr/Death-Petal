using State_Machine;
using UnityEngine;

public class UIPlayerCircle : MonoBehaviour
{
    private void Awake()
    {
        PlayerManager.Instance.playerAimEventListener = new EventBindings<PlayerAimEvent>(OnPlayerAim);
        PlayerManager.Instance.playerAimCancelEventListener = new EventBindings<PlayerAimCancelEvent>(OnPlayerAimExit);
    }
    void Update()
    {
        transform.position = PlayerManager.Instance.transform.position;
    }

    void OnPlayerAim()
    {
        transform.gameObject.SetActive(true);
    }

    void OnPlayerAimExit()
    {
        transform.gameObject.SetActive(false);
    }
}
