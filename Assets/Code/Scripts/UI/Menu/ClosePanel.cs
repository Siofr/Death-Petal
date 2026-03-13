using UnityEngine;

public class ClosePanel : MonoBehaviour
{
    public void CloseThisPanel()
    {
        //expand this by adding animation triggers and etc.
        gameObject.SetActive(false);
    }
}
