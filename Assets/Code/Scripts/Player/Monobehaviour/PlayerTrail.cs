using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    public float lifetime;
    public float speed;
    private float endTime;
    public Transform target;

    void OnEnable()
    {
        endTime = Time.time + lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null && Vector3.Distance(target.transform.position, transform.position) <= 0.5f)
        {
            Destroy(gameObject);
        }

        transform.position += transform.forward * speed * Time.deltaTime;

        if (Time.time >= endTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
