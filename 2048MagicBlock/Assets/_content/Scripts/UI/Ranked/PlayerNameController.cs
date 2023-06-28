using UnityEngine;
using System.Collections.Generic;

public class PlayerNameController : MonoBehaviour
{
    private string _placeText, _scoreText;

    private Sprite _avatar, _flags;

    [SerializeField] private List<Sprite> _avatarList;
    [SerializeField] private List<Sprite> _flagList;

    [SerializeField] private List<PlayerRanked> _playerList;

    private string _name;

    private TouchScreenKeyboard _keyboard;

    private void Start()
    {
        LoadName();
        _placeText = Random.Range(200, 300).ToString();
        _avatar = _avatarList[Random.Range(0, _avatarList.Count)];
        _flags = _flagList[Random.Range(0, _avatarList.Count)];

        _scoreText = GameData.highScore.ToString();

        AppointAllPlace(_placeText);
        AppointAllScore(_scoreText);
        AppointAllFlags(_flags);
        AppointAllAvatar(_avatar);
    }

#if !UNITY_EDITOR
    //private bool _state;
    //[System.Obsolete]
    //private void FixedUpdate()
    //{
    //    if (_keyboard.done)
    //    {
    //        if (_state == true)
    //        {
    //            _name = _keyboard.text.ToString();
    //            SaveName();
    //            AppointAllName(_name);
    //            _state = false;
    //        }
    //    }
    //}
#endif
    private void SaveName()
    {
        PlayerPrefs.SetString("PlayerName", _name);
    }

    private void LoadName()
    {
        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            PlayerPrefs.SetString("PlayerName", "Player");
            _name = "Player";
        }
        else
        {
            _name = PlayerPrefs.GetString("PlayerName");
        }

        AppointAllName(_name);
    }

    public void NameChange()
    {
        _keyboard = TouchScreenKeyboard.Open("Enter your name ");
        // _state = true;
    }

    private string NameCorrection(string name)
    {
        string result = null;

        if (name.Length > 10)
            result = name.Substring(0, 9) + "...";
        else
            result = name;

        return result;
    }

    #region Обновление данных
    private void AppointAllName(string name)
    {
        for (int i = 0; i < _playerList.Count; i++)
            _playerList[i].UpdateName(NameCorrection(name));
    }

    private void AppointAllPlace(string place)
    {
        for (int i = 0; i < _playerList.Count; i++)
            _playerList[i].UpdatePlaceText(place);
    }

    private void AppointAllScore(string score)
    {
        for (int i = 0; i < _playerList.Count; i++)
            _playerList[i].UpdateScoreText(score);
    }

    private void AppointAllFlags(Sprite flag)
    {
        for (int i = 0; i < _playerList.Count; i++)
            _playerList[i].UpdateFlags(flag);
    }

    private void AppointAllAvatar(Sprite avatar)
    {
        for (int i = 0; i < _playerList.Count; i++)
            _playerList[i].UpdateAvatar(avatar);
    }
    #endregion
}
