using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustableObject : MonoBehaviour {


    public Vector3 AdjustAxis;
    [Header("put the minimum and maximum distances here, based on value")]
    public Vector3 minimumValue;
    public Vector3 maximumValue;
    public bool stayInBounds;

    private bool coroutineRunning = false;
    private bool controllerMoved = false;

    private Vector3 originPosition;
    private Quaternion originRotation;

    private Transform control;

    private Transform originParent;

    void Start()
    {
        originParent = transform.parent;
    }

    public void MovingObject(Transform controller)
    {
        originPosition = transform.position;
        originRotation = transform.rotation;
        transform.parent = controller;
        control = controller;
    }

    public void ReleaseObject()
    {
        transform.parent = originParent;
        controllerMoved = true;
    }

    IEnumerator LerpPosition(Vector3 current, Vector3 goal)
    {
        coroutineRunning = true;
        float step = 0.0f;
        while (step <= 1.0f)
        {
            transform.position = Vector3.Lerp(current, goal, step);
            step += Time.deltaTime;
            yield return null;
        }
        coroutineRunning = false;
        controllerMoved = false;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (stayInBounds)
        {
            if (other.tag == "PointClouds")
            {
                if (transform.position.x > originPosition.x)
                {
                    StartCoroutine(LerpPosition(transform.position, maximumValue));
                }
                else
                {
                    StartCoroutine(LerpPosition(transform.position, originPosition));
                }
            }
        } 
    }*/

    void Update()
    {
        if(control != null)
        {
            if (coroutineRunning)
            {
                StopAllCoroutines();
                coroutineRunning = false;
            }
            if (AdjustAxis.x > 0.0f)
            {
                transform.position = new Vector3(transform.position.x, originPosition.y, originPosition.z);
            }
            if(AdjustAxis.y > 0.0f)
            {
                transform.position = new Vector3(originPosition.x, transform.position.y, originPosition.z);
            }
            if(AdjustAxis.z > 0.0f)
            {
                transform.position = new Vector3(originPosition.x, originPosition.y, transform.position.z);
            }
            transform.rotation = originRotation;
        }
        else //check if in bounds, if not lerp
        {
            /*if (stayInBounds && controllerMoved)
            {
                Vector3 goalPosition = transform.position;
                Vector3 position = transform.position;
                if (AdjustAxis.x > 0.0f)
                {
                    if (transform.position.x > maximumValue.x) { goalPosition = maximumValue; }
                    if (transform.position.x < minimumValue.x) { goalPosition = minimumValue; }
                }
                if (AdjustAxis.y > 0.0f)
                {
                    if (transform.position.y > maximumValue.y) { goalPosition = maximumValue; }
                    if (transform.position.y < minimumValue.y) { goalPosition = minimumValue; }
                }
                if (AdjustAxis.z > 0.0f)
                {
                    if (transform.position.z > maximumValue.z) { goalPosition = maximumValue; }
                    if (transform.position.z < minimumValue.z) { goalPosition = minimumValue; }
                }

                if(goalPosition != position && !coroutineRunning)
                {
                    StartCoroutine(LerpPosition(position, goalPosition));
                }
            }*/
        }
        
    }

}
