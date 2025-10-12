using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class DialogueManager : MonoBehaviour
{
     public static DialogueManager Instance;
    public GameObject subtitlePanel;
    public TMP_Text subtitleText;

    void Awake()
    {
        Instance = this;
        subtitlePanel.SetActive(false);
    }

    public void ShowSubtitle(string line)
    {
        StopAllCoroutines();
        StartCoroutine(TypeAndShow(line));
    }

    IEnumerator TypeAndShow(string line)
    {
        subtitlePanel.SetActive(true);
        subtitleText.text = "";
        foreach (char c in line)
        {
            subtitleText.text += c;
            yield return new WaitForSeconds(0.03f);
        }
    }

    public void HideSubtitle()
    {
        subtitlePanel.SetActive(false);
    }
}
