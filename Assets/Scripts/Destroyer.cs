using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.name == ("TriggerDestroy"))
        {
            Destroy(gameObject);
            if (TaskLogic.isTaskFinished)
            {
                TaskLogic.isBalloonStopped = true;
            }
        }
        else
        {
            return;
        }
    }
}
