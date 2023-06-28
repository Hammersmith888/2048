using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct FieldBounds
{
    public Transform leftBottomCorner;
    public Transform rightTopCorner;
}


public class CellLogic : MonoBehaviourSingleton<CellLogic>
{
    public int countTimerSecGenWhellAtached = 300;

    public static bool state;

    private TutorialLogic tutorialLogic => TutorialLogic.Instance;
    private CouplesLogic couplesLogic => CouplesLogic.Instance;

    public Sprite[] CellRef;

    public bool _rouletteState;

    [HideInInspector]
    public bool isFieldCheckDirty = false;

    public int MaxValueCounter => maxValueCounter;

    private int maxValueCounter = 0;
    public int maxGenerateKeysCell = 4;
    private int countGenerateKeysCell = 0;

    public const int WIDTH = 6;
    public const int HEIGHT = 9;
    private const int MovingNearCellsDivider = 8;

    public bool IsMovingGrid => isMovingGrid;
    private bool isMovingGrid;

    [HideInInspector]
    public bool isDragging;

    private Vector2 cellsStep;
    private Vector2 cellSize;

    public List<Cell> cells;
    private CellContainer[,] cellPositions;

    public CellContainer[,] GetAllCells => cellPositions;

    public Transform CellsParent;

    public Material cellsMaterial;

    public GameObject wheelCellAttachPrefab;

    public GameObject keyCellAttachPrefab;
    public GameObject moneyCellAttachPrefab;

    private bool isHighlighting = false;
    private bool canGenFortuneCell = true;

    private float cellHighlightTimer = 0;

    private GameManager.GameScreen CurrentScreen => GameManager.Instance.CurrentScreen;
    private bool isPause => GameManager.Instance.isPause;

    public Vector2 CellsStep => cellsStep;
    public Vector2 CellSize => cellSize;

    [SerializeField]
    private GameObject combineParticlePrefab;

    [SerializeField]
    private Image highlightPrefab;

    [SerializeField]
    private Cell cellPrefab;
    [SerializeField]
    private Transform CellsGridParent;

    public BalanceValues balance;


    [SerializeField]
    public GameObject _25counterObject;

    [SerializeField]
    private AnimationPanel startDataSelector;
    [SerializeField]
    private GameObject startData18;

    [SerializeField]
    private GameObject wheelFortuneButton;

    [SerializeField]
    public Slider timerSlider;

    [SerializeField]
    private GameManager _gameManager;

    [HideInInspector]
    public Timer timer;

    public int Score => scoreTarget;

    public int scoreTarget = 0;

    private float scoreAddTimer;
    private const float scoreAddDelay = 0.05f;

    public event System.Action<int> onCombineNewMaxValue;

    [Header("Для отладки")]

    public int maxValue = 5;


    private void Start()
    {
        countGenerateKeysCell = PlayerPrefs.GetInt("countGenerateKeysCell", countGenerateKeysCell);
    }


#if UNITY_EDITOR

    [EasyButtons.Button("Unlock startFrom 10")]
    public void UnlockStartFrom10()
    {
        GameManager.Instance.gameData.score = 10;
    }
    [EasyButtons.Button("Unlock startFrom 18")]
    public void UnlockStartFrom18()
    {
        GameManager.Instance.gameData.score = 18;

    }

#endif

    public void AnimateScore()
    {
        if (GameManager.Instance.gameData.score < scoreTarget)
        {
            //Debug.LogError("scoreTarget ------" + scoreTarget);
            scoreAddTimer += Time.deltaTime;
            if (scoreAddTimer > scoreAddDelay)
            {
                scoreAddTimer = 0;
                GameManager.Instance.gameData.score++;
                GameManager.Instance.UpdateStatsUI();
            }
        }
    }

    public void CheckCellHighlight()
    {
        if (!GameData.isTutorialShowedUp)
            return;
        if (isPause)
            return;
        if (CurrentScreen != GameManager.GameScreen.Game)
            return;

        if (cellHighlightTimer > 5f && !isHighlighting && !isDragging)
        {
            cellHighlightTimer = 3f;
            StartCoroutine(HighlightCellsRoutine());
        }

        if (isDragging && isHighlighting)
        {
            isHighlighting = false;
        }

        if (!isDragging)
            cellHighlightTimer += Time.deltaTime;
        else
            cellHighlightTimer = 0;
    }

