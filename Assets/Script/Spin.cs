using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float xSpeed;
    public float ySpeed;
    public float zSpeed;

    Vector3 rotationAngle = new Vector3(0,0,0);
    void Update()
    {
        rotationAngle.x = xSpeed * Time.deltaTime;
        rotationAngle.y = ySpeed * Time.deltaTime;
        rotationAngle.z = zSpeed * Time.deltaTime;

        gameObject.transform.localEulerAngles = gameObject.transform.localEulerAngles + rotationAngle;
    }
}
