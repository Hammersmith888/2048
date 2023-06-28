using UnityEngine;

public class OnEnableCheckActiveAnotherObj : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private bool _valueToSetActiveFunc;

    private void OnEnable()
    {
        if (_gameObject.activeSelf)
        { 
            gameObject.SetActive(_valueToSetActiveFunc);
        }
    }
}
