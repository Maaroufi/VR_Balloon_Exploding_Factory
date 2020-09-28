using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public static TMP_InputField inputField;
    public static GameObject inputFieldObj;
    public static GameObject scrollTextObjAgree;
    public static GameObject scrollTextObjInfo;
    public static RectTransform RectTransformScrollTextInfo;
    public static RectTransform RectTransformScrollTextAgreement;
    public static bool informationPanel = true;
    public static bool agreementPanel = false;
    public static bool panelAskID = false;
    public static bool panelAskHand = false;
    public static bool canLeave = false;

    public GameObject _IDPanel;
    public GameObject _handPanel;
    public GameObject _agreePanel;
    public GameObject _infoPanel;
    public static GameObject infoPanel;
    public static GameObject IDPanel;
    public static GameObject handPanel;
    public static GameObject agreePanel;
    public static Toggle Toggle_L;
    public static Toggle toggle_R;


    void Start()
    {
        inputFieldObj = GameObject.FindGameObjectWithTag("PlayerTextOutput");
        inputField = inputFieldObj.GetComponent<TMP_InputField>();
        scrollTextObjInfo = GameObject.FindGameObjectWithTag("ScrollInformation");
        RectTransformScrollTextInfo = scrollTextObjInfo.GetComponent<RectTransform>();
        scrollTextObjAgree = GameObject.FindGameObjectWithTag("ScrollAgreement");
        RectTransformScrollTextAgreement = scrollTextObjAgree.GetComponent<RectTransform>();
        Toggle_L = GameObject.Find("LeftToggle").GetComponent<Toggle>();
        toggle_R = GameObject.Find("RightToggle").GetComponent<Toggle>();

        infoPanel = _infoPanel;
        IDPanel = _IDPanel;
        handPanel = _handPanel;
        agreePanel = _agreePanel;

        agreePanel.SetActive(false);
        handPanel.SetActive(false);
        IDPanel.SetActive(false);
    }

    public static void ScrollText(RectTransform rect)
    {
        rect.position = new Vector3(rect.position.x,
                                    rect.position.y + 0.5f,
                                    rect.position.z);
    }

    public static void FromInformationToAgreement()
    {
        infoPanel.SetActive(false);
        agreePanel.SetActive(true);
        informationPanel = false;
        agreementPanel = true;
    }

    public static void FromAgreementToID()
    {
        IDPanel.SetActive(true);
        agreePanel.SetActive(false);
        agreementPanel = false;
        panelAskID = true;
    }

    public static void FromIdToHand()
    {
        //Set data to PlayerPrefs
        PlayerPrefs.SetInt("ParticipantID", int.Parse(inputField.text));
        handPanel.SetActive(true);
        IDPanel.SetActive(false);
        panelAskHand = true;
        panelAskID = false;
    }

    public static void DeleteText()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }

    public static void WriteText(string text)
    {
        inputField.text += text;
    }

    public static void SwitchToggle()
    {
        if (Toggle_L.isOn)
        {
            Toggle_L.isOn = false;
            toggle_R.isOn = true;
            PlayerPrefs.SetString("handedness", "Right");
            canLeave = true;
        }
        else if (toggle_R.isOn)
        {
            toggle_R.isOn = false;
            Toggle_L.isOn = true;
            PlayerPrefs.SetString("handedness", "Left");
            canLeave = true;
        }
        else if (!toggle_R.isOn && !Toggle_L.isOn)
        {
            toggle_R.isOn = true;
            PlayerPrefs.SetString("handedness", "Right");
            canLeave = true;
        }
    }
}
