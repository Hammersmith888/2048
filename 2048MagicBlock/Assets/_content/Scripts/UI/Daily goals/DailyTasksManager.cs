using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DailyTasksManager : MonoBehaviour
{
    [SerializeField] private Transform _tasksFrame;

    [SerializeField] private List<TaskBlock> _task;

    [SerializeField] private AnimationPanel _animationPanel;

    private int _day;

    private List<int> _index;

    #region поля для дневных задач

    private int _mergeCount, _watchAVideo, _collect1Block15InNormalMode, _spinTheWheelOfFortune, _useRevive1Time, _useTheClockOrSaw1Time;
    private bool _mergeState, _watchState, _collect1BlockState, _spinState, _useReviveState, _useTheClockOrSaw1State;

    #endregion


    private void Awake()
    {
        _index = new List<int>() { 1, 2, 3, 4, 5, 6 };

        LoadDataTime();

       // StartCoroutine(UpdateTime());

        GoalsEventManager.OnUpMerge20Times += UpMerge20Times;
        GoalsEventManager.OnUpWatchAVideoAd += UpWatchAVideoAd;
        GoalsEventManager.OnUpCollect1Block15InNormalMode += UpCollect1Block15InNormalMode;
        GoalsEventManager.OnUpSpinTheWheelOfFortune += UpSpinTheWheelOfFortune;
        GoalsEventManager.OnUpUseRevive1Time += UpUseRevive1Time;
        GoalsEventManager.OnUseTheClockOrSaw1Time += UseTheClockOrSaw1Time;
    }



    public void ShowPopupDailyGoals()
    {
        AnalyticsManager.Instance.LogEvent("goals_entry");
        PopupController.Instance.ShowPopup(_animationPanel);
    }
    public void HidePopupDailyGoals()
    {
        PopupController.Instance.HidePopup(_animationPanel);
    }

    private IEnumerator UpdateTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            var time = DateTime.UtcNow;

            if (time.Day != _day)
            {
                PlayerPrefs.SetInt("DataTime", time.Day);
                UpdateTasks();
            }
        }
    }
       
    private void LoadProgress(int numberTask,int value)
    {
        //Debug.LogError("LoadProgress on day");
        if(value==0)
        {
            return;
        }
        switch (numberTask)
        {
            case 1:
                _mergeCount = value-1;
                AnalyticsManager.Instance.LogEvent("merge_blocks" + _mergeCount);
                UpMerge20Times();
                break;
            case 2:
                _watchAVideo = value-1;
                UpWatchAVideoAd();
                break;
            case 3:
                _collect1Block15InNormalMode = value - 1;
                UpCollect1Block15InNormalMode();
                break;
            case 4:
                _spinTheWheelOfFortune = value - 1;
                UpSpinTheWheelOfFortune();
                break;
            case 5:
                _useRevive1Time = value - 1;
                UpUseRevive1Time();
                break;
            case 6:
                _useTheClockOrSaw1Time = value - 1;
                UseTheClockOrSaw1Time();
                break;
        }
    }
    private void LoadDataTime()
    {
        if (!PlayerPrefs.HasKey("DataTime"))
        {
            var time = DateTime.UtcNow;
            PlayerPrefs.SetInt("DataTime", time.Day);
            _day = time.Day;
        }
        else
        {
            _day = PlayerPrefs.GetInt("DataTime");
        }
    }

    public void CreateTask()
    {
        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, _index.Count);
            Saver.Instance.SaveNewDailyGoals(index);
            TaskInitialization(index + 1);

            _index.RemoveAt(index);
        }

        for (int j = 0; j < _index.Count; j++)
        {
            TaskInitializationSetActive(_index[j]);
        }
    }

    public void LoadTaskOnDay(List<DailyGoaldProgress> daysTask)
    {
        _index = new List<int>() { 1, 2, 3, 4, 5, 6 };

        for (int i = 0; i < daysTask.Count; i++)
        {
            Debug.Log(daysTask[i].numberTask);
            int index = daysTask[i].numberTask;
            TaskInitialization(index + 1);
            _index.RemoveAt(index);
        }

        for (int j = 0; j < _index.Count; j++)
        {
             TaskInitializationSetActive(_index[j]);
        }
        for (int i = 0; i < daysTask.Count; i++)
        {
            LoadProgress(daysTask[i].numberTask + 1, daysTask[i].progress);
            Debug.Log(daysTask[i].numberTask + 1);
            Debug.Log(daysTask[i].progress);
        }
        
    }
    private void TaskInitialization(int index)
    {
        switch (index)
        {
            case 1:
                _mergeState = true;

                break;
            case 2:
                _watchState = true;

                break;
            case 3:
                _collect1BlockState = true;

                break;
            case 4:
                _spinState = true;

                break;
            case 5:
                _useReviveState = true;

                break;
            case 6:
                _useTheClockOrSaw1State = true;

                break;
        }
    }
    private void TaskInitializationSetActive(int index)
    {
        switch (index)
        {
            case 1:
                _mergeState = false;

                break;
            case 2:
                _watchState = false;

                break;
            case 3:
                _collect1BlockState = false;

                break;
            case 4:
                _spinState = false;

                break;
            case 5:
                _useReviveState = false;

                break;
            case 6:
                _useTheClockOrSaw1State = false;

                break;
        }

        _task[index - 1].SetActiveGameObject();
    }


    private void UpdateTasks()
    {
        _index = new List<int>() { 1, 2, 3, 4, 5, 6 };

        _mergeState = false;
        _watchState = false;
        _collect1BlockState = false;
        _spinState = false;
        _useReviveState = false;
        _useTheClockOrSaw1State = false;

        CreateTask();
    }

    private void UpMerge20Times()
    {
        if (_mergeCount < 20 && _mergeState == true)
        {
            _mergeCount++;
            _task[0].Value(_mergeCount.ToString() + "/20");
            _task[0].FillBar((1f / 20f) * (float)_mergeCount);

        }
        if (_mergeCount == 20 && _mergeState == true)
        {
            _task[0].Complite();
        }
        Debug.Log("Save"+ _mergeCount);
        Saver.Instance.SaveProgressDailyGoals(0,_mergeCount);
    }

    private void UpWatchAVideoAd()
    {
        if (_watchAVideo < 1 && _watchState == true)
        {
            _watchAVideo++;
            _task[1].Value(_watchAVideo.ToString() + "/1");
            _task[1].FillBar(1);
        }
        if (_watchAVideo == 1 && _watchState == true)
        {
            _task[1].Complite();
        }
        Debug.Log("Save UpWatchAVideoAd");
        Saver.Instance.SaveProgressDailyGoals(1, _watchAVideo);
    }

    private void UpCollect1Block15InNormalMode()
    {
        if (_collect1Block15InNormalMode < 1 && _collect1BlockState == true)
        {
            _collect1Block15InNormalMode++;
            _task[2].Value(_collect1Block15InNormalMode.ToString() + "/1");
            _task[2].FillBar(1);
        }
        if (_collect1Block15InNormalMode == 1 && _collect1BlockState == true)
        {
            _task[2].Complite();
        }
        Debug.Log("Save UpCollect1Block15InNormalMode");
        Saver.Instance.SaveProgressDailyGoals(2, _collect1Block15InNormalMode);
    }

    private void UpSpinTheWheelOfFortune()
    {
        if (_spinTheWheelOfFortune < 1 && _spinState == true)
        {
            _spinTheWheelOfFortune++;
            _task[3].Value(_spinTheWheelOfFortune.ToString() + "/1");
            _task[3].FillBar(1);
        }
        if (_spinTheWheelOfFortune == 1 && _spinState == true)
        {
            _task[3].Complite();
        }
        Debug.Log("Save UpSpinTheWheelOfFortune");
        Saver.Instance.SaveProgressDailyGoals(3, _spinTheWheelOfFortune);
    }

    private void UpUseRevive1Time()
    {
        if (_useRevive1Time < 1 && _useReviveState == true)
        {
            _useRevive1Time++;
            _task[4].Value(_useRevive1Time.ToString() + "/1");
            _task[4].FillBar(1);
        }
        if (_useRevive1Time == 1 && _useReviveState == true)
        {
            _task[4].Complite();
        }
        Debug.Log("Save UpUseRevive1Time");
        Saver.Instance.SaveProgressDailyGoals(4, _useRevive1Time);
    }

    private void UseTheClockOrSaw1Time()
    {
        if (_useTheClockOrSaw1Time < 1 && _useTheClockOrSaw1State == true)
        {
            _useTheClockOrSaw1Time++;
            _task[5].Value(_useTheClockOrSaw1Time.ToString() + "/1");
            _task[5].FillBar(1);
        }
        if (_useTheClockOrSaw1Time == 1 && _useTheClockOrSaw1State == true)
        {
            _task[5].Complite();
        }
        Debug.Log("Save UseTheClockOrSaw1Time");
        Saver.Instance.SaveProgressDailyGoals(5, _useTheClockOrSaw1Time);
    }
}
