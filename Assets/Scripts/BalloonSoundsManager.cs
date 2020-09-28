using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonSoundsManager : MonoBehaviour
{
    private AudioSource audioSource;
    //public AudioClip inflateSounds;
    private bool stopUpdate = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (TaskLogic.isBalloonInflating && !stopUpdate)
        {
            Debug.Log("Le ballon gonfle");
            //audioSource.clip = inflateSounds[Random.Range(0, inflateSounds.Count)];
            audioSource.Play();
            stopUpdate = true;
        }
        else if (!TaskLogic.isBalloonInflating && stopUpdate)
        {
            audioSource.Stop();
            stopUpdate = false;
        }
    }
}
