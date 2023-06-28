using UnityEngine;

public class OnEnableSetActiveObj : MonoBehaviour
{
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private GameObject _gameObject2;
    [SerializeField] private GameObject _gameObject3;
    [SerializeField] private bool _valueToSetActiveFunc;

    private void OnEnable()
    {
        _gameObject.SetActive(_valueToSetActiveFunc);
        _gameObject2.SetActive(_valueToSetActiveFunc);
        _gameObject3.SetActive(_valueToSetActiveFunc);
    }
}
