using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LightMenu : MonoBehaviour
{
    private Image _liht;
    private void Start()
    {
        _liht = gameObject.GetComponent<Image>();
        RandomLight();
    }

    private void RandomLight()
    {
        float min = Random.Range(0.20f, 0.25f);
        float max = Random.Range(0.30f, 0.35f);
        float delay = Random.Range(0.3f, 0.5f);

        _liht.DOFade(min, delay).onComplete += () =>
        {
            float delay = Random.Range(0.2f, 0.5f);
            _liht.DOFade(max, delay).onComplete += () =>
            {
                RandomLight();
            };
        };
    }
}
