using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;               
    [SerializeField] private float smoothSpeed = 5f;         
    [SerializeField] private Vector3 offset;                 
    [SerializeField] private bool followEnabled = true;      

    [Header("Follow Buffer")]
    public float deadZoneX = 1.5f; 
    public float deadZoneY = 1f;   

    void LateUpdate()
    {
        if (!followEnabled || target == null) return;

        Vector3 cameraPos = transform.position;
        Vector3 targetPos = target.position + offset;


        Vector2 delta = targetPos - cameraPos;
        float moveX = Mathf.Abs(delta.x) > deadZoneX ? targetPos.x : cameraPos.x;
        float moveY = Mathf.Abs(delta.y) > deadZoneY ? targetPos.y : cameraPos.y;



        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }

    public void EnableFollow(bool state)
    {
        followEnabled = state;
    }
}