    private List<GameObject> highlightingOfCells = new List<GameObject>();

    private IEnumerable<IGrouping<int, Cell>> GetDublicates()
    {
        return cells.GroupBy(n => n.data).Where(g => g.Count() > 1);
    }

    [EasyButtons.Button("Clear cells field")]
    public void ClearCellsField()
    {
        couplesLogic.RemoveAllCouples();
        for (int i = 0; i < cells.Count; i++)
        {
            Destroy(cells[i].gameObject);
        }
        cells.Clear();
    }

    private IEnumerator HighlightCellsRoutine()
    {
        isHighlighting = true;
        var cellDublicateValues = GetDublicates();

        Cell cell1;
        Cell cell2;


        if (cellDublicateValues.Count() > 0)
        {
            cell1 = cellDublicateValues.ElementAt(0).ElementAt(0);
            cell2 = cellDublicateValues.ElementAt(0).ElementAt(1);

            var highlight1 = Instantiate(highlightPrefab, CellsParent);
            highlight1.transform.SetAsFirstSibling();
            var highlight2 = Instantiate(highlightPrefab, CellsParent);
            highlight2.transform.SetAsFirstSibling();

            highlightingOfCells.Add(highlight1.gameObject);
            highlightingOfCells.Add(highlight2.gameObject);

            float timer = 0;
            while (isHighlighting && cell1 != null && cell2 != null)
            {
                timer += Time.deltaTime / 2;

                float val = timer < 0.5f ? timer * 2 : (1 - timer) * 2;

                if (timer > 1f)
                    break;

                highlight1.transform.position = cell1.transform.position;
                highlight2.transform.position = cell2.transform.position;

                highlight1.color = highlight2.color = new Color(1, 1, 1, val);

                yield return null;

            }

            StopHighlighting();
        }


        yield return null;
    }

    private void StopHighlighting()
    {
        for (int i = 0; i < highlightingOfCells.Count; i++)
        {
            Destroy(highlightingOfCells[i]);
        }
        isHighlighting = false;
        highlightingOfCells.Clear();
    }

    public void UpdateCellStep()
    {
        var cell0 = cellPositions[0, 0];
        var cell1 = cellPositions[1, 1];

        if (cells.Count > 0)
        {
            var cellImg = cells[0];
            cellSize = cellImg.image.rectTransform.rect.size;
        }

        cellsStep = new Vector2(
            Mathf.Abs(cell1.transform.position.x - cell0.transform.position.x),
            Mathf.Abs(cell1.transform.position.y - cell0.transform.position.y));

    }

    private IEnumerator MoveGridRoutine()
    {
        yield return null;

        if (isMovingGrid)
            yield return new WaitUntil(() => isMovingGrid);

        isMovingGrid = true;

        var positions = new Vector2Int[cells.Count];

        for (int i = 0; i < cells.Count; i++)
        {
            positions[i] = cells[i].cellPosition.GridPos;
            cells[i].cellPosition = null;
        }

        for (int i = cells.Count - 1; i >= 0; i--)
        {
            var cell = cells[i];
            var newPos = positions[i] + new Vector2Int(0, -1);
            if (cell.data == 0)
                continue;
            /*   if (newPos.y < 0)
               {
                   cell.data = 0;
                   cells.RemoveAt(i);
                   Debug.Log("Game over");
                   GameManager.Instance.GameOver();
                   continue;
               }*/

            cell.cellPosition = GetCellPosition(newPos);
            cell.ResetPosition();
        }

        int countGeneratedKey = 0;
        int countGeneratedMoneyCell = 0;
        for (int ix = 0; ix < WIDTH; ix++)
        {
            int dataOnTop = getData(ix, HEIGHT - 2);
            
            while (true)
            {
                //var genData = UnityEngine.Random.Range(1, maxValue + 1);

                var genData = GenerateData();

                Debug.Log(genData);
                if (genData == dataOnTop)
                    continue;

                Debug.Log("MakeCell");
                var cell = MakeCell(new Vector2Int(ix, HEIGHT-1 ), genData);

                if (Random.Range(0, 100) < balance.wheelFortuneChance && canGenFortuneCell)
                    GenFortuneCell(cell);
                else if (Random.Range(0, 100) < balance.moneyCellChance
                    && countGeneratedMoneyCell < 1)
                {
                    GenMoneyCell(cell);
                    countGeneratedMoneyCell++;
                }
                else if (Random.Range(0, 100) < balance.keyChance 
                    && countGenerateKeysCell < maxGenerateKeysCell
                    && countGeneratedKey < 1)
                {
                    GenKeyCell(cell);

                    countGenerateKeysCell++;
                    countGeneratedKey++;
                    PlayerPrefs.SetInt("countGenerateKeysCell", countGenerateKeysCell);
                }

               
                break;
            }
        }

        CheckCellsEmpty();
        couplesLogic.GenerateCouples();

        Debug.Log("Move grid");
        yield return new WaitForSeconds(0.12f);

        isMovingGrid = false;
    }

