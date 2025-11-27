using UnityEngine;

public class EnemySFX : MonoBehaviour
{
    private EventBindings<WrongShotEvent> _wrongShotEventListener;

    private void OnEnable()
    {
        _wrongShotEventListener = new EventBindings<WrongShotEvent>(OnWrongShot);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnWrongShot(WrongShotEvent ctx)
    {

    }
}
