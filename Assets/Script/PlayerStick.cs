using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStick : MonoBehaviour
{
    Vector3 PlayerPosition;
    Quaternion PlayerEulerRotation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerPosition = other.transform.position;
            PlayerEulerRotation = other.transform.rotation;
            Debug.Log(PlayerEulerRotation.eulerAngles);
            other.transform.SetParent(null);
            other.transform.rotation = PlayerEulerRotation;
            other.transform.position = PlayerPosition;
        }
    }
}
