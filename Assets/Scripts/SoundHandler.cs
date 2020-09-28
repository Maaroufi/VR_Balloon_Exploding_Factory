using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundHandler : MonoBehaviour
{
    public AudioClip keyClick;
    private AudioSource clickSource;

    void Start()
    {
        clickSource = gameObject.GetComponent<AudioSource>();
    }

    public void PlayKeyClick()
    {
        clickSource.PlayOneShot(keyClick);
    }

}
