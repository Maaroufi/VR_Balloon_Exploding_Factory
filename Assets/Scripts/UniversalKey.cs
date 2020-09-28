using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalKey : MonoBehaviour
{
    public static bool keyJustHit = false;
    private bool alreadyLaunch = false;

    void Update()
    {
        if (keyJustHit && !alreadyLaunch)
        {
            StartCoroutine(KeyWait());
            alreadyLaunch = true;
        }
    }

    IEnumerator KeyWait()
    {
        yield return new WaitForSeconds(0.5f);
        keyJustHit = false;
        alreadyLaunch = false;
    }
}
