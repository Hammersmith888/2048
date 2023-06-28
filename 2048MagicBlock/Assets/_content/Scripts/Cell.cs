using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class WheelCellAttached
{
    public bool isDestroyed;
    public bool isMoneyCell = false;
    public int moneyInCell = 20;
    public float timer;

    public GameObject wheelInstance;


    public void Update()
    {
        /*if (!isMoneyCell)
        {
            timer += Time.deltaTime;
            if (timer > CellLogic.Instance.balance.wheelAttachedSec)
            {
                timer = 0f;
                isDestroyed = true;
                GameObject.Destroy(wheelInstance);
            }
        }*/
    }
}

public class Cell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int data {
        get => _data;
        set {
            _data = value;
            if (_data == 0 || _data == 25)
                Destroy(gameObject);
            else
                UpdateGraphic();
        }
    }
    public int _data;
    [HideInInspector]
    public Image image;
    public Image image2;

    public CellCouple couple;

    public Rigidbody2D rb;
    public BoxCollider2D boxCollider2D;

    public WheelCellAttached wheelAttached { get; private set; }
    public GameObject keyAttached;

    public CellContainer cellPosition { 
        get => _cellPosition;
        set {
            if (_cellPosition != null && _cellPosition != value && _cellPosition.cell == this)
            {
                _cellPosition.cell = null;
            }

            _cellPosition = value;
            if (_cellPosition)
                _cellPosition.cell = this;
            
        }
    }
    private CellContainer _cellPosition;

    public PhysicsMaterial2D material;

    public bool IsDragging => isDragging;
    private bool isDragging;

    private PolygonCollider2D _collider;

    public RectTransform rectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();

            return _rectTransform;
        }
    }

    private RectTransform _rectTransform;

    private void Awake()
    {
        //material = Resources.Load<PhysicsMaterial2D>("Cell2D");

        image = GetComponent<Image>();
        _collider = GetComponent<PolygonCollider2D>();
        _rectTransform = GetComponent<RectTransform>();

        BlocksSkinController.Instance.OnSkinSelected += (int id) => {
            if (this == null) return;
            if (gameObject == null) return;
            if (image == null) return;

            image.sprite = BlocksSkinController.Instance.GetSpriteByValue(data, out Vector3 scale);
            image.rectTransform.localScale = scale;
        };

        boxCollider2D = gameObject.GetComponentInChildren<BoxCollider2D>();

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    private void UpdateGraphic()
    {
        if(image == null)
            image = GetComponent<Image>();

        image.sprite = CellLogic.Instance.GetSpriteByValue(data,out Vector3 scale);
        //image.color = image.sprite != null ? Color.white : Color.clear;
        image.rectTransform.localScale = scale;
    }

    #region INTERFACE REALIZATION

    private static Cell cellDragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cellDragging != null && cellDragging != this) return;

        if (GameData.isTutorialShowedUp)
            if (CellLogic.Instance.IsMovingGrid ||
                GameManager.Instance.isPause ||
                GameManager.Instance.isGameOverScreen)
                return;

        if (!TutorialLogic.Instance.CanDragCell(this))
            return;

        if (couple != null)
        {
            couple.currentDrag = this;
            couple.OnBeginDrag();
        }

        StopAllCoroutines();
        transform.DOKill();

        isDragging = true;

        cellDragging = this;

        SetTriggerState(true);

        transform.SetAsLastSibling();

        CellLogic.Instance.isDragging = true;



        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }






    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 curScreenPoint;

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag Start Script");
        if (!isDragging) return;

        Vector3 target = Camera.main.ScreenToWorldPoint(eventData.position);
        target.z = 0;

        if (couple != null)
            couple.OnDrag(target - cellPosition.transform.position);
        else
        {
            //target = CellLogic.Instance.ClampCellMoving(this, target);
            //rb.MovePosition(target);
            //rb.position = target;
            //rb.MovePosition(target);
            //rb.velocity = new Vector3(Mathf.Clamp01(target.x), Mathf.Clamp01(target.y), Mathf.Clamp01(target.z));
            //rectTransform.anchoredPosition += eventData.delta;
        }
        Debug.Log("OnDrag Finish Script");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            Destroy(rb);
            return;
        }

        if (couple != null)
            couple.OnEndDrag();

        if (!GameData.isTutorialShowedUp)
        {
            if (TutorialLogic.Instance.IsDestinationCell(cellPosition.GridPos))
                TutorialLogic.Instance.NextTutorialStep();
            else if(!TutorialLogic.Instance.IsDestinationCellX(cellPosition.GridPos.x))
                cellPosition = CellLogic.Instance.GetCellPosition(TutorialLogic.Instance.Tutorial.sourceMoveCell);
        }

        isDragging = false;

        cellDragging = null;

        ResetPosition();

        SetTriggerState(false);
        CellLogic.Instance.isDragging = false;
    }

    #endregion

    public void SetTriggerState(bool active)
    {
        //_collider.enabled = active;

        if (active)
        {
            if(rb == null)
                rb = gameObject.AddComponent<Rigidbody2D>();
            //rb.isKinematic = true;
            rb.freezeRotation = true;
            rb.gravityScale = 0;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.sharedMaterial = material;
        }
        else
        {
            if(rb != null)
                Destroy(rb);
        }
    }
    public void CellAddRigidBody2D()
    {
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        //rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        rb.freezeRotation = true;
        rb.gravityScale = 0;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.sharedMaterial = material;
    }

    public void MoveRelativeInitPos(Vector3 delta)
    {
        //rb.MovePosition(cellPosition.transform.position + delta);
        transform.position = cellPosition.transform.position + delta;
    }

    public Vector3 ClampMovingDelta(Vector3 deltaIn)
    {
        var target = CellLogic.Instance.ClampCellMoving(this, cellPosition.transform.position + deltaIn);
        return target - cellPosition.transform.position;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(couple != null && couple.currentDrag != null && couple.currentDrag != this)
        {
            return;
        }

        var cellContainer = collision.GetComponent<CellContainer>();
        if (cellContainer != null && cellContainer != cellPosition &&
            Vector2.Distance(cellContainer.transform.position, transform.position)
            < CellLogic.Instance.CellsStep.y / 2f)
        {
            var cell = CellLogic.Instance.GetCell(cellContainer.GridPos);

            if (cell == null || (couple != null && cell.couple == couple))
            {
                cellPosition = cellContainer;

                if (couple != null)
                {
                    couple.OnSwap();
                }
            }

            if (
                cell != null 
                && cell.data == data 
                && (couple == null || (couple != null && cell.couple != couple)))
            {
                if (couple != null)
                {
                    couple.OnEndDrag();
                }

                if (!GameData.isTutorialShowedUp)
                {
                    if(!TutorialLogic.Instance.IsDestinationCell(cellContainer.GridPos.x, cellContainer.GridPos.y))
                    {
                        return;
                    }
                }

                CellLogic.Instance.CombineCells(this, cell);
                cell.ResetPosition();
                CellLogic.Instance.isDragging = false;
                isDragging = false;

                if (!GameData.isTutorialShowedUp)
                    TutorialLogic.Instance.NextTutorialStep();
            }
        }
    }
    public Cell lastCellIntersect;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var cell = collision.gameObject.GetComponent<Cell>();
        var collider = collision.gameObject.GetComponent<PolygonCollider2D>();
        //Debug.LogError("--------------------------------------------------");
        if (cell != null
            && _data == cell._data 
            && !cell.IsDragging 
            && collider != null
            && collider.isTrigger == false)
        {
            collider.isTrigger = true;
        }

        if (cell != null 
            && cell.IsDragging
            && cell.data == data)
        {
            lastCellIntersect = cell;
        }
        else if (cell != null
            && cell.data == data)
        {
            lastCellIntersect = cell;
        }
        if (collision.collider != null)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = -collision.relativeVelocity * 0.5f;
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        var cell = collision.gameObject.GetComponent<Cell>();
        var collider = collision.gameObject.GetComponent<PolygonCollider2D>();

        if (cell != null
            && _data == cell._data
            && !cell.IsDragging
            && collider != null
            && collider.isTrigger == false)
        {
            collider.isTrigger = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PolygonCollider2D>() != null)
            collision.gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PolygonCollider2D>() != null)
            collision.gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
    }

    public void ResetPositionImmediate()
    {
        transform.position = cellPosition.transform.position;
    }

    public void ResetPosition(float mult = 1.05f)
    {
        if (gameObject != null && gameObject.activeSelf)
            StartCoroutine(ResetPositionRoutine(mult));
        else
            transform.position = cellPosition.transform.position;
    }

    Vector3 velocity;
    private IEnumerator ResetPositionRoutine(float mult)
    {
        var distance = Vector2.Distance(transform.position, cellPosition.transform.position);
        while (distance > 0.1 && cellPosition != null)
        {
            Debug.Log("Iteriction");
            distance = Vector2.Distance(transform.position, cellPosition.transform.position);

            velocity = Vector2.Lerp(velocity, (cellPosition.transform.position - transform.position) * 0.7f, Time.deltaTime * 20);

            transform.position = transform.position + Vector3.ClampMagnitude(velocity, distance /** mult*/);

            if (couple != null)
                couple.UpdateLinksPosition();

            //  yield return null;
            yield return new WaitForFixedUpdate();
        }
        if(cellPosition != null)
            transform.position = cellPosition.transform.position;
    }

    private float timer = 0;
    private const float delay = 0.1f;

    private void OnDestroy()
    {
        CellLogic.Instance.OnCellDestroy(this);
    }

    private Tween tween;

    public void DeatachWheel()
    {
        wheelAttached.isDestroyed = true;
        Destroy(wheelAttached.wheelInstance);
        wheelAttached = null;
    }

    public WheelCellAttached AttachWheel(bool isMoneyCell = false)
    {
        wheelAttached = new WheelCellAttached();

        if (isMoneyCell)
            wheelAttached.isMoneyCell = true;

        if (isMoneyCell)
            wheelAttached.wheelInstance = Instantiate(CellLogic.Instance.moneyCellAttachPrefab, transform);
        else
            wheelAttached.wheelInstance = Instantiate(CellLogic.Instance.wheelCellAttachPrefab, transform);

        wheelAttached.wheelInstance.transform.position = transform.position;

        return wheelAttached;
    }

    public bool IsWheelAttached()
    {
        return wheelAttached != null && !wheelAttached.isDestroyed && !wheelAttached.isMoneyCell;
    }

    public bool IsMoneyCellAttached()
    {
        return wheelAttached != null && !wheelAttached.isDestroyed && wheelAttached.isMoneyCell;
    }

    private void FixedUpdate()
    {
        if (isDragging && couple == null && rb != null)
        {
            curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
            rb.velocity = (curPosition - transform.position) * 25;
        }
        if (boxCollider2D.isTrigger && lastCellIntersect != null && GameData.isTutorialShowedUp)
        {
            CellLogic.Instance.CombineCells(lastCellIntersect, this);
        }
    }

    private void Update()
    {
        if (wheelAttached != null)
        {
            if (wheelAttached.isDestroyed)
                wheelAttached = null;
            else
                wheelAttached.Update();
        }


        timer += Time.deltaTime;
        if (timer < delay)
        {
            return;
        }
        timer = 0;

        if (couple != null
            || CellLogic.Instance.IsMovingGrid 
            || isDragging
            || data == 0 
            || cellPosition.GridPos.y >= CellLogic.HEIGHT - 1)
            return;
        
        if (GameData.lockFallCells)
            return;
        

        if (tween != null && tween.IsActive())
        {
            return;   
        }
        
        var bottomPos = new Vector2Int(cellPosition.GridPos.x, cellPosition.GridPos.y + 1);
        var bottom = CellLogic.Instance.GetCellPosition(bottomPos);

        if (bottom.cell == null || bottom.cell.data == data)
        {
            StopAllCoroutines();

            transform.DOKill();

            if (bottom.cell == null)
            {
                tween = transform.DOMove(bottom.transform.position, 0.1f);
                tween.onKill += () => {
                    if (!GameData.isTutorialShowedUp && TutorialLogic.Instance.IsDestinationCell(bottom.GridPos))
                        TutorialLogic.Instance.NextTutorialStep();
                };
                cellPosition = bottom;
                CellLogic.Instance.isFieldCheckDirty = true;
            }
            else
            {
                if (!GameData.isTutorialShowedUp)
                    TutorialLogic.Instance.NextTutorialStep();
                CellLogic.Instance.CombineCells(this, bottom.cell,true);
                var initPos = bottom.cell.transform.position;
                var centerPos = Vector3.Lerp(initPos, transform.position, 0.45f);
                bottom.cell.tween = bottom.cell.transform.DOMove(centerPos, 0.1f);
                bottom.cell.transform.position = transform.position;
                bottom.cell.tween.onKill = () => { 
                    bottom.cell.tween = bottom.cell.transform.DOMove(initPos, 0.1f); 
                };
                CellLogic.Instance.isFieldCheckDirty = true;
            }
        }

        
         
    }
}