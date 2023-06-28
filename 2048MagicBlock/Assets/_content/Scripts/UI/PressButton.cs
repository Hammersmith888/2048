using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Transform _UI;
    [SerializeField] private float _pressPos, _startPos;
    private Button _button;

    private void Awake()
    {
        _startPos = _UI.transform.position.y;
        _button = gameObject.GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (_button.interactable == true)
            _UI.DOLocalMoveY(_pressPos, 0f);
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        _UI.DOLocalMoveY(0, 0f);
    }
}
