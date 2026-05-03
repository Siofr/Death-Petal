using System.Collections.Generic;
using UnityEngine;

public class AutoCloseUi : MonoBehaviour
{
    public List<GameObject> popUpsToClose;

    void ClosePopUps()
    {
        foreach (var popup in popUpsToClose)
        {
            popup.SetActive(false);
        }
    }

    void OnDisable()
    {
        ClosePopUps();
    }
}
