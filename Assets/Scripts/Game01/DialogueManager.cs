using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public GameObject subtitlePanel; // panel with background
    public Text subtitleText;

    void Awake()
    {
        Instance = this;
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
    }

    public void ShowSubtitle(string line)
    {
        if (subtitlePanel != null) {
            subtitlePanel.SetActive(true);
            subtitleText.text = line;
        }
    }

    public void HideSubtitle()
    {
        if (subtitlePanel != null) subtitlePanel.SetActive(false);
    }
}
