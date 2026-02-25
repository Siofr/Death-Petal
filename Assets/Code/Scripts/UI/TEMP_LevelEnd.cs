using UnityEngine;

public class TEMP_LevelEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "Player")
        {
            EventBus<OnLevelEndEvent>.Raise(new OnLevelEndEvent());
        }
    }
}
