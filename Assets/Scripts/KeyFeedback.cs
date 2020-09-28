using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyFeedback : MonoBehaviour
{
    public bool keyHit = false;
    public bool keyCanBeHitAgain = false;
    private SoundHandler soundHandler;

    private float originalYPosition;

    void Start()
    {
        soundHandler = GameObject.FindGameObjectWithTag("SoundHandler").GetComponent<SoundHandler>();
        originalYPosition = transform.position.y;
    }


    void FixedUpdate()
    {
        if (keyHit)
        {
            soundHandler.PlayKeyClick();
            keyHit = false;
            keyCanBeHitAgain = false;
            transform.position += new Vector3(0, -0.03f, 0);
            UniversalKey.keyJustHit = true;
        }
        if (transform.position.y < originalYPosition)
        {
            transform.position += new Vector3(0, 0.005f, 0);
        }
        else
        {
            keyCanBeHitAgain = true;
        }
    }
}
