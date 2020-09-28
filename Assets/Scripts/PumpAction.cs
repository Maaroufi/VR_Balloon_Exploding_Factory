using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpAction : MonoBehaviour
{
    private bool balloonStoppedDetected = false;
    private bool alreadyAttached = false;

    void OnEnable()
    {
        EventManager.OnStopBalloon += StartPumpBalloon;
    }

    void OnDisable()
    {
        EventManager.OnStopBalloon -= StartPumpBalloon;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == ("TriggerStop") && !alreadyAttached)
        {
            this.gameObject.AddComponent<Animator>();
            Animator Balloon_Animator = GetComponent<Animator>();
            this.gameObject.AddComponent<BalloonAnimatorManager>();
            Balloon_Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/Balloon");
            EventManager.StopBalloon();
            alreadyAttached = true;
        }
        else
        {
            return;
        }
    }

    private void StartPumpBalloon()
    {
        balloonStoppedDetected = true;
    }
}
