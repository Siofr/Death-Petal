using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    public float lifetime;
    public float speed;
    private float endTime;

    void OnEnable()
    {
        Debug.Log("Spawn trail");
        endTime = Time.time + lifetime;
    }

    // Update is called once per frame
    void Update()
    {
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
