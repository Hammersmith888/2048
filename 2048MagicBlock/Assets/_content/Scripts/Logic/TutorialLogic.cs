using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

class CellData
{
    public Vector2Int position;
    public int data;
}

public class TutorialLogic : MonoBehaviourSingleton<TutorialLogic>
{
    private static int WIDTH => CellLogic.WIDTH;
    private static int HEIGHT => CellLogic.HEIGHT;

    private CellLogic cellLogic => CellLogic.Instance;

    public TutorialData Tutorial { get => tutorial; }

    [SerializeField]
    private TutorialData tutorial;

    private bool progressSaved;
    private List<CellData> cells = new List<CellData>();

    public void StartTutorial()
    {
        AnalyticsManager.Instance.LogEvent("tutorial_start");
        GameData.isTutorialShowedUp = false;
        tutorial.step = 0;
        ShowTutorial();
    }

    public void SaveProgress()
    {
        progressSaved = true;
        var cellsToSave = cellLogic.cells;

        cells.Clear();

        for (int i = 0; i < cellsToSave.Count; i++)
        {
            var cellToSave = cellsToSave[i];
            cells.Add(new CellData()
            {
                position = cellToSave.cellPosition.GridPos,
                data = cellToSave.data
            });
        }


    }
    
    public void ShowTutorial()
    {
        var tipData = tutorial.GetMoveTipData();

        tutorial.sourceMoveCell = tipData.src;
        tutorial.destinationMoveCell = tipData.dst;

        cellLogic.ClearCellsField();

        cellLogic.MakeCell(new Vector2Int(0, HEIGHT - 1), 1);
        cellLogic.MakeCell(new Vector2Int(1, HEIGHT - 1), 2);
        cellLogic.MakeCell(new Vector2Int(2, HEIGHT - 1), 1);
        cellLogic.MakeCell(new Vector2Int(4, HEIGHT - 1), 4);
        cellLogic.MakeCell(new Vector2Int(5, HEIGHT - 1), 3);
        cellLogic.MakeCell(new Vector2Int(5, HEIGHT - 2), 5);

        tutorial.FadeBackground.gameObject.SetActive(true);

        UpdateHandMovingData();

        GameManager.Instance.SetActiveUI(false);

        CellHighlights();
    }

    public void NextTutorialStep()
    {
        tutorial.step++;

        DeleteCellHighlights();

        if (tutorial.step >= tutorial.tutorialMoveTips.Length)
        {
            FinalizeTutorial();
            AnalyticsManager.Instance.LogEvent("tutorial_complete");
            return;
        }

        UpdateHandMovingData();

        CellHighlights();
    }

    private void DeleteCellHighlights()
    {
        if (tutorial.cellHighlight2 != null)
            Destroy(tutorial.cellHighlight2);
    }

    private void CellHighlights()
    {
        var cellPos = cellLogic.GetCellPosition(tutorial.destinationMoveCell);
        if (cellPos.cell != null)
            return;

        tutorial.cellHighlight2 = Instantiate(tutorial.cellHighlightPrefab, GameManager.Instance.PriorityShowParent);
        tutorial.cellHighlight2.transform.position = cellPos.transform.position;
    }

    private void UpdateHandMovingData()
    {
        var tipData = tutorial.GetMoveTipData();

        tutorial.sourceMoveCell = tipData.src;
        tutorial.destinationMoveCell = tipData.dst;

        tutorial.cellPreview.sprite = cellLogic.GetSpriteByValue(cellLogic.GetCell(tutorial.sourceMoveCell).data,out Vector3 scale);
        tutorial.cellPreview.rectTransform.localScale = scale;

        StopCoroutine(handMovingRoutine());
        StartCoroutine(handMovingRoutine());
    }

    public void FinalizeTutorial()
    {
        StopCoroutine(handMovingRoutine());
        
        tutorial.FadeBackground.gameObject.SetActive(false);
        GameData.isTutorialShowedUp = true;
        GameManager.Instance.tapToPlay.Show(progressSaved);

        if (progressSaved)
        {
            cellLogic.ClearCellsField();

            for (int i = 0; i < cells.Count; i++)
            {
                var cell = cells[i];
                cellLogic.MakeCell(cell.position, cell.data);
            }

            progressSaved = false;
            cells.Clear();
        }

    }

