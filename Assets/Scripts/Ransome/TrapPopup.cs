using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TrapPopup : MonoBehaviour
{
     public Button closeButton;
    public Button payButton;

    void Start()
    {
        closeButton.onClick.AddListener(() => Destroy(gameObject));
        payButton.onClick.AddListener(() =>
        {
            GameManager.Instance.SetState(GameManager.GameState.GameOverTrap);
            Destroy(gameObject);
        });
    }
}
