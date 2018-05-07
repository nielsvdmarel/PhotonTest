using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public float CameraMoveSpeed = 120.0f;
    public GameObject CameraFollowObject;
    private Vector3 FollowPOS;
    public float clampAngle = 80.0f;
    public float inputSensivity = 150.0f;
    public GameObject CameraObj;
    public GameObject EnemyTargetLockOBJ;
    public float camDistanceXToPlayer;
    public float camDistanceYToPlayer;
    public float camDistancezToPlayer;
    public float mouseX;
    public float mouseY;
    public float finalInputX;
    public float finalInputZ;
    public float smoothX;
    public float smoothY;
    private float rotY = 0.0f;
    private float rotX = 0.0f;
    [SerializeField]
    private bool lookAtTarget = true    ;

	void Start ()
    {
        lookAtTarget = true;
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	void Update ()
    {
        float inputX = Input.GetAxis("RightStickHorizontal");
        float inputZ = Input.GetAxis("RightStickVertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        finalInputX = inputX + mouseX;
        finalInputZ = inputZ + mouseY;

        rotY += finalInputX * inputSensivity * Time.deltaTime;
        rotX += finalInputZ * inputSensivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
        if (Input.GetKey(KeyCode.E))
        {
            LookAtTargetFunc();
        }

	}

    private void LateUpdate()
    {
        cameraUpdater();
    }

    private void cameraUpdater()
    {
        //set the target object
        Transform target = CameraFollowObject.transform;

        //move towards game object that is the target
        float step = CameraMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }

    private void LookAtTargetFunc()
    {
        if (lookAtTarget)
        {
            transform.LookAt(EnemyTargetLockOBJ.transform);
        }
    }
}
