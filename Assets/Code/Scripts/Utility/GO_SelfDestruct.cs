using System.Collections;
using UnityEngine;

public class GO_SelfDestruct : MonoBehaviour
{
    [SerializeField] private float aliveTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyAfterSeconds(aliveTime));
    }

    IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
