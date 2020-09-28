using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPositioning : MonoBehaviour
{
    public Transform targetPositionRight;
    public Transform targetPositionLeft;
    private Transform targetPosition;
    public float speed = 1.0f;
    private bool isGoingTowards = true;
    private bool test = false;
    private bool init = true;
    public GameObject handObjectRight;
    public GameObject handObjectLeft;
    private GameObject handObject;
    private int interval = 10;
    public static AudioSource audioSource;
    private string handedness;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        handedness = PlayerPrefs.GetString("handedness");

        if (handedness == "Right")
        {
            targetPosition = targetPositionRight;
            handObject = handObjectRight;
        }
        else
        {
            targetPosition = targetPositionLeft;
            handObject = handObjectLeft;
        }
    }
    void FixedUpdate()
    {
        if (isGoingTowards)
        {
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, step);
        }
        if (Vector3.Distance(transform.position, targetPosition.transform.position) < 0.1 && init)
        {
            this.transform.parent = handObject.transform;
            this.gameObject.AddComponent<FixedJoint>();
            this.GetComponent<FixedJoint>().connectedBody = handObject.GetComponent<Rigidbody>();

            init = false;
            isGoingTowards = false;
            test = true;
        }
        if (test)
        {
            if (handedness == "Right")
            {
                float step = speed * Time.deltaTime; // calculate distance to move
                Vector3 newTrans = new Vector3(-0.115f, -0.1f, 0.11f);
                transform.localRotation = Quaternion.Euler(14, 0, 0);
                transform.localPosition = Vector3.MoveTowards(newTrans, newTrans, step);
                test = false;
            }
            else
            {
                float step = speed * Time.deltaTime; // calculate distance to move
                Vector3 newTrans = new Vector3(0.052f, 0.066f, -0.124f);
                transform.localRotation = Quaternion.Euler(-3.046f, -153.825f, -4.711f);
                transform.localPosition = Vector3.MoveTowards(newTrans, newTrans, step);
                test = false;
            }

        }
    }

    public static void PlayPumpSound()
    {
        audioSource.Play();
    }
}
