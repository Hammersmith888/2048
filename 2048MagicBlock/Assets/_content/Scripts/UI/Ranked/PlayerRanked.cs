using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerRanked : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _placeText, _playerNameText, _scoreText;
    [SerializeField] private Text _place;

    [SerializeField] private Image _avatar, _flags;

    public void UpdateName(string playerName)
    {
        _playerNameText.text = playerName;
    }

    public void UpdatePlaceText(string place)
    {
        if (_placeText != null)
            _placeText.text = place;
        if (_place != null)
            _place.text = place;
    }

    public void UpdateScoreText(string score)
    {
        _scoreText.text = score;
    }

    public void UpdateAvatar(Sprite avatar)
    {
        _avatar.sprite = avatar;
    }

    public void UpdateFlags(Sprite flags)
    {
        _flags.sprite = flags;
    }
}
