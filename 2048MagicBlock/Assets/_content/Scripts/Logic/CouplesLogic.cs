
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CouplesLogic : MonoBehaviourSingleton<CouplesLogic>
{
    [SerializeField] private GameObject _verticalLinkCrash, _horizontalLinkCrash;


    private CellLogic cellLogic => CellLogic.Instance;


    [SerializeField]
    private GameObject linkSawHorizontalPrefab;
    [SerializeField]
    private GameObject linkSawVerticalPrefab;

    [SerializeField]
    private Image horizontalLink;
    [SerializeField]
    private Image verticalLink;

    public Transform LinksParent;

    private int minCouplesGen = 0;
    private int maxCouplesGen = 2;

    public Image CouplesShow;

    private static int WIDTH => CellLogic.WIDTH;
    private static int HEIGHT => CellLogic.HEIGHT;

    public List<CellCouple> couples = new List<CellCouple>();
    public CellCouple MakeCellCouple(Vector2Int[] positions)
    {
        Cell[] cellsToConnect = new Cell[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            cellsToConnect[i] = cellLogic.GetCell(positions[i]);
            if (cellsToConnect[i] == null)
            {
                Debug.LogError($"{positions[i]} && error making couple");
                return null;
            }
        }

        var couple = new CellCouple()
        {
            cells = cellsToConnect.ToList()
        };

        couple.Init();
        couples.Add(couple);
        return couple;
    }

    public Image MakeLink(Cell from, Cell to, out bool isHorizontal)
    {
        isHorizontal = from.cellPosition.GridPos.x == to.cellPosition.GridPos.x;
        Image img = Instantiate(isHorizontal ? horizontalLink : verticalLink, LinksParent);

        return img;
    }


    /*
        Логика удаления + анимация
    */
    public void RemoveAllCouplesAnimation()
    {
        if (cellLogic.IsMovingGrid || GameManager.Instance.isGameOverScreen)
            return;

        GameManager.Instance.isPause = true;

        if (couples.Count == 0)
            return;

        GameManager.Instance.UpdateStatsUI();

        /*
         Линк - соедниение между двумя ячейками 
         */

        // Получаем все линки в сцепке 
        List<CellCouple.LinkData> links = new List<CellCouple.LinkData>();

        for (int i = 0; i < couples.Count; i++)
        {
            links.AddRange(couples[i].links);
        }

        for (int i = 0; i < links.Count; i++)
        {
            var go = SpawnLinkSaw(links[i]);
            Destroy(go, 1.75f);
        }

        //Выполняем коорутину через 1 секунду используя метод из Utilites 
        //За эту секунду происходит анимация у пил в скрипте SawAnimation.cs
        StartCoroutine(Utilites.ExecuteAfterTime(1.25f, () => {

            //ниже удаляются линки, поэтому можно указать анимацию тут

            /* Вариант 1 */
            for (int i = 0; i < links.Count; i++)
            {
                var isVertical = links[i].isVertical;
                var linkPos = links[i].instance.transform.position;

                Instantiate(isVertical ? _verticalLinkCrash : _horizontalLinkCrash, linkPos, Quaternion.identity, LinksParent);
            }
            /*  */

            for (int i = 0; i < couples.Count; i++)
            {
                couples[i].Destroy();
            }

            couples.Clear();

            GameManager.Instance.isPause = false;
        }));

        /*
        // Вариант 2
        var positions = links.Select(n => n.instance.transform.position).ToList(); //кэшируем позиции

        StartCoroutine(Utilites.ExecuteAfterTime(1.75f, () => {
            for (int i = 0; i < links.Count; i++)
            {
                var isVertical = links[i].isVertical;
                var linkPos = positions[i];

                Instantiate(isVertical ? _verticalLinkCrash : _horizontalLinkCrash, linkPos, Quaternion.identity, LinksParent);
            }

        }));

         */
    }



    //Спавним пилу для линка
    private GameObject SpawnLinkSaw(CellCouple.LinkData linkData)
    {
        GameObject go = null;

        if (linkData.isVertical)
        {
            go = Instantiate(linkSawVerticalPrefab, linkData.instance.transform);
            go.transform.localPosition = Vector3.zero;
        }
        else
        {
            go = Instantiate(linkSawHorizontalPrefab, linkData.instance.transform);
            go.transform.localPosition = Vector3.zero;
        }

        return go;
    }

    public void RemoveAllCouples()
    {
        if (cellLogic.IsMovingGrid || GameManager.Instance.isGameOverScreen)
            return;

        if (couples.Count == 0)
            return;

        GameManager.Instance.UpdateStatsUI();

        for (int i = 0; i < couples.Count; i++)
        {
            couples[i].Destroy();
        }
        couples.Clear();
    }

    private int GetCoupleMaxSize()
    {
        if (cellLogic.maxValue < cellLogic.balance.couplesAEnable)
            return 1;
        if (cellLogic.maxValue < cellLogic.balance.couplesBEnable)
            return cellLogic.balance.couplesMaxSizeA;
        return 3;
    }

    public void GenerateCouples(int recursionCount = 0)
    {
        if (recursionCount > 1)
            return;
        
        var couplesCount = UnityEngine.Random.Range(minCouplesGen, maxCouplesGen);

        var coupleMinSize = 2;
        var coupleMaxSize = GetCoupleMaxSize();

        if (coupleMaxSize < coupleMinSize)
            return;

        int generatedCouples = 0;

        for (int i = 0; i < couplesCount; i++)
        {
            var isHorizontal = UnityEngine.Random.Range(0, 3) > 1;
            if (isHorizontal)
            {
                var coupleSize = UnityEngine.Random.Range(coupleMinSize, coupleMaxSize + 1);
                var maxX = WIDTH - 1 - coupleSize;
                var posX = UnityEngine.Random.Range(0, maxX);

                bool canGenerate = true;
                for (int j = posX; j < maxX; j++)
                {
                    var cell = cellLogic.GetCell(j, HEIGHT - 1);
                    if (cell == null)
                        canGenerate = false;
                    if (cell != null && (cell.couple != null))
                        canGenerate = false;
                }

                if (canGenerate)
                {
                    Vector2Int[] cellPositions = new Vector2Int[coupleSize];
                    for (int j = 0; j < cellPositions.Length; j++)
                    {
                        cellPositions[j] = new Vector2Int(posX + j, HEIGHT - 1);
                        print(cellPositions[j]);
                    }

                    MakeCellCouple(cellPositions);
                    generatedCouples++;
                }
            }
            else
            {
                var x = UnityEngine.Random.Range(0, WIDTH - 1);

                var cell = cellLogic.GetCell(x, HEIGHT - 1);
                var cellTop = cellLogic.GetCell(x, HEIGHT - 2);

                if (cell != null && cellTop != null)
                {
                    if (cellTop.couple != null)
                    {
                        if (cellTop.couple.size.y <= 3)
                            cellTop.couple.Attach(cell);
                    }
                    else
                    {
                        MakeCellCouple(new Vector2Int[] {
                            cell.cellPosition.GridPos,
                            cellTop.cellPosition.GridPos
                        });
                    }
                }
            }


        }

        if (couples.Count > 0 && !GameData.isCouplesShowedUp)
        {
            GameData.isCouplesShowedUp = true;
            if (!GameManager.Instance.boosterLogic.sawBooster.Enabled)
            {
                StartCoroutine(ShowCouplesRoutine());
            }
        }

        recursionCount++;

        GenerateCouples(recursionCount + generatedCouples);
        
    }

    private IEnumerator ShowCouplesRoutine()
    {
        GameManager.Instance.mainTimerObj.SetActive(false);
        CouplesShow.gameObject.SetActive(true);
        GameData.isCellsLocked = true;

        for (int i = 0; i < couples.Count; i++)
        {
            couples[i].ShowTutorial();
        }

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < couples.Count; i++)
        {
            couples[i].HideTutorial();
        }

        CouplesShow.gameObject.SetActive(false);
        GameData.isCellsLocked = false;


        GameManager.Instance.AddSaw();
    }

    public void MoveCouplesDown()
    {
        if (!GameManager.Instance.isGameOverScreen && !GameManager.Instance.isPause)
            for (int i = couples.Count - 1; i >= 0; i--)
            {
                var couple = couples[i];
                if (!couple.IsValid)
                {
                    couples.RemoveAt(i);
                    continue;
                }

                couple.MoveDown();
            }

    }

}

