using System;

public class GoalsEventManager
{
    public static Action OnUpMerge20Times;
    public static Action OnUpWatchAVideoAd;
    public static Action OnUpCollect1Block15InNormalMode;
    public static Action OnUpSpinTheWheelOfFortune;
    public static Action OnUpUseRevive1Time;
    public static Action OnUseTheClockOrSaw1Time;

    public static void SendUpMerge20Times()
    {
        if (OnUpMerge20Times != null) OnUpMerge20Times.Invoke();
    }
    public static void SendUpWatchAVideoAd()
    {
        if (OnUpWatchAVideoAd != null) OnUpWatchAVideoAd.Invoke();
    }
    public static void SendUpCollect1Block15InNormalMode()
    {
        if (OnUpCollect1Block15InNormalMode != null) OnUpCollect1Block15InNormalMode.Invoke();
    }
    public static void SendUpSpinTheWheelOfFortune()
    {
        if (OnUpSpinTheWheelOfFortune != null) OnUpSpinTheWheelOfFortune.Invoke();
    }
    public static void SendUpUseRevive1Time()
    {
        if (OnUpUseRevive1Time != null) OnUpUseRevive1Time.Invoke();
    }
    public static void SendUseTheClockOrSaw1Time()
    {
        if (OnUseTheClockOrSaw1Time != null) OnUseTheClockOrSaw1Time.Invoke();
    }
}
