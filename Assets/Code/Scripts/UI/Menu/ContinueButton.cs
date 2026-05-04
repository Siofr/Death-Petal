using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    private Button _button;
    
    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.interactable = SaveSystem.CheckData();
    }
}