    //void a()
    //{
    //    for (int ix = 0; ix < WIDTH; ix++)
    //    {
    //        int dataOnTop = getData(ix, HEIGHT - 2);

    //        while (true)
    //        {
    //            //var genData = UnityEngine.Random.Range(1, maxValue + 1);

    //            var genData = GenerateData();

    //            Debug.Log(genData);
    //            if (genData == dataOnTop)
    //                continue;

    //            Debug.Log("MakeCell");
    //            var cell = MakeCell(new Vector2Int(ix, HEIGHT - 1), genData);

    //            if (Random.Range(0, 100) < balance.wheelFortuneChance)
    //                GenFortuneCell(cell);
    //            else if (Random.Range(0, 100) < balance.keyChance)
    //            {
    //                cell.keyAttached = Instantiate(keyCellAttachPrefab, cell.transform);

    //                cell.keyAttached.transform.position = cell.transform.position;
    //            }


    //            break;
    //        }
    //    }
    //}

    private WheelCellAttached cellAttached;
    private Coroutine TimerWheelFortune5MinReturned;

    private void GenFortuneCell(Cell cell, bool generateImmediately = false)
    {
        if (wheelFortuneButton.activeInHierarchy)
            return;
        
        if (cellAttached != null && cellAttached.isDestroyed || generateImmediately)
            cellAttached = null;
        
        if (cellAttached != null)
            return;
        
        if (cellAttached == null && (canGenFortuneCell || generateImmediately))
        {
            if (TimerWheelFortune5MinReturned == null)
            {
                TimerWheelFortune5MinReturned = StartCoroutine(TimerWheelFortune5Min(countTimerSecGenWhellAtached));
                canGenFortuneCell = false;
            }

            //GameManager.Instance.AddFreeSpin();

            Debug.LogError("cellAttached = cell.AttachWheel();3");
            cellAttached = cell.AttachWheel();
        }
    }

    private void GenMoneyCell(Cell cell)
    {
        cellAttached = cell.AttachWheel(true);
    }

    private void GenKeyCell(Cell cell)
    {
        cell.keyAttached = Instantiate(keyCellAttachPrefab, cell.transform);

        cell.keyAttached.transform.position = cell.transform.position;
    }
    private IEnumerator TimerWheelFortune5Min(int countTimerSec)
    {
        yield return new WaitForSeconds(1);
        countTimerSec--;

        if (countTimerSec <= 0)
        {
            canGenFortuneCell = true;
        }
    }

    public Cell GetCell(int x, int y)
    {
        return cellPositions[x, y].cell;
    }

    public Cell GetCell(Vector2Int pos)
    {
        return GetCellPosition(pos).cell;
    }

    private int cellCounter = 0;
    public Cell MakeCell(Vector2Int pos, int data = 0, int wheelAttached = 0)
    {
        cellCounter++;
        var cell = Instantiate(cellPrefab, CellsParent);

        if (wheelAttached == 3)
        {
            GenKeyCell(cell);
        }
        if (wheelAttached == 2)
        {
            GenFortuneCell(cell,true);
        }
        else if (wheelAttached == 1)
        {
            GenMoneyCell(cell);
        }

        cell.name += cellCounter.ToString();
        cell.cellPosition = GetCellPosition(pos);
        cell.ResetPositionImmediate();
        cell.data = data;
        cells.Add(cell);
        return cell;
    }

    public Cell MakeCell(int x, int y, int data = 1)
    {
        return MakeCell(new Vector2Int(x, y), data);
    }

    public void CheckCellsEmpty()
    {
        for (int i = cells.Count - 1; i >= 0; i--)
        {
            if (cells[i] == null)
                cells.RemoveAt(i);
        }
    }

