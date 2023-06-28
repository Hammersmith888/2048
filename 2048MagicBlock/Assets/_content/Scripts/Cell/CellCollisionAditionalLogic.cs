using UnityEngine;

public class CellCollisionAditionalLogic : MonoBehaviour
{
    public Cell _cellParent;
    public RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var cell = collision.gameObject.GetComponent<Cell>();
        var collider = collision.gameObject.GetComponent<PolygonCollider2D>();
        //Debug.LogError("--------------------------5555------------------------");
       
        if (cell != null
            && cell.IsDragging
            && cell.data == _cellParent.data)
        {
            _cellParent.lastCellIntersect = cell;
            Debug.LogError("--------------------------5555------------------------");
        }
        else if (cell != null
            && cell.data == _cellParent.data)
        {
            _cellParent.lastCellIntersect = cell;
            Debug.LogError("--------------------------5555------------------------");
        }
    }

    private void Update()
    {
        _rectTransform.localPosition = Vector3.zero;
    }

}