    public bool CanDragCell(Cell cell)
    {
        if (GameData.isTutorialShowedUp && !GameData.isCellsLocked) return true;

        return
            cell.cellPosition.GridPos.x == tutorial.sourceMoveCell.x &&
            cell.cellPosition.GridPos.y == tutorial.sourceMoveCell.y;
    }

    public bool IsDestinationCellX(int x)
    {
        return x == tutorial.destinationMoveCell.x;
    }

    public bool IsDestinationCell(int x, int y)
    {
        return x == tutorial.destinationMoveCell.x && y == tutorial.destinationMoveCell.y;
    }

    public bool IsDestinationCell(Vector2Int pos)
    {
        return IsDestinationCell(pos.x, pos.y);
    }

    private IEnumerator handMovingRoutine()
    {
        float val = 0f;

        while (!GameData.isTutorialShowedUp)
        {
            val += Time.deltaTime;

            if (val >= 1f)
            {
                val = 0f;
                yield return new WaitForSeconds(0.05f);
            }

            if (cellLogic.isDragging)
            {
                tutorial.cellPreview.gameObject.SetActive(false);
                tutorial.hand.gameObject.SetActive(false);
                continue;
            }
            else
            {

                tutorial.cellPreview.gameObject.SetActive(true);
                tutorial.hand.gameObject.SetActive(true);
            }

            var cellSize = tutorial.cellPreview.rectTransform.rect.size;
            var handSize = tutorial.hand.rectTransform.rect.size;

            var srcPos = cellLogic.GetCellPosition(tutorial.sourceMoveCell).rectTransform.anchoredPosition3D;
            var dstPos = cellLogic.GetCellPosition(tutorial.destinationMoveCell).rectTransform.anchoredPosition3D;

            if (val >= 1f)
            {
                val = 0f;
                yield return new WaitForSeconds(0.05f);
            }

            if (cellLogic.isDragging)
            {
                tutorial.cellPreview.gameObject.SetActive(false);
                tutorial.hand.gameObject.SetActive(false);
                continue;
            }
            else
            {
                tutorial.cellPreview.gameObject.SetActive(true);
                tutorial.hand.gameObject.SetActive(true);
            }

            if (tutorial.sourceMoveCell.y < tutorial.destinationMoveCell.y)
            {
                var xVal = Mathf.Clamp01(val / 0.7f);
                var yVal = Mathf.Clamp01((val - 0.7f) * 3.3f);

                float x = Mathf.Lerp(srcPos.x, dstPos.x, xVal);
                float y = Mathf.Lerp(srcPos.y, dstPos.y, yVal);

                var offset = new Vector3(0, -cellSize.y / 2, 0);
                var cellOffset = new Vector3(-cellSize.x / 8f, cellSize.y / 2f, 0);
                var targetPos = new Vector3(x, y, 0);

                tutorial.cellPreview.rectTransform.anchoredPosition3D = targetPos + cellOffset + offset;
                tutorial.hand.rectTransform.position = tutorial.cellPreview.rectTransform.position;
            }
            else
            {
                var curve = tutorial.moveCurve.Evaluate((val > 0.5f ? 1 - val : val)); // 0->1 => 0->0.5->0

                if (tutorial.step != 0 && tutorial.step != 3)
                    curve = tutorial.moveCurve.Evaluate(0);

                var offset = new Vector3(0, cellSize.y * curve - cellSize.y / 2, 0);
                var cellOffset = new Vector3(-cellSize.x / 8f, cellSize.y / 2f, 0);
                var targetPos = Vector3.Lerp(srcPos, dstPos, val);

                tutorial.cellPreview.rectTransform.anchoredPosition3D = targetPos + cellOffset + offset;
                tutorial.hand.rectTransform.position = tutorial.cellPreview.rectTransform.position;

            }


            yield return null;
        }
    }


}

