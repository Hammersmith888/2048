using UnityEngine;

public class MoveToRectTransform : MonoBehaviour
{
    [SerializeField] private RectTransform _moveFromRectTransform;
    [SerializeField] private RectTransform _moveToRectTransform; 

    void Update()
    {
        if (_moveFromRectTransform.position != _moveToRectTransform.position)
        {
            _moveFromRectTransform.position = _moveToRectTransform.position;
        }
    }
}