    private int GenerateData()
    {
        float chance = UnityEngine.Random.Range(0, 1f);

        int bigChanceMin = 15;
        int bigChanceMax = 21;
        int midChanceMin = 15;
        int smallChanceMin = 1;
        int smallChanceMax = 3;

        if (maxValue >= balance.step4.maxValue)
        {
            bigChanceMin = balance.step5.bigChanceMin;
            bigChanceMax = balance.step5.bigChanceMax;
            midChanceMin = balance.step5.midChanceMin;
            smallChanceMin = balance.step5.smallChanceMin;
            smallChanceMax = balance.step5.smallChanceMax;
        }
        else if (maxValue >= balance.step3.maxValue)
        {
            bigChanceMin = balance.step4.bigChanceMin;
            bigChanceMax = balance.step4.bigChanceMax;
            midChanceMin = balance.step4.midChanceMin;
            smallChanceMin = balance.step4.smallChanceMin;
            smallChanceMax = balance.step4.smallChanceMax;
        }
        else if (maxValue >= balance.step2.maxValue)
        {
            bigChanceMin = balance.step3.bigChanceMin;
            bigChanceMax = balance.step3.bigChanceMax;
            midChanceMin = balance.step3.midChanceMin;
            smallChanceMin = balance.step3.smallChanceMin;
            smallChanceMax = balance.step3.smallChanceMax;
        }
        else if (maxValue >= balance.step1.maxValue)
        {
            bigChanceMin = balance.step2.bigChanceMin;
            bigChanceMax = balance.step2.bigChanceMax;
            midChanceMin = balance.step2.midChanceMin;
            smallChanceMin = balance.step2.smallChanceMin;
            smallChanceMax = balance.step2.smallChanceMax;
        }
        else if (maxValue >= 5)
        {
            bigChanceMin = balance.step1.bigChanceMin;
            bigChanceMax = balance.step1.bigChanceMax;
            midChanceMin = balance.step1.midChanceMin;
            smallChanceMin = balance.step1.smallChanceMin;
            smallChanceMax = balance.step1.smallChanceMax;
        }

        int genData = 0;
        if (chance > 0.5f)
            genData = UnityEngine.Random.Range(bigChanceMin, bigChanceMax + 1);
        else if (chance > 0.2f)
            genData = UnityEngine.Random.Range(midChanceMin, bigChanceMax + 1);
        else
        {
            genData = UnityEngine.Random.Range(smallChanceMin, smallChanceMax + 1);
        }

        return genData;
    }

    public void ShowStartDataSelect()
    {
        if (!GameData._10Opened)
        {
            SetStartData();
            return;
        }
        BoosterLogic.Instance.sawBooster.Enabled = false;
        BoosterLogic.Instance.timeBooster.Enabled = false;
        GameManager.Instance.isPause = true;
        startData18.SetActive(GameData._18Opened);
        startDataSelector.Show();
    }

    public void SetStartData(int val)
    {
        GameManager.Instance.isPause = false;
        maxValue = val;
        SetStartData();
        startDataSelector.Hide();
    }

    private void SetStartData()
    {
        ClearCellsField();

        for (int ix = 0; ix < WIDTH; ix++)
        {
            for (int iy = HEIGHT - 3; iy < HEIGHT; iy++)
            {
                while (true)
                {
                    int value = Random.Range(1, maxValue);
                    var cell = GetCell(ix, iy - 1);
                    if (cell != null && cell.data == value)
                        continue;

                    MakeCell(ix, iy, value);
                    break;
                }
            }
        }
    }

    public void InitCells()
    {
        cells = new List<Cell>();
        cellPositions = new CellContainer[WIDTH, HEIGHT];
        var cellsGrid = CellsGridParent.GetComponentsInChildren<CellContainer>();

        for (int i = 0; i < cellsGrid.Length; i++)
        {
            int x = i % WIDTH;
            int y = i / WIDTH;
            cellsGrid[i].GridPos = new Vector2Int(x, y);
            cellPositions[x, y] = cellsGrid[i];
        }
    }

    public int getData(int x, int y, CellCouple compareCouple)
    {
        if (x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT)
        {
            var cell = GetCell(x, y);
            if (cell == null || (cell != null && cell.couple == compareCouple))
                return 0;
            return cell.data;
        }
        return 0;
    }

    public int getData(int x, int y)
    {
        if (x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT)
        {
            var cell = GetCell(x, y);
            if (cell == null)
                return 0;
            return cell.data;
        }
        return 0;
    }

