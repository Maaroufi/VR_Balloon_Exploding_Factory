using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class KeyDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var key = other.GetComponentInChildren<TextMeshPro>();

        if (!UniversalKey.keyJustHit)
        {
            if (key != null)
            {
                var keyFeedback = other.gameObject.GetComponent<KeyFeedback>();

                if (keyFeedback.keyCanBeHitAgain)
                {
                    if (PanelManager.informationPanel)
                    {
                        if (key.text == "<-")
                        {
                            PanelManager.ScrollText(PanelManager.RectTransformScrollTextInfo);
                        }
                        else if (key.text == "Exit")
                        {
                            Application.Quit();
                        }
                        else if (key.text == "Continue")
                        {
                            PanelManager.FromInformationToAgreement();
                        }
                        else
                        {
                        }
                        keyFeedback.keyHit = true;
                    }
                    else if (PanelManager.agreementPanel)
                    {
                        if (key.text == "<-")
                        {
                            PanelManager.ScrollText(PanelManager.RectTransformScrollTextAgreement);
                        }
                        else if (key.text == "Exit")
                        {
                            Application.Quit();
                        }
                        else if (key.text == "Continue")
                        {
                            PanelManager.FromAgreementToID();
                        }
                        else
                        {
                        }
                        keyFeedback.keyHit = true;
                    }
                    else if (PanelManager.panelAskID)
                    {
                        if (key.text == "<-")
                        {
                            PanelManager.DeleteText();
                        }
                        else if (key.text == "Exit")
                        {
                            Application.Quit();
                        }
                        else if (key.text == "Continue")
                        {
                            PanelManager.FromIdToHand();
                        }
                        else
                        {
                            PanelManager.WriteText(key.text);
                        }
                        keyFeedback.keyHit = true;
                    }
                    else if (PanelManager.panelAskHand)
                    {
                        if (key.text == "Exit")
                        {
                            Application.Quit();
                        }
                        else if (key.text == "Continue")
                        {
                            if (PanelManager.canLeave)
                            {
                                LevelChanger.FadeToLevel(1);
                            }
                        }
                        else if (key.text == "<-")
                        {
                            PanelManager.SwitchToggle();
                        }
                        else
                        {
                        }
                        keyFeedback.keyHit = true;
                    }
                }
            }
        }
    }
}
