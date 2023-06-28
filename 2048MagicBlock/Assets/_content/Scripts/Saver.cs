using Mkey;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Saver : MonoBehaviourSingleton<Saver>
{

    private const string INFO_KEY = "PlayerInfo";
    private const string MAP_KEY = "MapInfo";
    private const string DATA_KEY = "DataInfo";
    private PlayerData _playerData;
    public PlayerData PlayerData => _playerData;

    GamePlayerData gamePlayerData;

    public DailyTasksManager _dailyTaskManager;
    private void Awake()
    {
        _dailyTaskManager = FindObjectOfType<DailyTasksManager>();
    }
    private void Start()
    {
        if (PlayerPrefs.HasKey(INFO_KEY))
        {
            _playerData = JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(INFO_KEY));
            LoadDailyGoals();
        }
        else
        {
            _playerData = new PlayerData();
            _playerData.dailyGoals = new List<DailyGoaldProgress>();
            _dailyTaskManager.CreateTask();
            _playerData.dayDailyGoals = DateTime.Now.Day;
            SaveInfo();
        }
        _playerData.dayDailyGoals = DateTime.Now.Day;
        SaveInfo();
    }
    public void SaveNewDailyGoals(int index)
    {
        DailyGoaldProgress goals = new DailyGoaldProgress();
        goals.numberTask = index;
        _playerData.dailyGoals.Add(goals);
        SaveInfo();
    }
    public void SaveProgressDailyGoals(int numberTask, int value)
    {
        for (int i = 0; i < _playerData.dailyGoals.Count; i++)
        {
            if (_playerData.dailyGoals[i].numberTask == numberTask)
            {
                _playerData.dailyGoals[i].progress = value;
                
            }
        }
        SaveInfo();
    }
    private void LoadDailyGoals()
    {
        if (_playerData.dayDailyGoals != DateTime.Now.Day)
        {
            _playerData.dailyGoals.Clear();
            _dailyTaskManager.CreateTask();
        }
        else
        {
            _dailyTaskManager.LoadTaskOnDay(_playerData.dailyGoals);
        }
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(MAP_KEY);
    }

    public void AddDayToDailyReward()
    {
        _playerData.numberDayInReward++;
        _playerData.lastVisitData = DateTime.Today.Day;



        SaveInfo();
    }
    private void SaveInfo()
    {
        string jsonSave = JsonUtility.ToJson(_playerData);
        Debug.Log(jsonSave);
        PlayerPrefs.SetString(INFO_KEY, jsonSave);
    }
    public CellsSaweData LoadInfo()
    {

        LoadData();
        return JsonUtility.FromJson<CellsSaweData>(PlayerPrefs.GetString(MAP_KEY));
    }

    public void LoadData()
    {
        gamePlayerData = JsonUtility.FromJson<GamePlayerData>(PlayerPrefs.GetString(DATA_KEY));

        Debug.Log(JsonUtility.ToJson(gamePlayerData));

        GameData.isCouplesShowedUp = gamePlayerData.sawStatus;

        BoosterLogic.Instance.sawBooster.Enabled = gamePlayerData.sawStatus;
        BoosterLogic.Instance.timeBooster.Enabled  = gamePlayerData.timerStatus;

        BoosterLogic.Instance.sawBooster.Count = gamePlayerData.sawCount;
        BoosterLogic.Instance.timeBooster.Count = gamePlayerData.timerCount;
        
        BoosterLogic.Instance.sawBooster.UpdateGraphic();
        BoosterLogic.Instance.timeBooster.UpdateGraphic();

        GameManager.Instance.gameData.score = gamePlayerData.score;

        CellLogic.Instance.scoreTarget = gamePlayerData.score;

        //GameManager.Instance.UpdateStatsUI();
        StartCoroutine(Delay());

        IEnumerator Delay()
        {
            yield return null;
            CellLogic.Instance.timer.value = gamePlayerData.timer;
            CellLogic.Instance.timer.delay = gamePlayerData.timerDelay;
        }
    }

    private void OnApplicationQuit()
    {
        BlocksSkinControllerNew.Instance._isOpenedPackage = false;
        SaveData();
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
            SaveData();
    }

    public void SaveData()
    {
        CellsSaweData cellSaweData = new CellsSaweData();

        CellContainer[,] allCells = CellLogic.Instance.GetAllCells;

        cellSaweData.CreateData(allCells.GetLength(1));

        for (int j = 0; j < allCells.GetLength(1); j++)
        {
            for (int i = 0; i < allCells.GetLength(0); i++)
            {

                if (allCells[i, j].cell)
                {
                    var myCell = allCells[i, j];

                    bool couple = myCell.cell.couple != null;

                    List<CellCouple.LinkData> links = null;
                    Vector2Int[] couplePos = null;
                    if (couple)
                    {
                        links = allCells[i, j].cell.couple.links;
                        couplePos = new Vector2Int[links.Count];

                        for (int c = 0; c < couplePos.Length; c++)
                        {
                            if (links[c].a == myCell.cell)
                                couplePos[c] = links[c].b.cellPosition.GridPos;
                            else
                                couplePos[c] = links[c].a.cellPosition.GridPos;
                        }
                    }

                    int atachedType = 0;

                    if (myCell.cell.wheelAttached != null)
                    {
                        if (myCell.cell.wheelAttached.isMoneyCell)
                            atachedType = 1;
                    }
                    if (myCell.cell.wheelAttached != null)
                    {
                        if (!myCell.cell.wheelAttached.isMoneyCell)
                            atachedType = 2;
                    }
                    else if (myCell.cell.keyAttached != null)
                    {
                        atachedType = 3;
                    }

                    cellSaweData.AddCellToData(
                        new Data() { 
                            position = myCell.GridPos, 
                            data = myCell.cell.data, 
                            haveCouple = couple, 
                            ñouplePosicion = couple ? couplePos : null,
                            typeAttachedCell = atachedType
                        }
                        );
                }
                else
                {
                    cellSaweData.AddCellToData(new Data() { position = allCells[i, j].GridPos, data = -1 });
                }
            }

            cellSaweData.NewLine();
        }

        //Debug.LogError("----test6513-- "+JsonUtility.ToJson(cellSaweData)); bug
       
            PlayerPrefs.SetString(MAP_KEY, JsonUtility.ToJson(cellSaweData));

        gamePlayerData = new GamePlayerData();
        gamePlayerData.sawStatus = BoosterLogic.Instance.sawBooster.Enabled;
        gamePlayerData.timerStatus = BoosterLogic.Instance.timeBooster.Enabled;

        gamePlayerData.sawCount = BoosterLogic.Instance.sawBooster.Count;
        gamePlayerData.timerCount = BoosterLogic.Instance.timeBooster.Count;

        gamePlayerData.timer = CellLogic.Instance.timer.value;
        gamePlayerData.timerDelay = CellLogic.Instance.timer.delay;

        gamePlayerData.score = GameManager.Instance.gameData.score;
       // gamePlayerData.score = CellLogic.Instance.scoreTarget;

        Debug.Log(JsonUtility.ToJson(gamePlayerData));
        PlayerPrefs.SetString(DATA_KEY, JsonUtility.ToJson(gamePlayerData));
        // PlayerPrefs.DeleteAll();
       
    }
}

