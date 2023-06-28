using System;

public class GlobalEventManager
{
    public static Action TakeRoulette;


    // -------------- Вызов подписанных на ивент --------------


    // Получение рулетки с блока
    public static void SendTakeRoulette()
    {
        if (TakeRoulette != null) TakeRoulette.Invoke();
    }
}
