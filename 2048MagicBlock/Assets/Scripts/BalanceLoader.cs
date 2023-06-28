using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct BalanceStep
{
    public int maxValue;
    public int bigChanceMin;
    public int bigChanceMax;
    public int midChanceMin;
    public int smallChanceMin;
    public int smallChanceMax;
}

[System.Serializable]
public struct BalanceValues
{
    public int couplesMinSize;
    public int couplesAEnable;
    public int couplesMaxSizeA;
    public int couplesBEnable;

    public BalanceStep step1;
    public BalanceStep step2;
    public BalanceStep step3;
    public BalanceStep step4;
    public BalanceStep step5;

    public int wheelFortuneChance;
    public int moneyCellChance;
    public int keyChance;
    public int wheelAttachedSec;
    public int wheelButtonSec;

    public int timerSec;
    public int timeBoosterHelp;
    public int timerRateUsShow1;
    public int timerRateUsShow2;

    public int pauseBetweenMusicMin;
    public int pauseBetweenMusicMax;

};

public class GSFUJsonHelper
{
    public static T[] JsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array = new T[] { };
    }
}


public class BalanceLoader
{
    public bool isLoaded;

    public static BalanceLoader Instance;


    [System.Serializable]
    public class BalanceVar
    {
        public string _textId;
        public string _value;
    }

    [System.Serializable]
    public struct BalanceTexts
    {
        public string _id;
        public string _value;
    }

    [System.Serializable]
    public class BalanceVariablesTexts : ArrayInClassWrapper<BalanceTexts>
    {
        public BalanceVariablesTexts()
        {
        }

        public BalanceVariablesTexts(int capacity) : base(capacity)
        {
        }
    }

    public void Init()
    {
        Instance = this;
        //GoogleSpeadsheetGet.instance.onGetDataFromLocalData += GoogleSheets_onGetData;
        //GoogleSpeadsheetGet.instance.onGetData += GoogleSheets_onGetData;
    }

    public event Action<BalanceValues> onDataLoaded;

    public void GoogleSheets_onGetData(GoogleSpeadsheetGet.QueryType query, List<string> objTypeNames, List<string> jsonData)
    {
        Debug.LogError("loader--data--balance");

        var _textsTemp = BalanceVariablesTexts.Create<BalanceVariablesTexts>(GSFUJsonHelper.JsonArray<BalanceTexts>(jsonData[4]));
        var texts = new List<BalanceVar>();

        foreach (BalanceTexts textTemp in _textsTemp)
        {
            if (!string.IsNullOrEmpty(textTemp._id))
            {
                texts.Add(new BalanceVar()
                {
                    _textId = textTemp._id,
                    _value = textTemp._value
                });
            }
        }

        ParseBalanceVariables(texts);

        isLoaded = true;
    }

    private void ParseBalanceVariables(List<BalanceVar> variables)
    {
        Debug.Log("Applying balance");

        var balanceData = new BalanceValues();

        balanceData.couplesMinSize = FindVariable(variables, "couplesMinSize");
        balanceData.couplesAEnable = FindVariable(variables, "couplesAEnable");
        balanceData.couplesMaxSizeA = FindVariable(variables, "couplesMaxSizeA");
        balanceData.couplesBEnable = FindVariable(variables, "couplesBEnable");

        balanceData.step1 = FindStep(variables, 1);
        balanceData.step2 = FindStep(variables, 2);
        balanceData.step3 = FindStep(variables, 3);
        balanceData.step4 = FindStep(variables, 4);
        balanceData.step5 = FindStep(variables, 5);

        balanceData.wheelFortuneChance = FindVariable(variables, "wheelFortuneChance");
        balanceData.moneyCellChance = FindVariable(variables, "moneyCellChance");
        balanceData.keyChance = FindVariable(variables, "keyChance");
        balanceData.wheelAttachedSec = FindVariable(variables, "wheelAttachedSec");
        balanceData.wheelButtonSec = FindVariable(variables, "wheelButtonSec");

        balanceData.timerSec = FindVariable(variables, "timerSec");
        balanceData.timeBoosterHelp = FindVariable(variables, "timeBoosterHelp");
        balanceData.timerRateUsShow1 = FindVariable(variables, "timerRateUsShow1");
        balanceData.timerRateUsShow2 = FindVariable(variables, "timerRateUsShow2");

        balanceData.pauseBetweenMusicMin = FindVariable(variables, "pauseBetweenMusicMin");
        balanceData.pauseBetweenMusicMax = FindVariable(variables, "pauseBetweenMusicMax");

        onDataLoaded?.Invoke(balanceData);
    }

    private int FindVariable(List<BalanceVar> variables, string name)
    {
        try
        {
            return int.Parse(variables.FirstOrDefault(n => n._textId == name)._value);
        }
        catch (Exception)
        {
            return 0;
            throw;
        }
    }

    private BalanceStep FindStep(List<BalanceVar> variables, int stepID)
    {
        var step = new BalanceStep();

        step.maxValue = FindVariable(variables,       $"maxValue{stepID}");
        step.bigChanceMin = FindVariable(variables, $"bigChanceMin{stepID}");
        step.bigChanceMax = FindVariable(variables, $"bigChanceMax{stepID}");

        step.midChanceMin = FindVariable(variables, $"midChanceMin{stepID}");
        step.smallChanceMin = FindVariable(variables, $"smallChanceMin{stepID}");
        step.smallChanceMax = FindVariable(variables, $"smallChanceMax{stepID}");

        return step;
    }
}

