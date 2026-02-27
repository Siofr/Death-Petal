using UnityEngine;

public class Bishop_EyeLookAt : MonoBehaviour
{

    [SerializeField] public Transform lookAtTarget;
    [SerializeField] private int adjustment;

    // Update is called once per frame
    void Update()
    {
        if ((Time.time + transform.position.y) % .5 <= 0.1)
        {
            transform.LookAt(lookAtTarget);
            transform.localEulerAngles =
                new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + adjustment, transform.localEulerAngles.z);
        }

    }
}
