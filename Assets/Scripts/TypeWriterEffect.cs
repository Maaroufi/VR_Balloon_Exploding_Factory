using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{
    public float delay = 0.1f;
    private string fullText;
    private string currentText = "";
    private AudioSource audioSource;

    private void Start()
    {
        fullText = this.GetComponent<TextMeshProUGUI>().text;
        StartCoroutine(DisplayText());
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    IEnumerator DisplayText(){
        for (int i = 0; i < fullText.Length + 1; i++)
        {
            currentText = fullText.Substring(0, i);
            this.GetComponent<TextMeshProUGUI>().text = currentText;
            yield return new WaitForSeconds(delay);
        }
        audioSource.Stop();
    }

}