[Serializable]
public class PlayerData
{
    public int dayDailyGoals;
    public List<DailyGoaldProgress> dailyGoals;

    public int numberDayInReward;
    public int lastVisitData;
}

[Serializable]
public class PlayerSaveData
{
    public int countCoin;
    public int numberDayInReward;
    public int lastVisitData;
}

[Serializable]
public class GamePlayerData
{
    public bool sawStatus;
    public bool timerStatus;

    public int sawCount;
    public int timerCount;

    public float timer;
    public float timerDelay;

    public int score;
}

[Serializable]
public class CellsSaweData
{
    private int lineId=0;

    private List<Data> dataList;
    public LineData[] data;

    public void CreateData(int j)
    {
        dataList = new List<Data>();
        data = new LineData[j];

        for (int i = 0; i < j; i++)
        {
            data[i] = new LineData();
        }
    }

    public void AddCellToData(Data _data )
    {
        dataList.Add(_data);
    }

    public void NewLine()
    {
        data[lineId].data = dataList.ToArray();

        lineId++;

        dataList = new List<Data>();
    }

    public Data[] GetLineData(int lineID)
    {
        bool lineIsEmpty = true;
        foreach (Data d in data[lineID].data) { if (d.data != -1) lineIsEmpty = false; }

        if (lineIsEmpty)
            return null;
        else
            return data[lineID].data;
    }
}
[Serializable]
public class LineData
{
    public Data[] data;
}
[Serializable]
public class Data
{
    public Vector2Int position;
    public int data;

    public bool haveCouple = false;
    public Vector2Int[] ñouplePosicion;
    /// <summary>
    ///0 is null,
    ///1 is money attached,
    ///2 is wheel attached,
    ///3 is key attached
    /// </summary>
    public int typeAttachedCell = 0;
}

[Serializable]
public class DailyGoaldProgress
{
    public int numberTask;
    public int progress;
}