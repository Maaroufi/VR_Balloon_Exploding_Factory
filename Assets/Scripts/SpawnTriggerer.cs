using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpawnTriggerer : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == ("Balloon"))
        {
            EventManager.SpawnTriggered();
        }
        else
        {
            return;
        }
    }
}
