using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSound : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip startConveyor;
    public AudioClip stopConveyor;
    private bool currentState = true;
    private bool lastState = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        currentState = TaskLogic.isBalloonStopped;

        if (currentState != lastState && currentState == false)
        {
            audioSource.PlayOneShot(startConveyor);
            lastState = currentState;
        }
        else if (currentState != lastState && currentState == true)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(stopConveyor);
            lastState = currentState;
        }
    }
}
