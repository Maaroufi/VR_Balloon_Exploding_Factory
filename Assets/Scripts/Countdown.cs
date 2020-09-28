using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Countdown : MonoBehaviour
{
    private float delay = 1f;
    private string text;
    private int countdown = 10;

    private void FixedUpdate()
    {
        if (TaskLogic.startCountdown && TaskLogic.isBalloonStopped)
        {
            text = this.GetComponent<TextMeshProUGUI>().text;
            StartCoroutine(DisplayText());
            TaskLogic.startCountdown = false;
        }
    }

    IEnumerator DisplayText()
    {
        for (int i = countdown; i >= 0; i--)
        {
            if (i == 0)
            {
                this.GetComponent<TextMeshProUGUI>().fontSize = 0.05f;
                this.GetComponent<TextMeshProUGUI>().text = "Go!";
                yield return new WaitForSeconds(1f);
                this.GetComponent<TextMeshProUGUI>().text = "";
                DataToSave.isTrialFinished = false;
                DataToSave.isTrialStarted = true;
                DataToSave.isNewTrial = true;
                if (TaskLogic.currentBalloon % 5 == 1)
                {
                    DataToSave.isBlockFinished = false;
                    DataToSave.isBlockStarted = true;
                }
            }
            else
            {
                this.GetComponent<TextMeshProUGUI>().fontSize = 0.09f;
                this.GetComponent<TextMeshProUGUI>().text = i.ToString();
            }

            yield return new WaitForSeconds(delay);
        }
    }
}
