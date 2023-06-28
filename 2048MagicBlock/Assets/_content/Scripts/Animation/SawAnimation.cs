using UnityEngine;
using DG.Tweening;

//������ ������ �������� ������� (����) ����-����
public class SawAnimation : MonoBehaviour
{
    //������ ������� ��������
    [SerializeField]
    private Vector3 localStart;
    //������ ������� ��������
    [SerializeField]
    private Vector3 localEnd;
    //�������� ��������
    [SerializeField]
    private float speed = 1f;
    //����� (���������) ��� ������� �������� ����������� ��������
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
