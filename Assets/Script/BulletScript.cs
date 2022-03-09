using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float damage = 2;
    public Transform shotBy;

    private void Awake()
    {
        damage = Random.Range(damage * 0.8f, damage * 1.2f);
        transform.localScale = Vector3.one * Random.Range(0.2f,1f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.Equals(shotBy))
        {
            Destroy(gameObject);
        }
    }
}
