using UnityEngine;
using UnityEngine.UI;

public class SpriteEqualsAndEnableBlickScript : MonoBehaviour
{
    [SerializeField] private Sprite _spriteToEquals;
    [SerializeField] private Image _imageComponentToSpriteEquals;
    [SerializeField] private BlickMove _BlickMove;

    void Start()
    {
        if (_spriteToEquals == _imageComponentToSpriteEquals.sprite)
        {
            _BlickMove.enabled = true;
        }
    }
}
