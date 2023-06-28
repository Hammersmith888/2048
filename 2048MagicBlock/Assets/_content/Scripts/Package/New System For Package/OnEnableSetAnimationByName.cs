using UnityEngine;

public class OnEnableSetAnimationByName : MonoBehaviour
{
    [SerializeField] private string _animationName;
    [SerializeField] private Animator _animatorToSetAnim; 
    
    void OnEnable()
    {
        _animatorToSetAnim.Play(_animationName);
    }
}
