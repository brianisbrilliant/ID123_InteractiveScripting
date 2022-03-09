using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoving : MonoBehaviour
{
    // Serialized Variables

    [Header("Platform Script With Multiple Waypoints")]
    [Tooltip("Empty Gameobjects work well as waypoints, make sure each item in list has object assigned")]
    [SerializeField]
    List<Transform> wayPointList = new List<Transform>();
    [Tooltip("Sets the platform object to move")]
    [SerializeField]
    GameObject platform;

    [Header("Platform Rotation Settings")]
    [SerializeField]
    bool rotatePlatform = true;
    [SerializeField]
    [Range(2f,15f)]
    float rotateSpeed = 7f;

    [Header("Platform Movement Settings")]
    [Range(1f, 10f)]
    public float platformSpeed = 0.15f;



    [Header("Platform System Settings")]
    //Public Variables
    public bool reverse = false;
    public bool repeat = true;

    //Private Variables
    private bool stopped = false;
    private Transform targetPoint;
    private int nextPointIndex = 1;
    Rigidbody platformRB;
    Transform playerTrans;

    void Start()
    {
        platformRB = gameObject.GetComponent<Rigidbody>();
        platform.transform.position = wayPointList[0].position;
        targetPoint = wayPointList[1];
    }

    void FixedUpdate()
    {
        if (!stopped)
        {
            CheckDistance();
            MovePlatform();
        }
    }

    void CheckDistance()
    {
        if (Vector3.Distance(platform.transform.position, targetPoint.position) < 0.5) SetNextPoint();
    }

    void SetNextPoint()
    {
        NextIndex();
        targetPoint = wayPointList[nextPointIndex];
    }

    void NextIndex()
    {
        if (!reverse)
        {
            if (nextPointIndex < wayPointList.Count - 1)
            {
                nextPointIndex++;
            }
            else
            {
                if (!repeat) stopped = true;
                reverse = true;
                nextPointIndex--;
            }
        }
        else
        {
            if (nextPointIndex > 0)
            {
                nextPointIndex--;
            }
            else
            {
                if (!repeat) stopped = true;
                reverse = false;
                nextPointIndex++;
            }
        }
    }

    void MovePlatform()
    {
        platform.transform.position = Vector3.MoveTowards(platform.transform.position, targetPoint.position, platformSpeed * Time.deltaTime);
        if(rotatePlatform) RotatePlatform();
    }

    void RotatePlatform()
    {
        Vector3 targetDirection = (platform.transform.position - targetPoint.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        platform.transform.rotation = Quaternion.RotateTowards(platform.transform.rotation, targetRotation, rotateSpeed);
    }
}
