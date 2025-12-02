using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeClothes : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject changeClothesUI;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Player Characters")]
    [SerializeField] private GameObject currentPlayer; // Ayan With-Glasses
    [SerializeField] private GameObject newPlayer; // Ayan Black

    [Header("Camera")]
    [SerializeField] private CameraManager cameraManager;

    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";

    private bool hasChangedClothes = false;

    private void Start()
    {
        // Ensure UI is hidden at start
        if (changeClothesUI != null)
        {
            changeClothesUI.SetActive(false);
        }

        // Ensure fade image is transparent at start
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(false);
        }

        // Ensure new player is disabled at start
        if (newPlayer != null)
        {
            newPlayer.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !hasChangedClothes)
        {
            if (changeClothesUI != null)
            {
                changeClothesUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            if (changeClothesUI != null)
            {
                changeClothesUI.SetActive(false);
            }
        }
    }

    // Call this method from the Change Clothes Button's OnClick event
    public void OnChangeClothesButtonClicked()
    {
        if (hasChangedClothes) return;
        hasChangedClothes = true;

        // Hide the change clothes UI
        if (changeClothesUI != null)
        {
            changeClothesUI.SetActive(false);
        }

        StartCoroutine(ChangeClothesSequence());
    }

    private IEnumerator ChangeClothesSequence()
    {
        // Enable fade image
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
        }

        // Fade to black (alpha 0 to 1)
        yield return StartCoroutine(FadeImage(0f, 1f));

        // Swap players while screen is black
        SwapPlayers();

        // Brief pause while screen is black
        yield return new WaitForSeconds(0.2f);

        // Fade back in (alpha 1 to 0)
        yield return StartCoroutine(FadeImage(1f, 0f));

        // Disable fade image
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(false);
        }

        #if UNITY_EDITOR
        Debug.Log("Clothes changed successfully!");
        #endif
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha)
    {
        if (fadeImage == null) yield break;

        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            color.a = alpha;
            fadeImage.color = color;
            yield return null;
        }

        // Ensure final alpha is set
        color.a = endAlpha;
        fadeImage.color = color;
    }

    private void SwapPlayers()
    {
        if (currentPlayer == null || newPlayer == null) return;

        // Store current player position and rotation
        Vector3 playerPosition = currentPlayer.transform.position;
        Quaternion playerRotation = currentPlayer.transform.rotation;

        // Disable current player
        currentPlayer.SetActive(false);

        // Enable new player at same position
        newPlayer.transform.position = playerPosition;
        newPlayer.transform.rotation = playerRotation;
        newPlayer.SetActive(true);

        // Update camera manager target
        if (cameraManager != null)
        {
            cameraManager.SendMessage("SetTarget", newPlayer.transform, SendMessageOptions.DontRequireReceiver);
            cameraManager.SendMessage("InitializeCamera", SendMessageOptions.DontRequireReceiver);
        }
    }
}