    private const float CellMoveAroundClamp = 1.25f;

    public Vector3 ClampCellMoving(Cell cell, Vector3 targetPosition)
    {
        var initPos = cell.cellPosition.transform.position;
        var gridPos = cell.cellPosition.GridPos;

        var xClamp = cellsStep.x / MovingNearCellsDivider;
        var yClamp = cellsStep.y / MovingNearCellsDivider;

        float minX = initPos.x - xClamp;
        float maxX = initPos.x + xClamp;

        float minY = initPos.y - yClamp;
        float maxY = initPos.y + yClamp;

        if (CheckCellData(cell, new Vector2Int(gridPos.x - 1, gridPos.y)))
            minX = initPos.x - cellsStep.x * CellMoveAroundClamp;

        if (CheckCellData(cell, new Vector2Int(gridPos.x + 1, gridPos.y)))
            maxX = initPos.x + cellsStep.x * CellMoveAroundClamp;

        if (CheckCellData(cell, new Vector2Int(gridPos.x, gridPos.y - 1)))
            maxY = initPos.y + cellsStep.y * CellMoveAroundClamp;

        if (CheckCellData(cell, new Vector2Int(gridPos.x, gridPos.y + 1)))
            minY = initPos.y - cellsStep.y * CellMoveAroundClamp;

        var delta = targetPosition - initPos;

        var deltaAbs = new Vector2(Mathf.Abs(delta.x), Mathf.Abs(delta.y));

        int gx = delta.x < 0 ? -1 : 1;
        int gy = delta.y < 0 ? -1 : 1;

        if (deltaAbs.x > xClamp && deltaAbs.y > yClamp)
        {
            if (deltaAbs.x > deltaAbs.y)
            {
                if (!CheckCellData(cell, new Vector2Int(gridPos.x + gx, gridPos.y + 1)))
                    minY = initPos.y - yClamp;
                if (!CheckCellData(cell, new Vector2Int(gridPos.x + gx, gridPos.y - 1)))
                    maxY = initPos.y + yClamp;
            }
            else
            {
                if (!CheckCellData(cell, new Vector2Int(gridPos.x - 1, gridPos.y + gy)))
                    minX = initPos.x - xClamp;
                if (!CheckCellData(cell, new Vector2Int(gridPos.x + 1, gridPos.y + gy)))
                    maxX = initPos.x + xClamp;
            }
        }

        float x = Mathf.Clamp(targetPosition.x, minX, maxX);

        float y = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(x, y, targetPosition.z);
    }

    private bool CheckCellData(Cell currentCell, Vector2Int targetGridPos)
    {
        if (targetGridPos.x < 0 || targetGridPos.x >= WIDTH || targetGridPos.y < 0 || targetGridPos.y >= HEIGHT)
            return false;

        var otherCell = GetCell(targetGridPos);

        var couple = currentCell.couple;

        return (GameData.isTutorialShowedUp
                || otherCell == null
                    || (targetGridPos.x == tutorialLogic.Tutorial.destinationMoveCell.x
                        && targetGridPos.y == tutorialLogic.Tutorial.destinationMoveCell.y))
                        && (otherCell == null
                            || otherCell.data == 0
                            || otherCell.data == currentCell.data
                            || (couple != null && otherCell.couple == couple));
    }

    public void SwapCells(Cell src, Cell dst)
    {
        var temp = src.cellPosition;
        src.cellPosition = dst.cellPosition;
        dst.cellPosition = temp;

        src.cellPosition.cell = src;
        dst.cellPosition.cell = dst;

        isFieldCheckDirty = true;
    }

    private Color GetColorFromSprite(Sprite sprite)
    {
        if (sprite == null)
            return Color.white;

        var tex = sprite.texture;
        var w = tex.width;
        var h = tex.height;

        return tex.GetPixel((int)(w * 0.8f), (int)(h * 0.8f));
    }

    private void SetColorParticles(ParticleSystem system, Color color)
    {
        var settings = system.main;
        settings.startColor = color;
    }

