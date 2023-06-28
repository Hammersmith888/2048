using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

class LocText : MonoBehaviour
{
    [SerializeField]
    private string _thisID;

    /*
     private TextMeshProUGUI textTMP { get {
            if (_textTMP == null)
                _textTMP = GetComponent<TextMeshProUGUI>();

            return _textTMP;
        } }

    private TextMeshProUGUI _textTMP;

    private Text textUGUI
    {
        get
        {
            if (_textUGUI == null)
                _textUGUI = GetComponent<Text>();

            return _textUGUI;
        }
    }

    private Text _textUGUI;
     */

    private TextMeshProUGUI _textTMP;
    private Text _textUGUI;

    private void Awake()
    {
        _textTMP = GetComponent<TextMeshProUGUI>();
        _textUGUI = GetComponent<Text>();
    }

    private void Start()
    {
        if (LocalizationLoader.Instance == null) return;

        LocalizationLoader.Instance.onTextsUpdate += UpdateData;
        UpdateData();
    }

    private void UpdateData()
    {
        if (LocalizationLoader.Instance == null)
            return;

        var text = LocalizationLoader.Instance.GetString(_thisID);


        if(_textTMP != null)
            _textTMP.text = text;
        else if (_textUGUI != null)
            _textUGUI.text = text;
    }

    private void OnEnable()
    {
        UpdateData();
    }
}

