using UnityEngine;
using UnityEngine.UI;
//Don't use script
public class LinkDataNew
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
/////////////////////////////////////////////////////////////////////////////////////////////////////////////
public class CellCouplesLogicNew : MonoBehaviour
{
    

    [SerializeField] private GameObject _verticalLinkCrash, _horizontalLinkCrash;
    [SerializeField] private GameObject _linkSawHorizontalPrefab;
    [SerializeField] private GameObject _linkSawVerticalPrefab;
    [SerializeField] private Image _horizontalLink;
    [SerializeField] private Image _verticalLink;

    private int minCouplesGen = 0;
    private int maxCouplesGen = 2;
    public Transform LinksParent;

    
}


