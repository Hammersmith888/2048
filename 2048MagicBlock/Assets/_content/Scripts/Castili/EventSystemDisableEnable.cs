using UnityEngine;

public class EventSystemDisableEnable : MonoBehaviour
{
    private UnityEngine.EventSystems.EventSystem _eventSystem;

    [SerializeField] private AnimationPanel _AnimPanelComponent;
    
    private void Awake()
    {
        _eventSystem = UnityEngine.EventSystems.EventSystem.current;
    }

    public void DisableEventSystemLateEnable()
    {
        _eventSystem.enabled = false;
        Invoke("EnableEventSystem", _AnimPanelComponent.AnimDuration);
    }

    private void EnableEventSystem()
    {
        _eventSystem.enabled = true;
    }
}
