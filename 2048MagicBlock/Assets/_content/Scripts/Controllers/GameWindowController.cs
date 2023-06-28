using System.Collections.Generic;
using UnityEngine;

public class GameWindowController : MonoBehaviour
{
    [SerializeField] private GameObject _loadWindow;

    [SerializeField] private List<GameObject> _window;

    private void Awake()
    {
        for (int i = 0; i < _window.Count; i++)
        {
            _window[i].SetActive(false);
        }

        _loadWindow.SetActive(true);
    }
}
