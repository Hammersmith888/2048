using UnityEngine;
using DG.Tweening;

//Скрипт делает движение объекта (пилы) туда-сюда
public class SawAnimation : MonoBehaviour
{
    //Первая граница движения
    [SerializeField]
    private Vector3 localStart;
    //Вторая граница движения
    [SerializeField]
    private Vector3 localEnd;
    //Скорость движения
    [SerializeField]
    private float speed = 1f;
    //Порог (дистанция) при котором меняется направление движения
    [SerializeField]
    private float threshold = 5;

    private bool direction = true;

    void Update()
    {
        if (direction)
            MoveSaw(localStart * 0.75f, localEnd * 0.75f);
        else
            MoveSaw(localEnd * 0.75f, localStart * 0.75f);
    }

    private void MoveSaw(Vector3 from,Vector3 to)
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, to, Time.deltaTime* speed);
        if (Vector3.Distance(transform.localPosition, to) < threshold)
        {
            direction = !direction;
            transform.localPosition = direction? localStart * 0.75f : localEnd * 0.75f;
        }
    }
    /*
    private void SawMove()
    {
        gameObject.transform.DOlo
    }
    */
}
