using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class RandomPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _placeText, playerNameText, _scoreText;

    [SerializeField] private Image _avatar, _flags;

    [SerializeField] private List<Sprite> _avatarList;

    private void Awake()
    {
        playerNameText.text = "player_" + Random.Range(1000, 99999).ToString();
        _avatar.sprite = _avatarList[Random.Range(0, _avatarList.Count)];
    }

}
