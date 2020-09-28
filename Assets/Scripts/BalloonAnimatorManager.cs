using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonAnimatorManager : MonoBehaviour
{
    public Animator balloonAnim;
    float float_var = 0f;
    public float newSegments, oldSegments = 0f;
    public static bool startPumping = false;

    void Start()
    {
        balloonAnim = GetComponent<Animator>();
        balloonAnim.Play("Balloon");
        balloonAnim.speed = 0;
    }


    void FixedUpdate()
    {
        float_var = balloonAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (startPumping)
        {
            balloonAnim.speed = 1;
            newSegments += DataToSave.FillingBalloon();
            startPumping = false;
        }

        if (float_var < newSegments)
        {
            balloonAnim.speed = 1;
            TaskLogic.isBalloonInflating = true;
        }
        else
        {
            balloonAnim.speed = 0;
            TaskLogic.isBalloonInflating = false;
        }
        
        if (float_var > 0.99f)
        {
            EventManager.BalloonExplode(this.gameObject);
        }
    }
}
