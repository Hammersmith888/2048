using UnityEngine;
using System;

[Serializable]
public class CellContainer : MonoBehaviour
{
    public Vector2Int GridPos;

    public Cell cell;

    [SerializeField] private BlocksSkinControllerNew _blocksSkinControllerNew;

    public RectTransform rectTransform
    {
        get
        {
            if (_rectTranform == null)
                _rectTranform = GetComponent<RectTransform>();
            return _rectTranform;
        }
    }

    private RectTransform _rectTranform;

    private void Awake()
    {
        _rectTranform = GetComponent<RectTransform>();
    }
}
