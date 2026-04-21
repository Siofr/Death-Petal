using UnityEngine;
using UnityEngine.UI;

public class LoadButton : MonoBehaviour
{
    void Start () {
		Button btn = GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick(){
		Debug.Log ("[PAUSE] Load Button Clicked");

        SaveSystem.LoadGameData();
	}
}
