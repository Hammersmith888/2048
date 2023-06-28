using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoosterPopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI descriptionText;
    [SerializeField]
    private TextMeshProUGUI headTitle;

    public Image previewIconImage;

    [HideInInspector]
    public PopupShow AnimationPanel
    {
        get { 
            if (_animationPanel == null)
                _animationPanel = GetComponent<PopupShow>();
            return _animationPanel;
        }
    }

    private PopupShow _animationPanel;

    public Vector3 endAnimScale;

    private Vector3 initIconPosition;

    private void Awake()
    {
        initIconPosition = previewIconImage.transform.localPosition;
    }

    public void UpdateData(Booster booster)
    {
        descriptionText.text = LocalizationLoader.Instance.GetString(booster.descriptionID);
        headTitle.text = LocalizationLoader.Instance.GetString(booster.titleID);

        previewIconImage.sprite = booster.Icon;

        ResetPreviewImage();
    }

    public void ResetPreviewImage()
    {
        previewIconImage.transform.localPosition = initIconPosition;
        previewIconImage.transform.localScale = Vector3.one;
    }
}
