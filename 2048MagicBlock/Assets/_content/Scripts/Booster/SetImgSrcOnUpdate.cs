using UnityEngine;
using UnityEngine.UI;

public class SetImgSrcOnUpdate : MonoBehaviour
{
    [SerializeField] private Image _spriteSetFromWhichImageComponent;
    private Image _imageComponentForSetImg;

    private void Start()
    {
        _imageComponentForSetImg = this.GetComponent<Image>();
        _imageComponentForSetImg.sprite = _spriteSetFromWhichImageComponent.sprite;
    }
     
    private void Update()
    {
        if (!_imageComponentForSetImg.sprite.Equals(_spriteSetFromWhichImageComponent.sprite))
        {
            _imageComponentForSetImg.sprite = _spriteSetFromWhichImageComponent.sprite;
        }
    }
}
