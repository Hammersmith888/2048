using System;

public class GlobalEventManager
{
    public static Action TakeRoulette;


    // -------------- ����� ����������� �� ����� --------------


    // ��������� ������� � �����
    public static void SendTakeRoulette()
    {
        if (TakeRoulette != null) TakeRoulette.Invoke();
    }
}
