using UnityEngine;

public struct GameData
{
    //public int money;

    public int score { get => _score; set { _score = value; highScore = Mathf.Max(highScore, score);Debug.Log("score"); } }
    private int _score;
    public int freeSpeenCount { 
        get { 
            return _freeSpeenCount; 
        } 
        set {
            _freeSpeenCount = value;
            PlayerPrefs.SetInt("freeSpeenCount", _freeSpeenCount);
        }
    }
    private int _freeSpeenCount;
    public static int highScore { get; private set; }

     

    public static bool isCellsLocked;
    public static bool isTutorialShowedUp;
    public static bool isCouplesShowedUp;

    public static int dailyRewardDay = 0;
    public static bool lockFallCells;


    public static bool _10Opened => PlayerPrefs.GetInt("HighCellNumber", 2) >= 10;
    public static bool _18Opened => PlayerPrefs.GetInt("HighCellNumber", 2) >= 18;

    public bool endlessUnlocked;
    public bool isEndless;

    //TODO: переделать на binary
    public void LoadData()
    {
        GameData.isTutorialShowedUp = Utilites.LoadBool("IsTutShowed");
        endlessUnlocked = Utilites.LoadBool("EndlessUnlocked");
        GameData.dailyRewardDay = PlayerPrefs.GetInt("DailyReward", 0);
        freeSpeenCount = PlayerPrefs.GetInt("freeSpeenCount", 0);

    }

    public void SaveData()
    {
        Utilites.SaveBool("IsTutShowed", GameData.isTutorialShowedUp);
        Utilites.SaveBool("EndlessUnlocked", endlessUnlocked);
        PlayerPrefs.SetInt("DailyReward", GameData.dailyRewardDay);
        PlayerPrefs.SetInt("freeSpeenCount", freeSpeenCount);
    }
    public void ClearData()
    {
        Utilites.SaveBool("IsTutShowed", false);
        Utilites.SaveBool("EndlessUnlocked", false);
        PlayerPrefs.SetInt("DailyReward", 1);
        PlayerPrefs.SetInt("freeSpeenCount", 0);
    }
}


