using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnableEnableObject : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;

    private void OnEnable()
    {
        _gameObject.SetActive(true);
    }
}
