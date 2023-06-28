using UnityEngine;
using DG.Tweening;

public class RotationChestAnimation : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _maxAngle;

    private Transform _chest;

    public bool inRotationMode;

    private void Awake()
    {
        _chest = gameObject.transform;
    }

    public void StartRotation(bool stateAnim = true)
    {
        inRotationMode = stateAnim;
        if(stateAnim)
            _chest.DORotate(new Vector3(0, 0, _maxAngle), _speed / 2).onComplete += () => { Rotate(); };
    }

    private void Rotate()
    {
        _chest.DORotate(new Vector3(0, 0, -_maxAngle), _speed).onComplete += () =>
        {
            _chest.DORotate(new Vector3(0, 0, _maxAngle), _speed).onComplete += () =>
            {
                if(inRotationMode)
                {
                    Rotate();
                }
              
            };
        };
    }
}
