using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
	void Start () {
		Button btn = GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		Debug.Log ("[PAUSE] Save Button Clicked");

        SaveSystem.SaveGameData();
	}
}
