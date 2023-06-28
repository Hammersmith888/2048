using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LocalizationLoader : MonoBehaviour
{
    [System.Serializable]
    public struct LanguageTexts
    {
        public string _id;
        public string _en;
        public string _ru;
        public string _ro;
        public string _fr;
        public string _de;
        public string _sp;
        public string _it;
        public string _jp;
        public string _ko;
        public string _tr;
    }

    [System.Serializable]
    public class Language_Texts : ArrayInClassWrapper<LanguageTexts>
    {
        public Language_Texts()
        {
        }

        public Language_Texts(int capacity) : base(capacity)
        {
        }
    }

    [System.Serializable]
    public class OneLoc
    {
        public string _langId;
        public string _text;
    }

    [System.Serializable]
    public class SingleTextLoc
    {
        public string _textId;
        public List<OneLoc> oneLoc = new List<OneLoc>();
    }

    [SerializeField]
    private List<SingleTextLoc> _texts = new List<SingleTextLoc>();

    public string langId;

    public static LocalizationLoader Instance;

    public event Action onTextsUpdate;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //GetComponent<GoogleSpeadsheetGet>().onGetData += SetOfflineTextsEditor;
        //GetComponent<GoogleSpeadsheetGet>().onGetDataFromLocalData += SetOfflineTextsEditor;
    }

    void Start()
    {
        
        SetDefaultLanguage();
    }


    public static string GetStringST(string _id)
    {
        if (Instance != null)
        {
            return Instance.GetString(_id);
        }
        return _id;
    }

    public string GetString(string _id)
    {
        if (string.IsNullOrEmpty(_id))
        {
            return "";
        }
        string to_return = "";
        int textsCount = _texts.Count;
        int locCount;


        for (int i = 0; i < textsCount; i++)
        {
            if (_id == _texts[i]._textId)
            {
                locCount = _texts[i].oneLoc.Count;
                for (int j = 0; j < locCount; j++)
                {
                    if (_texts[i].oneLoc[j]._langId == langId)
                    {
                        to_return = _texts[i].oneLoc[j]._text;
                        break;
                    }
                }
                break;
            }
        }
        return to_return;
    }

    public void SetDefaultLanguage(string setLang = "Unknown")
    {
        if (!PlayerPrefs.HasKey("CurrentLanguage"))
        {
            PlayerPrefs.SetString("CurrentLanguage", Application.systemLanguage.ToString());
        }
        else if (setLang != "Unknown")
        {
            PlayerPrefs.SetString("CurrentLanguage", setLang);
        }
        string savedId = PlayerPrefs.GetString("CurrentLanguage");
        /*

                langId = savedId switch
                {
                    "English" => "EN",
                    "Unknown" => "EN",
                    "Russian" => "RU",
                    "German" => "DE",
                    "Spanish" => "SP",
                    "Japanese" => "JP",
                    "Korean" => "KO",
                    _ => "EN"
                };
         */

        langId = "EN";


        UpdateAllTexts();
    }

    private void UpdateAllTexts()
    {
        onTextsUpdate?.Invoke();
    }

    public void SetOfflineTextsEditor(GoogleSpeadsheetGet.QueryType query, List<string> objTypeNames, List<string> jsonData)
    {
        Debug.Log("Applying localization");
        
        var _textsTemp = Language_Texts.Create<Language_Texts>(GSFUJsonHelper.JsonArray<LanguageTexts>(jsonData[2]));
        _texts.Clear();

        foreach (LanguageTexts textTemp in _textsTemp)
        {
            if (!string.IsNullOrEmpty(textTemp._id))
            {
                SingleTextLoc newLoc = new SingleTextLoc();

                OneLoc engLoc = new OneLoc();
                newLoc._textId = textTemp._id;
                engLoc._langId = "EN";
                engLoc._text = textTemp._en.Replace("[l]", "\n");
                newLoc.oneLoc.Add(engLoc);

                OneLoc ruLoc = new OneLoc();
                ruLoc._langId = "RU";
                ruLoc._text = textTemp._ru.Replace("[l]", "\n");
                newLoc.oneLoc.Add(ruLoc);

                OneLoc deLoc = new OneLoc();
                deLoc._langId = "DE";
                deLoc._text = textTemp._de.Replace("[l]", "\n");
                newLoc.oneLoc.Add(deLoc);

                OneLoc spLoc = new OneLoc();
                spLoc._langId = "SP";
                spLoc._text = textTemp._sp.Replace("[l]", "\n");
                newLoc.oneLoc.Add(spLoc);

                OneLoc jpLoc = new OneLoc();
                jpLoc._langId = "JP";
                jpLoc._text = textTemp._jp.Replace("[l]", "\n");
                newLoc.oneLoc.Add(jpLoc);

                OneLoc koLoc = new OneLoc();
                koLoc._langId = "KO";
                koLoc._text = textTemp._ko.Replace("[l]", "\n");
                newLoc.oneLoc.Add(koLoc);

                _texts.Add(newLoc);
            }
        }

        UpdateAllTexts();
    }



}

