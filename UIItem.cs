using MyBox;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// This script is to control the background highlight and enable/disable functions of UIItems. Consider this as a replacement for PC canvas buttons in VR.

namespace ConversationMatrixTool
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(BoxCollider))]
    public class UIItem : MonoBehaviour
    {
        private BoxCollider boxCollider;
        private RectTransform rectTransform;
        private bool invoked;

        [Range(0f, .666f)] [Tooltip("time before the player can re-invoke the item")]
        public float cooldownTime = .2f;

        private float counter;

        [Tooltip(
            "Set this false to validate the button automatically. Will turn to true after validation")]
        //so that it does not keep validating and cause a drop in the performance
        public bool isValidated;

        [Tooltip("TextMeshPro Components for normal and highlighted text")]
        public TextMeshProUGUI normalText, highlightedText;

        [Tooltip("Canvas Groups for normal and highlighted mode")] [ReadOnly]
        public CanvasGroup normal, highlighted;

        [Tooltip("Colours for text in normal and highlighted mode")]
        public Color normalTextColor = Color.black, highlightedTextColor = Color.white;

        [Tooltip("Set this to false if the item is an old school UI button")]
        public bool isModern = true;

        private RectTransform rectT;
        private Image bg;

        [Tooltip("Amount of movement of the button on the Z axis when highlighted")] [Range(0, 100)]
        public float highlightDistance = 66.6f;

        [ReadOnly] public Color originalColor, normalColor;

        [Tooltip("Colours for background in normal and highlighted mode")]
        public Color highlightedBGColor = Color.green;

        private float baseHeight;

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            baseHeight = rectTransform.anchoredPosition3D.z;
            if (bg == null) bg = GetComponent<Image>();
            if (bg != null) originalColor = bg.color;
            ResetNormalColor();
        }

        public void ChangeNormalColor(Color color)
        {
            normalColor = color;
        }

        public void ResetNormalColor()
        {
            normalColor = originalColor;
        }

        [ButtonMethod()]
        public void InvokeButtonPress()
        {
            if (!invoked)
            {
                invoked = true;
                counter = 0f;
                var tempButton = GetComponent<Button>();
                if (tempButton != null)
                    if (tempButton.onClick.GetPersistentEventCount() > 0)
                        tempButton.onClick.Invoke();
            }
        }


        private void Update()
        {
            if (invoked)
            {
                counter += Time.deltaTime;
            }

            if (counter > cooldownTime)
            {
                invoked = false;
                counter = 0;
            }
        }

        private void OnEnable()
        {
            if (!isValidated) ValidateCollider();
            ResetNormalColor();
        }

        private void ValidateCollider()
        {
            rectTransform = GetComponent<RectTransform>();

            boxCollider = GetComponent<BoxCollider>();

            boxCollider.size = rectTransform.sizeDelta;

            isValidated = true;
        }

        public void HighlightBackground(bool highlight)
        {
            if (isModern)
            {
                if (normal == null) normal = transform.GetChild(0).GetComponent<CanvasGroup>();
                if (transform.childCount < 2) return;
                if (highlighted == null) highlighted = transform.GetChild(1).GetComponent<CanvasGroup>();
                if (highlight)
                {
                    if (normal == null) return;
                    if (highlighted == null) return;
                    normal.alpha = 0f;
                    highlighted.alpha = 1f;
                    if (normalText == null)
                    {
                        normalText = normal.transform.GetComponentInChildren<TextMeshProUGUI>();
                        if (normalText != null)
                            normalText.color = normalTextColor;
                    }

                    if (normalText != null) normalText.fontStyle = FontStyles.Bold;

                    if (highlightedText == null)
                    {
                        highlightedText = highlighted.transform.GetComponentInChildren<TextMeshProUGUI>();
                        if (highlightedText != null)
                            highlightedText.color = highlightedTextColor;
                    }

                    if (highlightedText != null) highlightedText.fontStyle ^= FontStyles.Bold;
                }
                else
                {
                    if (normal == null) return;
                    if (highlighted == null) return;
                    normal.alpha = 1f;
                    highlighted.alpha = 0f;
                    if (normalText == null)
                    {
                        normalText = normal.transform.parent.GetComponentInChildren<TextMeshProUGUI>();
                        if (normalText != null)
                            normalText.color = normalTextColor;
                    }

                    if (normalText != null) normalText.fontStyle ^= FontStyles.Bold;

                    if (highlightedText == null)
                    {
                        highlightedText = highlighted.transform.parent.GetComponentInChildren<TextMeshProUGUI>();
                        if (highlightedText != null)
                            highlightedText.color = highlightedTextColor;
                    }

                    if (highlightedText != null) highlightedText.fontStyle = FontStyles.Bold;
                }
            }
            else
            {
                if (rectT == null) rectT = GetComponent<RectTransform>();

                if (rectT != null)
                {
                    var temp = rectT.anchoredPosition3D;
                    var text = GetComponent<Text>();
                    if (highlight)
                    {
                        rectT.anchoredPosition3D = new Vector3(temp.x, temp.y, baseHeight - highlightDistance);
                        if (text != null) text.color = highlightedTextColor;
                    }
                    else
                    {
                        rectT.anchoredPosition3D = new Vector3(temp.x, temp.y, baseHeight);
                        if (text != null) text.color = normalTextColor;
                    }
                }

                if (bg == null) bg = GetComponent<Image>();

                if (bg != null)
                {
                    if (highlight)
                        bg.color = highlightedBGColor;
                    else bg.color = normalColor;
                }
            }
        }
    }
}