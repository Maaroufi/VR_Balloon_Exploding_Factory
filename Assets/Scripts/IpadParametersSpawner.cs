using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IpadParametersSpawner : MonoBehaviour
{
    public GameObject tabletParametersLeft;
    public GameObject tabletParametersRight;

    void Start()
    {
        string handedness = PlayerPrefs.GetString("handedness");

        //PlayerPrefs.SetInt("ParticipantID", 1111);

        if (PlayerPrefs.GetInt("ParticipantID") == 1111 || PlayerPrefs.GetInt("ParticipantID") == 2222)
        {
            if (handedness == "Right")
            {
                tabletParametersRight.SetActive(false);
                tabletParametersLeft.SetActive(true);
            }
            else
            {
                tabletParametersRight.SetActive(true);
                tabletParametersLeft.SetActive(false);
            }
        }
        else
        {
            tabletParametersLeft.SetActive(false);
            tabletParametersRight.SetActive(false);
        }
    }
}
