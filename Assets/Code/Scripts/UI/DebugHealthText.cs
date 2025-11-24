using TMPro;
using UnityEngine;

public class DebugHealthText : MonoBehaviour
{
    public GameObject player;
    
    public TMP_Text text;

    private IEntity healthData;
    void Start()
    {
        healthData = player.GetComponent<IEntity>();
    }
    void Update()
    {
        text.text = healthData.Weaknesses.Count.ToString();
    }
}
