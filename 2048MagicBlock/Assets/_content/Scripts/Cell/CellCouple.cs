using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class CellCouple
{
    public class LinkData
    {
        public Image instance;
        public Cell a;
        public Cell b;
        public bool isVertical;

        public void UpdatePosition(Vector3 delta)
        {
            instance.transform.position = a.cellPosition.transform.position + (b.cellPosition.transform.position - a.cellPosition.transform.position) / 2 + delta;
        }
        public void UpdatePositionRealtime()
        {
            instance.transform.position = a.transform.position + (b.transform.position - a.transform.position) / 2;
        }

        public void Destroy()
        {
            Object.Destroy(instance);
        }
    }

    public bool isTweening => tween != null && tween.IsActive();

    private Tween tween;

    public void Destroy()
    {
        for (int i = links.Count - 1; i >= 0; i--)
        {
            links[i].Destroy();
        }
        links.Clear();
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].couple = null;
        }
        cells.Clear();
    }

    public void MoveDown()
    {
        if (GameData.lockFallCells)
            return;

        if (currentDrag != null || isTweening) return;

        bool canMoveDown = true;

        var positions = new Vector2Int[cells.Count];

        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            var pos = cell.cellPosition.GridPos;
            positions[i] = pos;
            var bottomPos = pos + new Vector2Int(0, 1);
            if (!CellLogic.Instance.IsPositionInsideGrid(bottomPos))
            {
                canMoveDown = false;
                break;
            }

            var bottomCell = CellLogic.Instance.GetCell(bottomPos);

            if (bottomCell == null || bottomCell.couple == this)
                continue;

            if (bottomCell != null && bottomCell.data != cell.data)
                canMoveDown = false;
        }

        if (!canMoveDown) return;

        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            cell.cellPosition = null;
        }

        for (int i = 0; i < cells.Count; i++)
        {
            var bottomPos = CellLogic.Instance.GetCellPosition(positions[i] + new Vector2Int(0, 1));
            if (bottomPos.cell == null)
            {
                cells[i].cellPosition = bottomPos;
                bottomPos.cell = cells[i];
                //cells[i].ResetPosition();

                tween = cells[i].transform.DOMove(bottomPos.transform.position, 0.1f);

                tween.onUpdate += () =>
                {
                    UpdateLinksPosition();
                };
            }

        }

        for (int i = 0; i < cells.Count; i++)
        {
            var bottomPos = CellLogic.Instance.GetCellPosition(positions[i] + new Vector2Int(0, 1));
            if (bottomPos.cell != null && bottomPos.cell != cells[i] && bottomPos.cell.data == cells[i].data)
            {
                cells[i].cellPosition = CellLogic.Instance.GetCellPosition(positions[i]);

                CellLogic.Instance.CombineCells(cells[i], bottomPos.cell);
                bottomPos.cell.ResetPosition();
            }
        }

        Validate();
    }

    public void UpdateLinksPosition()
    {
        for (int i = 0; i < links.Count; i++)
        {
            links[i].UpdatePositionRealtime();
        }
    }

    public Vector2 min;
    public Vector2 max;
    public Vector2 size => max - min;

    public List<Cell> cells = new List<Cell>();
    public List<LinkData> links = new List<LinkData>();

    public void ShowTutorial()
    {
        //GameManager.instance.priorityShowParent

        var parent = GameManager.Instance.PriorityShowParent;

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].transform.SetParent(parent);
        }

        for (int i = 0; i < links.Count; i++)
        {
            links[i].instance.transform.SetParent(parent);
        }
    }

    public void HideTutorial()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].transform.SetParent(CellLogic.Instance.CellsParent);
        }

        for (int i = 0; i < links.Count; i++)
        {
            links[i].instance.transform.SetParent(CouplesLogic.Instance.LinksParent);
        }
    }

    public Cell currentDrag;

    private Vector3 GetMaxDelta(Vector3 deltaIn)
    {
        Vector3 newDelta = deltaIn;

        for (int i = 0; i < cells.Count; i++)
        {
            var clampDeltaCell = cells[i].ClampMovingDelta(deltaIn);

            if (deltaIn.x < 0)
                newDelta.x = Mathf.Max(newDelta.x, clampDeltaCell.x);
            if (deltaIn.x > 0)
                newDelta.x = Mathf.Min(newDelta.x, clampDeltaCell.x);

            if (deltaIn.y < 0)
                newDelta.y = Mathf.Max(newDelta.y, clampDeltaCell.y);
            if (deltaIn.y > 0)
                newDelta.y = Mathf.Min(newDelta.y, clampDeltaCell.y);
        }

        return newDelta;
    }

    private Vector3[] dragDistances;
    private Vector2Int[] gridPosDeltas;

    public void OnBeginDrag()
    {

        gridPosDeltas = new Vector2Int[cells.Count];
        dragDistances = new Vector3[cells.Count];

        for (int i = 0; i < cells.Count; i++)
        {
            dragDistances[i] = cells[i].cellPosition.transform.position - currentDrag.cellPosition.transform.position;
            gridPosDeltas[i] = cells[i].cellPosition.GridPos - currentDrag.cellPosition.GridPos;
        }

        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            if (cell == currentDrag)
            {
                if (cell.rb != null)
                    cell.rb.freezeRotation = true;
                continue;
            }

            cell.transform.SetAsLastSibling();
        }

        foreach (var item in CellLogic.Instance.cells)
        {
            item.CellAddRigidBody2D();
        }
    }

    public void OnDrag(Vector3 delta)
    {
        var deltaClamped = GetMaxDelta(delta);
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i] == currentDrag)
            {
                cells[i].MoveRelativeInitPos(deltaClamped);
            }
            else
            {
                if (cells[i] != null)
                {
                    cells[i].transform.position = currentDrag.transform.position + dragDistances[i];
                }
                //cells[i].rb.position = currentDrag.transform.position + dragDistances[i];
                //cells[i].rb.MovePosition(currentDrag.transform.position + dragDistances[i]);
            }
        }

        UpdateLinksPosition();
    }

    public void OnEndDrag()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            cell.ResetPosition();

            if (cell == currentDrag)
                continue;
        }
        currentDrag = null;

        foreach (var item in CellLogic.Instance.cells)
        {
            item.SetTriggerState(false);
        }
    }

    private bool isConnected(Cell a, Cell b)
    {
        if (a == null || b == null)
        { Debug.LogError("ERROR"); return false; }

        var gridPos1 = a.cellPosition.GridPos;
        var gridPos2 = b.cellPosition.GridPos;

        return
            (gridPos1.x == gridPos2.x && Mathf.Abs(gridPos1.y - gridPos2.y) == 1)
            ||
            (gridPos1.y == gridPos2.y && Mathf.Abs(gridPos1.x - gridPos2.x) == 1);
    }

    public void Init()
    {
        for (int i = 0; i < links.Count; i++)
        {
            Object.Destroy(links[i].instance);
        }
        links.Clear();

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].couple = this;
        }

        List<Cell> skipCells = new List<Cell>();

        for (int i = 0; i < cells.Count; i++)
        {
            for (int j = 0; j < cells.Count; j++)
            {
                if (i == j)
                    continue;
                if (skipCells.Contains(cells[j]))
                    continue;
                if (!isConnected(cells[i], cells[j]))
                    continue;

                var link = new LinkData()
                {
                    instance = CouplesLogic.Instance.MakeLink(cells[i], cells[j],
                    out bool isHorizontal),
                    a = cells[i],
                    b = cells[j],
                    isVertical = !isHorizontal
                };
                link.UpdatePosition(Vector3.zero);

                links.Add(link);

            }
            skipCells.Add(cells[i]);
        }

        UpdateBounds();
    }

    public void OnSwap()
    {
        //var positions = new Vector2Int[cells.Count];

        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i] == currentDrag)
                continue;

            //positions[i] = cells[i].cellPosition.GridPos; 
            cells[i].cellPosition = null;
        }

        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];

            if (cell == currentDrag)
                continue;

            CellContainer newCellPos;

            try
            {
                newCellPos = CellLogic.Instance.GetCellPosition(currentDrag.cellPosition.GridPos + gridPosDeltas[i]);
            }
            catch (System.Exception)
            {
                foreach (var item in CellLogic.Instance.cells)
                {
                    //if (currentDrag.Equals(item))
                    //continue;
                    item.SetTriggerState(false);
                }
                throw;
            }

            if (newCellPos.cell != null && cell.data == newCellPos.cell.data && cell.name != newCellPos.cell.name)
            {
                var cellTemp = cell;
                cell.cellPosition = newCellPos;
                //cell.boxCollider2D.isTrigger = false;
                //cell.GetComponent<PolygonCollider2D>().isTrigger = false;
                //newCellPos.cell.GetComponent<Animator>().Play("CombineAnim");


                //Debug.LogError("---cell "+ cell.name);
                //Debug.LogError("---cell.lastCellIntersect.name" + cell.lastCellIntersect.name);
                //Debug.LogError("---newCellPos.cell " + newCellPos.cell.name);

                foreach (var item in CellLogic.Instance.cells)
                {
                    //if (currentDrag.Equals(item))
                        //continue;
                    item.SetTriggerState(false);
                }

                currentDrag.ResetPosition();
                //OnEndDrag();

                CellLogic.Instance.CombineCells(cell, newCellPos.cell, true, cellTemp);
            }
            else
            {
                cell.cellPosition = newCellPos;
            }
        }
    }

    private void UpdateBounds()
    {
        if (cells.Count < 2)
            return;

        min = cells[0].cellPosition.GridPos;
        max = cells[0].cellPosition.GridPos;

        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            var gridPos = cell.cellPosition.GridPos;
            min.x = Mathf.Min(min.x, gridPos.x);
            min.y = Mathf.Min(min.y, gridPos.y);
            max.x = Mathf.Max(max.x, gridPos.x);
            max.y = Mathf.Max(max.y, gridPos.y);
        }
    }

    public void Deatach(Cell cell)
    {
        for (int i = links.Count - 1; i >= 0; i--)
        {
            var link = links[i];
            if (link.a == cell || link.b == cell)
            {
                link.Destroy();
                links.RemoveAt(i);
            }
        }

        cell.couple = null;
        cell.ResetPosition();
        cells.Remove(cell);

        Validate();
    }

    public void Attach(Cell cell)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (isConnected(cell, cells[i]))
            {
                var link = new LinkData()
                {
                    instance = CouplesLogic.Instance.MakeLink(cell, cells[i], out bool isHorizontal),
                    a = cell,
                    b = cells[i],
                    isVertical = !isHorizontal
                };
                link.UpdatePosition(Vector3.zero);

                links.Add(link);
            }
        }
        cell.couple = this;
        cells.Add(cell);
        UpdateBounds();
    }

    public void Combine(CellCouple other)
    {
        var newCells = new List<Cell>(cells);

        for (int i = 0; i < other.cells.Count; i++)
        {
            var cell = other.cells[i];
            newCells.Add(cell);
            cell.couple.Deatach(cell);
            cell.couple = this;
        }

        Init();
    }

    public bool IsValid => cells.Count > 1;

    public void Validate()
    {
        if (cells.Count == 1)
        {
            Deatach(cells[0]);
            return;
        }

        var toRemove = new List<Cell>();

        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i] == null)
            {
                toRemove.Add(cells[i]);
                continue;
            }

            var connected = false;
            for (int j = 0; j < cells.Count; j++)
            {
                if (i == j)
                    continue;

                if (cells[j] == null)
                    continue;

                if (isConnected(cells[i], cells[j]))
                {
                    connected = true;
                    break;
                }
            }

            if (!connected)
                toRemove.Add(cells[i]);
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            Deatach(toRemove[i]);
        }
    }
}