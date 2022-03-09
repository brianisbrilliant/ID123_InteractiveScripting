using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkToDestroy : MonoBehaviour
{
    public float speed = 0.8f;
    void Start()
    {
        StartCoroutine(ShrinkObject());
    }

    IEnumerator ShrinkObject()
    {
        while(transform.localScale.x > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, speed * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }
}
