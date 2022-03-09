using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public Transform platformSafePosition;

    public float clockwiseMaxAngle = 90;
    public float counterClockwiseMinAngle = 0;

    bool trigger = false;
    bool isMoving = false;
    bool clockwise = false;

    PlayerController playerController;
    Transform playerTrans;

    float degrees = 1f;

    private void FixedUpdate()
    {
        if (trigger)
        {
            isMoving = true;
            if (clockwise)
            {
                StartCoroutine(rotatePlatformClockWise());
            }
            else
            {
                StartCoroutine(rotatePlatformCounterClockWise());
            }
        }
    }

    IEnumerator rotatePlatformClockWise()
    {
        while (transform.localEulerAngles.y < (clockwiseMaxAngle - 1))
        {
            if (playerController != null) playerController.isMoveable = false;
            if (playerTrans != null) playerTrans.position = Vector3.Lerp(playerTrans.position, platformSafePosition.position, 0.5f * Time.deltaTime);
            transform.Rotate(new Vector3(0, degrees * Time.deltaTime, 0));
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0, clockwiseMaxAngle, 0);
        isMoving = false;
        if(playerController != null) playerController.isMoveable = true;
        yield break;
    }

    IEnumerator rotatePlatformCounterClockWise()
    {
        while (transform.localEulerAngles.y > (counterClockwiseMinAngle + 1))
        {
            if (playerController != null) playerController.isMoveable = false;
            if (playerTrans != null) playerTrans.position = Vector3.Lerp(playerTrans.position, platformSafePosition.position, 0.5f * Time.deltaTime);
            transform.Rotate(new Vector3(0, -(degrees * Time.deltaTime), 0));
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0, counterClockwiseMinAngle, 0);
        isMoving = false;
        if (playerController != null) playerController.isMoveable = true;
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerController = other.GetComponent<PlayerController>();
            playerTrans = other.transform;
            if (!isMoving)
            {
                trigger = !trigger;
                clockwise = !clockwise;
            }
            else
            {

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        StartCoroutine(DetachPlayer());
    }



    IEnumerator DetachPlayer()
    {
        while(isMoving)
        {
            yield return null;
        }
        playerController = null;
        playerTrans = null;
        trigger = !trigger;
        yield break;
    }
}
