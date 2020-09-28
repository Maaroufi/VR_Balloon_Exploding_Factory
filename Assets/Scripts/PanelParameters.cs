using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelParameters : MonoBehaviour
{
    public TextMeshProUGUI fullTextMesh;
    private string paramText;
    public static bool updateOncePlease = false;

    void Start()
    {
        paramText = fullTextMesh.text;
        Invoke("UpdateParameters", 5f);
    }

    void FixedUpdate()
    {
        if (DataToSave.isHandCalibrationStarted || DataToSave.isHandSpeedCalibrationStarted || DataToSave.isTrialStarted
            || updateOncePlease)
        {
            UpdateParameters();
            updateOncePlease = false;
        }
    }

    public void UpdateParameters()
    {
        fullTextMesh.fontSize = 0.01f;
        fullTextMesh.alignment = TextAlignmentOptions.TopLeft;
        fullTextMesh.textStyle = TMP_Style.NormalStyle; 

        fullTextMesh.text = "<color=yellow><align=center><size=0.01><style=H2>\n Parameters </style></size></align></color>" +
                            "\nGroupe: " + DataToSave.groupBelong +
                            "\nDifficulty Level: " + DataToSave.difficultyLevel +
                            "\nRelative time (s): " + DataToSave.relativeTime +
                            "\nNumber Cycles: " + DataToSave.Number_Cycles +
                            "\nVelocity Baseline (m/s): " + DataToSave.speedApertureAssessed +
                            "\nVelocity Reference: " + DataToSave.speedRef +
                            "\nTotal Aperture Baseline (m): " + DataToSave.totalApertureAssessed +
                            "\nAperture Reference: " + DataToSave.apertureRef +
                            "\nCurrent Aperture: " + DataToSave.handAperture +
                            "\nVelocity last cycle: " + DataToSave.speedApertureCycle +
                            "\nAperture last cycle: " + DataToSave.lastCycleAperture +
                            "\nMin Aperture last cycle: " + DataToSave.lastHandMinAperture +
                            "\nMax Aperture last cycle: " + DataToSave.lastHandMaxAperture +
                            "\nAperture diff. (last - Ref): " + DataToSave.apertureDiff +
                            "\nVelocity diff. (last - Ref): " + DataToSave.speedDiff +
                            "\nPerformance: " + DataToSave.performance +
                            "\nPerformance Previous Block: " + DataToSave.performanceBlockBefore
                            ;
    }
}
