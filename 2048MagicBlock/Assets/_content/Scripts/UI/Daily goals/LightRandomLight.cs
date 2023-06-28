using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LightRandomLight : MonoBehaviour
{
    private Image _liht;

    private void Start()
    {
        _liht = gameObject.GetComponent<Image>();
        RandomLight();
    }

    private void RandomLight()
    {
        float min = Random.Range(0.50f, 0.55f);
        float max = Random.Range(0.75f, 0.85f);
        float delay = Random.Range(0.5f, 0.75f);

        _liht.DOFade(min, delay).onComplete += () =>
        {
            float delay = Random.Range(0.45f, 0.55f);
            _liht.DOFade(max, delay).onComplete += () =>
            {
                RandomLight();
            };
        };
    }
}
