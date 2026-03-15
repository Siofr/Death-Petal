using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    public float closeTime = 0.1f;
    public void CloseThisPanel()
    {
        //expand this by adding animation triggers and etc.
        gameObject.LeanScale(Vector3.zero , closeTime).setOnComplete(() => gameObject.SetActive(false)).setEaseInOutSine();
        
        ;
    }
    private void Awake()
    {
        gameObject.transform.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        gameObject.LeanScale(Vector3.one , closeTime);
    }
}
