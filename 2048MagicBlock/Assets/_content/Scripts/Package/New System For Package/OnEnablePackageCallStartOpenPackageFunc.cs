using UnityEngine;

public class OnEnablePackageCallStartOpenPackageFunc : MonoBehaviour
{
    [SerializeField] private GameObject _ChoiseGroupPackageObject;
    private void OnEnable()
    {
        _ChoiseGroupPackageObject.GetComponent<ChoiseGroupPackage>().StartOpenPackage();
        BlocksSkinControllerNew.Instance._isOpenedPackage = true;
    }
    private void OnDisable()
    {
        BlocksSkinControllerNew.Instance._isOpenedPackage = false;
    }
}
