using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    private string fullText;
    private string scoreText;
    public static int oldScore = 0;
    public static int newScore = 0;
    public static float timeWait;
    public static RawImage m_RawImage;
    public static List<Texture> m_Texture;
    public static TextMeshProUGUI fullTextMesh;
    public static TextMeshProUGUI scoreTextMesh;

    public RawImage m_RawImageInstance;
    public List<Texture> m_TextureInstance;
    public TextMeshProUGUI fullTextMeshInstance;
    public TextMeshProUGUI scoreTextMeshInstance;

    void Start()
    {
        m_RawImage = m_RawImageInstance;
        m_Texture = m_TextureInstance;
        fullTextMesh = fullTextMeshInstance;
        scoreTextMesh = scoreTextMeshInstance;

        fullText = fullTextMesh.text;
        scoreText = scoreTextMesh.text;
        m_RawImage.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (TaskLogic.currentBlock == 4 && TaskLogic.updateText)
        {
            fullTextMesh.fontSize = 0.02f;
            fullTextMesh.alignment = TextAlignmentOptions.Center;
            fullTextMesh.text = "<color=yellow><align=center><size=0.03>\n\n Keep Going! </size></align></color>";
            scoreTextMesh.text = " ";
        }
        else
        {
            if (oldScore != newScore)
            {
                StartCoroutine(ChangeScore());
            }

            if (TaskLogic.updateText)
            {
                StartCoroutine(ChangeText());
                TaskLogic.updateText = false;
            }
        }

    }

    public static IEnumerator ChangeText()
    {
        if (TaskLogic.currentBlock == 4)
        {
            fullTextMesh.fontSize = 0.02f;
            fullTextMesh.alignment = TextAlignmentOptions.Center;
            fullTextMesh.text = "<color=yellow><align=center><size=0.03>\n\n Keep Going! </size></align></color>";
        }
        else
        {
            m_RawImage.gameObject.SetActive(true);
            fullTextMesh.fontSize = 0.02f;
            fullTextMesh.alignment = TextAlignmentOptions.Top;
            m_RawImage.texture = m_Texture[DataToSave.rewards];
            fullTextMesh.text = "<color=yellow><align=center><size=0.03>\n\n Your Reward </size></align></color>\n\n\n <voffset=-0.2em>Trial "
                                + TaskLogic.currentBalloon + "</voffset>";

            yield return new WaitForFixedUpdate();
            timeWait = 3;
            yield return new WaitForSeconds(timeWait);
            m_RawImage.gameObject.SetActive(false);

            fullTextMesh.alignment = TextAlignmentOptions.Left;
            fullTextMesh.text = "<color=green><align=center><size=0.04><style=Normal>\n Level: " + TaskLogic.currentBlock + "</style></size></color>"
                                + "\nBalloon: " + TaskLogic.currentBalloon
                                + "\nTotal destroyed: " + TaskLogic.DestroyedCounter;
        }
    }

    public static IEnumerator ChangeScore()
    {
        if (TaskLogic.currentBlock == 4)
        {
            scoreTextMesh.text = " ";
        }
        else
        {
            for (int i = oldScore; i <= newScore; i = i + 10)
            {
                scoreTextMesh.text = "Score: " + i.ToString();

                yield return new WaitForSeconds(0.01f);
            }
            oldScore = newScore;
        }
    }
}
