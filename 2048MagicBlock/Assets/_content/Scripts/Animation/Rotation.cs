using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] private float _speed;
    //[SerializeField][Range(0.01f,1)] private float _scaleAnim = 1;

    // -1 на право      +1 на лево
    [Header("-1 на право      +1 на лево")]
    [SerializeField] private int _sideOfRotation;
    //[SerializeField] private bool _scaleAnimation = false;

    private Transform _transform;

    private void Start()
    {
        _transform = gameObject.transform;
    }
    private void Update()
    {
        /*if (_scaleAnimation)
        {
            //_transform.localScale = ;
        }*/
        _transform.Rotate(new Vector3(0, 0, (Time.deltaTime * _speed) * _sideOfRotation));
    }
}
