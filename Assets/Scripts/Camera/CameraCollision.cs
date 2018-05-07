using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour {
    public float minDistance = 1.0f;
    public float maxDistance = 4.0f;
    public float smooth = 10.0f;
    private Vector3 dollydir;
    public Vector3 dollyDirAdjusted;
    public float distance;

	void Awake()
    {
        dollydir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
	}
	
	void Update()
    {
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollydir * maxDistance);
        RaycastHit hit;
        if(Physics.Linecast(transform.parent.position, desiredCameraPos, out hit))
        {
            if(hit.transform.tag != "Player")
            {
                distance = Mathf.Clamp((hit.distance * 0.87f), minDistance, maxDistance);
            }
        }

        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollydir * distance, Time.deltaTime * smooth);
	}
}