    private IEnumerator ResetTimerRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (timer.value > delay + 0.1f)
        {
            ResetTimer();
        }
    }
    //private void Update()
    //{
    //    if (Input.GetKeyUp(KeyCode.F6))
    //    {
    //        MoveGrid();
    //    }
    //    if (Input.GetKeyUp(KeyCode.F7))
    //    {
    //        //Saver.Instance.DebugSave();
    //    }
    //    if (Input.GetKeyUp(KeyCode.F5))

    //}
    public void MoveGrid()
    {
        if (isPause || isMovingGrid || GameManager.Instance.isGameOverScreen)
            return;

        cellHighlightTimer = 0f;

        if (IsGameOver())
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            StartCoroutine(MoveGridRoutine());
        }


    }

    public bool IsGameOver()
    {
        var positions = new Vector2Int[cells.Count];

        for (int i = 0; i < cells.Count; i++)
        {
            positions[i] = cells[i].cellPosition.GridPos;
            //  cells[i].cellPosition = null;
        }

        for (int i = cells.Count - 1; i >= 0; i--)
        {
            var cell = cells[i];
            var newPos = positions[i] + new Vector2Int(0, -1);
            Debug.Log(newPos);
            if (cell.data == 0)
                continue;
            if (newPos.y < 0)
            {
                //  cell.data = 0;
                //  cells.RemoveAt(i);
                Debug.Log("Game over");
                Saver.Instance.ClearSave();
                return true;
            }

            //  cell.cellPosition = GetCellPosition(newPos);
            //   cell.ResetPosition();
        }
        return false;
    }

    public void ResetTimer(bool invokeEvent = true)
    {
        if (!GameData.isTutorialShowedUp)
            return;

        if (isPause || isMovingGrid || GameManager.Instance.isGameOverScreen)
            return;

        timer.Reset(invokeEvent);
    }

    public Cell GetOrMakeCell(Vector2Int pos)
    {
        var cell = GetCell(pos);
        if (cell == null)
            cell = MakeCell(pos);
        return cell;
    }

    public CellContainer GetCellPosition(Vector2Int pos)
    {
        return cellPositions[pos.x, pos.y];
    }

    //проверить есть ли еще ячейки с одинаковыми значениями
    public void CheckFieldValid()
    {
        if (!GameData.isTutorialShowedUp)
            return;

        CheckCellsEmpty();

        if (cells.Count == 0) return;

        var cellDatas = cells.Select(n => n.data).ToList();
        if (cellDatas.Distinct().ToList().Count == cellDatas.Count)
        {
            StartCoroutine(ResetTimerRoutine(0.2f));
        }

        if (couplesLogic.couples.Count == 1)
        {
            bool reset = true;

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].couple != couplesLogic.couples[0])
                    reset = false;
            }

            if (reset)
                StartCoroutine(ResetTimerRoutine(0.2f));
        }
    }

    public void OnCellDestroy(Cell cell)
    {
        cells.Remove(cell);
    }

    public Sprite GetSpriteByValue(int data, out Vector3 scale)
    {
        return BlocksSkinController.Instance.GetSpriteByValue(data, out scale);
    }

    public void CombineCells(Cell src, Cell dst, bool animPos = true, Cell src2 = null)
    {
        if (src.data != dst.data)
            return;

        var cellNear = new Cell();
        float cellDistanceMin = 0;
        float cellLastDistanceMin = 0;

        if (src.Equals(dst))
        {
            foreach (var item in cells)
            {
                //Debug.LogError("----foreach item.data == src2.data " + item.data + "    " + src2.data);
                if (src2 != null && item.data == src2.data && !src2.Equals(item))
                {
                    cellDistanceMin = Vector2.Distance(src2.rectTransform.anchoredPosition, item.rectTransform.anchoredPosition);
                    
                    if (cellLastDistanceMin > cellDistanceMin || cellLastDistanceMin == 0)
                    {
                        cellLastDistanceMin = cellDistanceMin;
                        cellNear = item;
                    }
                }
            }

            if (cellNear != null && (int)cellLastDistanceMin != 0)
            {
                cellNear.data++;
                cellNear.GetComponent<Animator>().Play("CombineAnim");
            }
        }

        GoalsEventManager.SendUpMerge20Times();  

        dst.GetComponent<Animator>().Play("CombineAnim");

        var go = Instantiate(combineParticlePrefab, GameManager.Instance.PriorityShowParent);
        var particleSystem = go.GetComponent<ParticleSystem>();

        var sprite = GetSpriteByValue(dst.data + 1, out Vector3 scale);

        SetColorParticles(particleSystem, GetColorFromSprite(sprite));

        go.transform.position = dst.transform.position;
        Destroy(go, 1f);

        if (src.couple != null)
            src.couple.Deatach(src);

        if (dst.couple != null)
            dst.couple.Deatach(dst);

        RemoveCell(src);
       

        dst.data++;

        Attached(dst);
        Attached(src);

        void Attached(Cell localCell)
        {

            if (localCell.IsWheelAttached())
            {
                GameManager.Instance.AnimateWheelFortuneButton(
                    localCell.wheelAttached.wheelInstance.transform.GetChild(0).position,
                    wheelFortuneButton.transform.position);
                localCell.DeatachWheel();
            }

            if (localCell.IsMoneyCellAttached())
            {
                GameManager.Instance.AnimateMoneyCellButton(
                    localCell.wheelAttached.wheelInstance.transform.GetChild(0).position,
                    GameManager.Instance.moneyStatIconPosition.transform.position,
                    localCell.wheelAttached.moneyInCell);
                localCell.DeatachWheel();
            }

            if (localCell.keyAttached != null)
            {
                GameManager.Instance.AnimateKeyButton(
                    localCell.keyAttached.transform.GetChild(0).position,
                    GameManager.Instance.packageButton.transform.position);
                Destroy(localCell.keyAttached);

                if (GameManager.Instance.CurrentScreen.Equals(GameManager.GameScreen.Game))
                    this.GetComponent<BlocksSkinControllerNew>().Add1KeyToOpenSkin();
            }
        }
        
        scoreTarget += dst.data;

        Debug.Log(dst.data);
        if (PlayerPrefs.GetInt("HighCellNumber", 2) < dst.data) 
            PlayerPrefs.SetInt("HighCellNumber", dst.data);
        
        Debug.Log(maxValue);

        if (dst.data  > maxValue )
        {
            //Event вызывается, когда увеличивается максимальное число блока в сцене
            onCombineNewMaxValue?.Invoke(dst.data);

           // Debug.LogError("dst.data -> " + dst.data);
            
            if (dst.data  == 9 || dst.data  == 14 || dst.data  == 19 || dst.data  == 24)
            {
                //Debug.Log(state);
                if (dst.data  == 14)
                {
                    GoalsEventManager.SendUpCollect1Block15InNormalMode();
                }

                // Вызов Окна
                if (state == false)
                {
                    //Debug.LogError("Вызов Окна dst.data -> " + dst.data);
                    _gameManager.ShowRouletteAndUpBlock(dst.data + 1 );
                    state = true;
                }
            }


        }

        if (dst.data == 25)
        {
            if (!GameManager.Instance.gameData.endlessUnlocked)
            {
                GameManager.Instance.gameData.endlessUnlocked = true;
                GameManager.Instance.UpdateEndlessButton();
                GameManager.Instance.gameData.SaveData();
            }

            if (GameManager.Instance.gameData.isEndless)
            {
                maxValueCounter++;
                GameManager.Instance.maxValueCounterText.text = $"x{maxValueCounter}";
                RemoveCell(dst);
                return;
            }
            else
            {
                isFieldCheckDirty = true;
            }

        }

        var target = Vector3.Lerp(dst.transform.position, src.transform.position, 0.5f);

        if (animPos)
        {
            src.transform.DOKill();
            src.transform.DOMove(target, 0.1f);
        }

        var tween1 = dst.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f);

        tween1.onKill += () =>
        {
            dst.transform.localScale = scale;
            if (animPos)
                dst.transform.position = dst.cellPosition.transform.position;
        };

        maxValue = Mathf.Max(maxValue, dst.data);

        isFieldCheckDirty = true;
    }

    public bool IsPositionInsideGrid(int x, int y)
    {
        return IsPositionInsideGrid(new Vector2Int(x, y));
    }

    public bool IsPositionInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < WIDTH && pos.y < HEIGHT;
    }


    public void RemoveCell(Cell cell)
    {
        GameManager.Instance._mergeSound.PlaySound();

        if (cell.couple != null)
            cell.couple.Deatach(cell);
        /*
        if (cell.IsWheelAttached())
        {
            GameManager.Instance.AnimateWheelFortuneButton(
                cell.wheelAttached.wheelInstance.transform.GetChild(0).position,
                wheelFortuneButton.transform.position);
        }

        if (cell.IsMoneyCellAttached())
        {
            GameManager.Instance.AnimateMoneyCellButton(
                cell.wheelAttached.wheelInstance.transform.GetChild(0).position,
                GameManager.Instance.moneyStatIconPosition.transform.position,
                cell.wheelAttached.moneyInCell);
            cell.DeatachWheel();
        }

        if (cell.keyAttached != null)
        {
            GameManager.Instance.AnimateKeyButton(
                cell.keyAttached.transform.GetChild(0).position,
                GameManager.Instance.packageButton.transform.position);
            Destroy(cell.keyAttached);

            if(GameManager.Instance.CurrentScreen.Equals(GameManager.GameScreen.Game))
                this.GetComponent<BlocksSkinControllerNew>().Add1KeyToOpenSkin();
        }*/


        cell.data = 0;

        if (cells.Count < 2)
            timer.Reset(true);

        isFieldCheckDirty = true;
    }

    public void RemoveRandomCellsAnimated()
    {
        StartCoroutine(RemoveRandomCellsAnimationRoutine());
    }

    private IEnumerator RemoveRandomCellsAnimationRoutine()
    {
        int max = Mathf.RoundToInt(cells.Count * 0.3f);
        int count = 0;

        List<Cell> removeCells = new List<Cell>();

        for (int iy = 0; iy < HEIGHT; iy++)
        {
            List<Cell> rowCells = new List<Cell>();

            for (int ix = 0; ix < WIDTH; ix++)
            {
                if (cellPositions[ix, iy].cell != null)
                {
                    rowCells.Add(cellPositions[ix, iy].cell);
                }
            }

            for (int i = 0; i < rowCells.Count; i++)
            {
                if (count > max)
                    break;

                if (Random.Range(0, 1f) > 0.5f)
                {
                    count++;
                    removeCells.Add(rowCells[i]);
                }
            }
        }

        for (int j = 1; j < 10; j++)
        {
            for (int i = 0; i < removeCells.Count; i++)
            {
                var cell = removeCells[i];

                var rand = Random.Range(0, 2);
                var rand1 = Random.Range(0, 2);

                cell.rectTransform.anchoredPosition3D += new Vector3((j + rand) % 2 == 0 ? 1 : -1, (j + rand1) % (1f / 2) == 0 ? 1 : -1, 0) * 10f;
                cell.ResetPosition();
            }

            yield return new WaitForSeconds(0.1f);
        }


        for (int i = 0; i < removeCells.Count; i++)
        {
            var rand = Random.Range(-1f, 1f);

            var cell = removeCells[i];
            cell.StopAllCoroutines();

            var targetPos = new Vector3(
                cell.rectTransform.anchoredPosition3D.x + rand * Random.Range(800,1500),
                cell.rectTransform.anchoredPosition3D.y - CanvasManager.Height / 2f,
                0);

            cell.rectTransform.DOAnchorPos(targetPos, 1f);

            cell.rectTransform.DORotate(new Vector3(0, 0, rand * 180), 1f);
            cell.image.DOColor(new Color(1f, 1f, 1f, 0f), 0.94f);
            cell.image2.DOColor(new Color(1f, 1f, 1f, 0f), 0.94f);
            cell.gameObject.AddComponent<Canvas>().overrideSorting = true;
            cell.gameObject.GetComponent<Canvas>().sortingOrder = 16;

        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < removeCells.Count; i++)
        {
            RemoveCell(removeCells[i]);

        }

        GameManager.Instance.SaveGameCellsPosition();

    }


    public void LoadSave()
    {
        List<Vector2Int[]> newCells = new List<Vector2Int[]>();

        CellsSaweData data = Saver.Instance.LoadInfo();

        foreach (LineData lineData in data.data)
        {
            foreach (Data cell in lineData.data)
            {
                if (cell.data != -1)
                {
                    MakeCell(cell.position, cell.data, cell.typeAttachedCell);

                    if (cell.haveCouple)
                    {
                        List<Vector2Int> vec= new List<Vector2Int>();

                        vec.Add(cell.position);
                        foreach(Vector2Int v in cell.сouplePosicion)
                            vec.Add(v);

                        newCells.Add(vec.ToArray());
                    }
                }
            }
        }

        List<Cell> inCouple= new List<Cell>();

        foreach (Vector2Int[] c in newCells)
        {
            if (inCouple.Contains(Instance.GetCell(c[0])))
                continue;

            foreach (Vector2Int v in c)
            {
                inCouple.Add(Instance.GetCell(v));
            }
            CouplesLogic.Instance.MakeCellCouple(c);
        }
    }

    
}

