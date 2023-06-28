using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BlickMove : MonoBehaviour
{
    [SerializeField] private GameObject _blick;

    [SerializeField] private Transform _startPos, _endPos;

    [SerializeField] private float _speedBlick;
    [SerializeField] private float _delayStartMove;

    private void Start()
    {
        StartMove();
    }
    private void StartMove()
    {
        _blick.transform.DOMove(_startPos.position, 0);
        _blick.transform.DOMove(_endPos.position, _speedBlick).SetDelay(_delayStartMove).onComplete += () => { { StartMove(); }; };
    }
}
