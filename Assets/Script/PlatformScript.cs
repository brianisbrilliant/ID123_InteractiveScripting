using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    Rigidbody rigidBody;
    public Color touchColor;

    public float waitToFallTime;
    public float waitToResetTime;
    Vector3 resetPoint;
    Quaternion resetRotation;
    Material thisMat;

    bool isReseting = false;
    bool isTriggered = false;
    float timer = 5        ;
    Color startColor;

    private void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
        thisMat = gameObject.GetComponentInChildren<Renderer>().material;
        thisMat.EnableKeyword(("_EMISSION"));
        startColor = thisMat.GetColor("_EmissionColor");
    }

    public AnimationCurve curve;
    private void Awake()
    {
        resetPoint = transform.position;
        resetRotation = transform.rotation;
    }

    private void Update()
    {
        if (isReseting && isTriggered == false)
        {
            transform.position = Vector3.Lerp(transform.position, resetPoint, timer * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, resetRotation, timer * Time.deltaTime);
        }
        lerpColor();
    }

    void lerpColor()
    {
        if (!isReseting)
        {
            thisMat.SetColor("_EmissionColor", Color.Lerp(thisMat.GetColor("_EmissionColor"), startColor, timer * Time.deltaTime));
        }
        if (isTriggered)
        {
            thisMat.SetColor("_EmissionColor", Color.Lerp(thisMat.GetColor("_EmissionColor"), touchColor, timer * Time.deltaTime));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isTriggered = true;
            StartCoroutine(WaitFall());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isTriggered = false;
        }
    }

    IEnumerator WaitFall()
    {
        yield return new WaitForSeconds(waitToFallTime);
        rigidBody.isKinematic = false;
        isReseting = false;
        StartCoroutine(WaitReset());
    }

    IEnumerator WaitReset()
    {
        yield return new WaitForSeconds(waitToResetTime);
        rigidBody.isKinematic = true;
        isReseting = true;
    }
}
