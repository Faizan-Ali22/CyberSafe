using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] private float charactersPerSecond = 9f;
    [SerializeField] private bool loopEffect = true;
    [SerializeField] private float delayBeforeLoop = 1f;
    
    private TextMeshProUGUI tmpText;
    private Text uiText;
    private string fullText;
    
    void Awake()
    {
        tmpText = GetComponent<TextMeshProUGUI>();
        uiText = GetComponent<Text>();
        
        if (tmpText)
            fullText = tmpText.text;
        else if (uiText)
            fullText = uiText.text;
    }
    
    void OnEnable()
    {
        StartCoroutine(AnimateText());
    }
    
    void OnDisable()
    {
        StopAllCoroutines();
    }
    
    private IEnumerator AnimateText()
    {
        while (true)
        {
            for (int i = 0; i <= fullText.Length; i++)
            {
                if (tmpText)
                    tmpText.text = fullText.Substring(0, i);
                else if (uiText)
                    uiText.text = fullText.Substring(0, i);
                
                yield return new WaitForSeconds(1f / charactersPerSecond);
            }
            
            if (!loopEffect)
                break;
                
            yield return new WaitForSeconds(delayBeforeLoop);
        }
    }
}