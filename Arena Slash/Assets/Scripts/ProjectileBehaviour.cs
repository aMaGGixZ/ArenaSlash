using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileBehaviour : MonoBehaviour {

    public float speed = 2f;
    public float lifeTime = 4f;

    Rigidbody rb;

    void Start () {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);

        Invoke("DestroySelf", lifeTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        DestroySelf();
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
