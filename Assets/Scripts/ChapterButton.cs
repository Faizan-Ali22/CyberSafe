using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Shift
{
    public class ChapterButton : MonoBehaviour
    {
        [Header("Resources")]
        public Sprite backgroundImage;
        public string buttonTitle = "My Title";
        [TextArea] public string buttonDescription = "My Description";

        [Header("Settings")]
        public bool useCustomResources = false;

        [Header("Progress")]
        [Tooltip("Index of this chapter (0 to 7). 0 is first chapter.")]
        public int chapterIndex = 0;

        [Header("Status")]
        public bool enableStatus = true;
        public StatusItem statusItem;

        Image backgroundImageObj;
        TextMeshProUGUI titleObj;
        TextMeshProUGUI descriptionObj;
        Transform statusNone;
        Transform statusLocked;
        Transform statusCompleted;

        public enum StatusItem
        {
            None,
            Locked,
            Completed
        }

        void Start()
        {
            if (!useCustomResources)
            {
                backgroundImageObj = transform.Find("Content/Background").GetComponent<Image>();
                titleObj = transform.Find("Content/Texts/Title").GetComponent<TextMeshProUGUI>();
                descriptionObj = transform.Find("Content/Texts/Description").GetComponent<TextMeshProUGUI>();

                backgroundImageObj.sprite = backgroundImage;
                titleObj.text = buttonTitle;
                descriptionObj.text = buttonDescription;
            }

            if (enableStatus)
            {
                statusNone      = transform.Find("Content/Texts/Status/None");
                statusLocked    = transform.Find("Content/Texts/Status/Locked");
                statusCompleted = transform.Find("Content/Texts/Status/Completed");

                UpdateStatusFromProgress();
                ApplyStatusVisuals();
            }
        }

        private void UpdateStatusFromProgress()
        {
            if (ProgressManager.Instance == null)
            {
                Debug.LogWarning("ChapterButton: No ProgressManager found. Using inspector status.");
                return;
            }

            bool isUnlocked  = ProgressManager.Instance.IsChapterUnlocked(chapterIndex);
            bool isCompleted = ProgressManager.Instance.IsChapterCompleted(chapterIndex);

            if (!isUnlocked)
            {
                statusItem = StatusItem.Locked;
            }
            else if (isCompleted)
            {
                statusItem = StatusItem.Completed;
            }
            else
            {
                statusItem = StatusItem.None;
            }
        }

        private void ApplyStatusVisuals()
        {
            if (statusNone != null)
                statusNone.gameObject.SetActive(statusItem == StatusItem.None);

            if (statusLocked != null)
                statusLocked.gameObject.SetActive(statusItem == StatusItem.Locked);

            if (statusCompleted != null)
                statusCompleted.gameObject.SetActive(statusItem == StatusItem.Completed);
        }
    }
}