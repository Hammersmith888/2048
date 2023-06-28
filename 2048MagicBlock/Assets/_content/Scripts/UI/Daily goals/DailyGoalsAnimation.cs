using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyGoalsAnimation : MonoBehaviour
{
    [SerializeField] private List<TaskBlock> _tasks;

    [SerializeField] private Transform _transform;

    [SerializeField] private Image _fill;

    [SerializeField] private CanvasGroup _chest1, _chest2, _chest3;

    [SerializeField] private List<RotationChestAnimation> _chests;
    [SerializeField] private CollectAnimationChest _collectAnimationChest;

    private RotationChestAnimation _currentOpened;

    private int _count;
    public int IndexSelectedChest => _indexSelectedChest;
    private int _indexSelectedChest;




    private int[] dailyMissions = new int[3];
    private string dateKey = "mission_date";
    private string progressKey = "mission_progress";

    private void LoadProgress()
    {
        string currentDate = DateTime.Today.ToString("yyyy-MM-dd");

        if (PlayerPrefs.HasKey(dateKey))
        {
            string savedDate = PlayerPrefs.GetString(dateKey);

            if (savedDate == currentDate)
            {
                string progressString = PlayerPrefs.GetString(progressKey);
                string[] progressArray = progressString.Split(',');

                for (int i = 0; i < dailyMissions.Length; i++)
                {
                    dailyMissions[i] = int.Parse(progressArray[i]);
                }
            }
            else
            {
                ResetProgress();
            }
        }
        else
        {
            ResetProgress();
        }
    }

    private void ResetProgress()
    {
        PlayerPrefs.SetString(dateKey, DateTime.Today.ToString("yyyy-MM-dd"));
        string progressString = string.Join(",", dailyMissions);
        PlayerPrefs.SetString(progressKey, progressString);
    }

    public void CollectReward(int missionIndex)
    {
        dailyMissions[missionIndex] = 1;
        string progressString = string.Join(",", dailyMissions);
        PlayerPrefs.SetString(progressKey, progressString);
    }


    private void Awake()
    {
        LoadProgress();
    }


    private void Start()
    {
        _count = 0;
        _fill.fillAmount = 0;

       /* _chest1.alpha = 0;
        _chest2.alpha = 0;
        _chest3.alpha = 0;*/
        foreach(var chest in _chests)
        {
            chest.GetComponent<Chest>().SetCollectAnimationChest(_collectAnimationChest);
        }

    }
    public void StopAnimationChest()
    {
        if(_currentOpened != null)
            _currentOpened.inRotationMode = false;
    }
    public void UpLine()
    {
        _count++;

        switch (_count)
        {
            case 0:
                UpdateFilling(0);
                break;
            case 1:
                _chest1.DOFade(1, 0.9f);
                if (dailyMissions[0] == 1)
                {
                    _chests[0].StartRotation(false);
                    _chests[0].GetComponent<Chest>().ChangeStateToOpened(false);
                    _chests[0].GetComponent<Chest>().SetIsOpenedChest(true);
                    _chests[0].GetComponent<Chest>().ChangeSprite();
                    UpdateFilling(0.425f,true);
                }
                else
                {
                    _chests[0].StartRotation(true);
                    _chests[0].GetComponent<Chest>().ChangeStateToOpened(true);
                    UpdateFilling(0.425f);
                }

                AnalyticsManager.Instance.LogEvent("goals_done" + 1);
                AnalyticsManager.Instance.LogEvent("goals_reward");
                break;
            case 2:
                _chest2.DOFade(1, 0.9f);

                if (dailyMissions[1] == 1)
                {
                    _chests[1].StartRotation(false);
                    _chests[1].GetComponent<Chest>().ChangeStateToOpened(false);
                    _chests[1].GetComponent<Chest>().SetIsOpenedChest(true);
                    _chests[1].GetComponent<Chest>().ChangeSprite();
                    UpdateFilling(0.7f, true);
                }
                else
                {
                    _chests[1].StartRotation(true);
                    _chests[1].GetComponent<Chest>().ChangeStateToOpened(true);
                    UpdateFilling(0.7f);
                }

                AnalyticsManager.Instance.LogEvent("goals_done" + 2);
                AnalyticsManager.Instance.LogEvent("goals_reward");
                break;
            case 3:
                _chest3.DOFade(1, 0.9f);
                if (dailyMissions[2] == 1)
                {
                    _chests[2].StartRotation(false);
                    _chests[2].GetComponent<Chest>().ChangeStateToOpened(false);
                    _chests[2].GetComponent<Chest>().SetIsOpenedChest(true);
                    _chests[2].GetComponent<Chest>().ChangeSprite();
                    UpdateFilling(1f, true);
                }
                else
                {
                    _chests[2].StartRotation(true);
                    _chests[2].GetComponent<Chest>().ChangeStateToOpened(true);
                    UpdateFilling(1f);
                }
                
                AnalyticsManager.Instance.LogEvent("goals_done" + 3);
                AnalyticsManager.Instance.LogEvent("goals_reward");
                break;
        }
    }

    public void SetCurrentOpenedChest(int index)
    {
        if (_chests.Count > index)
        {
            _currentOpened = _chests[index];
            _indexSelectedChest = index;
        }
    }
    public void StartCheckTasks()
    {
        StartCoroutine(CheckTasks());
    }

    private IEnumerator CheckTasks()
    {
        bool isCollect1Mission = (
            dailyMissions[0] == 1 || dailyMissions[1] == 1 || dailyMissions[2] == 1
            );

        if (isCollect1Mission)
            yield return new WaitForSeconds(0f);
        else
            yield return new WaitForSeconds(1f);

        for (int i = 0; i < _tasks.Count; i++)
        {
            if(isCollect1Mission)
                _tasks[i].StartAnimations(_transform,0);
            else
                _tasks[i].StartAnimations(_transform,0.7f);
        }
    }

    private void UpdateFilling(float fillCount, bool imediate = false)
    {
        StartCoroutine(Filling(fillCount, imediate)); //change to dotween
    }

    private IEnumerator Filling(float fillCount, bool imediate)
    {
        while (true)
        {
            if(imediate)
                yield return new WaitForSeconds(0);
            else
                yield return new WaitForSeconds(0.01f);

            if (imediate)
            {
                _fill.fillAmount = fillCount;
                _fill.fillAmount += 0.0009f;
            }
            else
                _fill.fillAmount += 0.005f;

            if (fillCount <= _fill.fillAmount)
            {
                break;
            }
        }

    }
}
