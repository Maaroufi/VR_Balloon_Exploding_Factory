using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountTriggerBalloon : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        if (other.tag == ("Balloon"))
        {
            TaskLogic.currentBalloon++;
            TaskLogic.updateText = true;
            TaskLogic.startCountdown = true;
        }
        else
        {
            return;
        }
    }
}
