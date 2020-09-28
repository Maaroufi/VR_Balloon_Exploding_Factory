using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public delegate void OnTriggerSpawn();
    public static event OnTriggerSpawn OnSpawnTriggered;

    public delegate IEnumerator OnEndOfBlock();
    public static event OnEndOfBlock OnTableToFill;

    public delegate void OnStartfBlock();
    public static event OnStartfBlock OnDataToLoad;

    public delegate void OnTriggerStop();
    public static event OnTriggerStop OnStopBalloon;
    public static event OnTriggerStop OnPumpBalloon;

    public delegate void OnBalloonExplode(GameObject currentBalloon);
    public static event OnBalloonExplode OnBalloonExploded;

    public static void SpawnTriggered()
    {
        OnSpawnTriggered?.Invoke();
    }

    public static void BlockStarted()
    {
        OnDataToLoad?.Invoke();
    }

    public static void BlockFinished()
    {
        OnTableToFill?.Invoke();
    }

    public static void StopBalloon()
    {
        OnStopBalloon?.Invoke();
        OnPumpBalloon?.Invoke();
    }

    public static void BalloonExplode(GameObject currentBalloon)
    {
        OnBalloonExploded?.Invoke(currentBalloon);
    }
}