using UnityEngine;
using System.Collections;
using System.Collections.Generic;   
using UnityEngine.UI;
using TMPro;

public class PCHealthUI : MonoBehaviour
{
   public Slider healthSlider;
    public TextMeshProUGUI healthLabel;

    public void SetValue(float normalized, string labelText = null)
    {
        if (healthSlider != null)
            healthSlider.value = Mathf.Clamp01(normalized);

        if (healthLabel != null && labelText != null)
            healthLabel.text = labelText;
    }
}
