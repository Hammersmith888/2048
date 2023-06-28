using UnityEngine;
using System;
using System.Collections.Generic;
using OneSignalSDK;
using Assets.SimpleAndroidNotifications;

public class NotificationController : MonoBehaviour 
{
    private bool _isConnected = false;
    private bool _isStartCreatNotifications = false;

    private const string _id = "ad27d22a-5b88-45d2-a338-674260c6d762";
    private string _idUser = "";
    private void Start()
    {



        if (PlayerPrefs.HasKey("user_id") == false)
        {
            _idUser = SystemInfo.deviceUniqueIdentifier;
            PlayerPrefs.SetString("user_id", _idUser);
        }
        else
        {
            _idUser = PlayerPrefs.GetString("user_id");
        }
        OneSignal.Default.Initialize(_id);
        OneSignal.Default.SetExternalUserId(_idUser);

    }

	DateTime day2hour8;
	DateTime day2hour19;
	DateTime day3hour9;
	DateTime day3hour20;
	DateTime day4hour10;
	DateTime day4hour21;
	DateTime day5hour7;
	DateTime day5hour18;


	private const string TITLE = "Merge Blocks Magic Numbers";
	private const string FiveMinutesCounter = "FIVEMIN";
	private const string OneHourCounter = "ONEHOUR";

	private readonly List<string> MinutesMesseges = new List<string>()
{
	"⚡️ Попробуй побить свой рекорд!",
	"👉 Получи монеты за вход.",
	"⚡️ Try to beat your record!",
	"👉 Get coins for entry."
};
	private readonly List<string> FirstDayMesseges = new List<string>()
{
	"👉 Тук-тук Доброе утро!",
	"🎮 Эй приятель! Зайди и повеселись сейчас!",
	"👉 Knock Knock Good Morning!",
	"🎮 Hey mate! Сome and have some fun now!"
};
	private readonly List<string> SecondDayMesseges = new List<string>()
{
	"🎮  Хорошего понемножку, но наша игра радует всегда!😈",
	"👉 Новые ежедневные цели ждут вас!",
	"🎁 A little bit of good, but our game always pleases!😈",
	"⚡️New Daily Goals awaits you!"
};
	private readonly List<string> ThirdDayMesseges = new List<string>()
{
	"⚡️ Попробуй побить свой рекорд!",
	"👉 Получи монеты за вход.",
	"⚡️ Try to beat your record!",
	"👉 Get coins for entry."
};
	private readonly List<string> FourthDayMesseges = new List<string>()
{
	"👉 Тук-тук Доброе утро!",
	"🎮 Эй приятель! Зайди и повеселись сейчас!",
	"👉 Knock Knock Good Morning!",
	"🎮 Hey mate! Сome and have some fun now!"
};

	private void Awake()
	{
		PlayerPrefs.SetInt(FiveMinutesCounter, PlayerPrefs.GetInt(FiveMinutesCounter, -1) + 1);
		PlayerPrefs.SetInt(OneHourCounter, PlayerPrefs.GetInt(OneHourCounter, -1) + 1);
		Application.quitting += () =>
		{
			if (!_isStartCreatNotifications)
				StartNotifications();
		};
	}

	public void OnApplicationQuit()
	{
		if (!_isStartCreatNotifications)
			StartNotifications();
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			NotificationManager.CancelAll();
		}
		else
		{
			if (!_isStartCreatNotifications)
				StartNotifications();
		}
	}


	private void StartNotifications()
	{
		if (!_isStartCreatNotifications)
		{
			_isStartCreatNotifications = true;
			NotificationManager.CancelAll();
			day2hour8 = DateTime.Today.AddDays(1).AddHours(7).AddMinutes(1);
			day2hour19 = DateTime.Today.AddDays(1).AddHours(18).AddMinutes(1);
			day3hour9 = DateTime.Today.AddDays(2).AddHours(8).AddMinutes(1);
			day3hour20 = DateTime.Today.AddDays(2).AddHours(19).AddMinutes(1);
			day4hour10 = DateTime.Today.AddDays(3).AddHours(9).AddMinutes(1);
			day4hour21 = DateTime.Today.AddDays(3).AddHours(20).AddMinutes(1);
			day5hour7 = DateTime.Today.AddDays(4).AddHours(10).AddMinutes(1);
			day5hour18 = DateTime.Today.AddDays(4).AddHours(21).AddMinutes(1);

			if (Application.systemLanguage.ToString() == "Russian")
			{
				CreateNotification(TITLE, SecondDayMesseges[0], TimeSpan.FromMinutes((day2hour8 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, SecondDayMesseges[1], TimeSpan.FromMinutes((day2hour19 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, ThirdDayMesseges[0], TimeSpan.FromMinutes((day3hour9 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, ThirdDayMesseges[1], TimeSpan.FromMinutes((day3hour20 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, FourthDayMesseges[0], TimeSpan.FromMinutes((day4hour10 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, FourthDayMesseges[1], TimeSpan.FromMinutes((day4hour21 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, FirstDayMesseges[0], TimeSpan.FromMinutes((day5hour7 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, FirstDayMesseges[1], TimeSpan.FromMinutes((day5hour18 - DateTime.Now).TotalMinutes));

				if (PlayerPrefs.GetInt(FiveMinutesCounter, 0) % 10 == 0)
				{
					CreateNotification(TITLE, MinutesMesseges[0], TimeSpan.FromMinutes(5));
				}
				if (PlayerPrefs.GetInt(OneHourCounter, 0) % 5 == 0)
				{
					CreateNotification(TITLE, MinutesMesseges[1], TimeSpan.FromMinutes(60));
				}
				#region Old1
				/*for(int i = 10; i < 70; i+=15)
				{
					DateTime dayhourplan1 = DateTime.Today.AddDays(i).AddHours(8).AddMinutes(1);
					DateTime dayhourplan2 = DateTime.Today.AddDays(i).AddHours(19).AddMinutes(1);
					CreateNotification(TITLE, FirstDayMesseges[0], TimeSpan.FromMinutes((dayhourplan1 - DateTime.Now).TotalMinutes));
					CreateNotification(TITLE, FirstDayMesseges[1], TimeSpan.FromMinutes((dayhourplan2 - DateTime.Now).TotalMinutes));
					dayhourplan1 = DateTime.Today.AddDays(i+3).AddHours(8).AddMinutes(1);
					dayhourplan2 = DateTime.Today.AddDays(i+3).AddHours(19).AddMinutes(1);
					CreateNotification(TITLE, SecondDayMesseges[0], TimeSpan.FromMinutes((dayhourplan1 - DateTime.Now).TotalMinutes));
					CreateNotification(TITLE, SecondDayMesseges[1], TimeSpan.FromMinutes((dayhourplan2 - DateTime.Now).TotalMinutes));
					dayhourplan1 = DateTime.Today.AddDays(i+6).AddHours(8).AddMinutes(1);
					dayhourplan2 = DateTime.Today.AddDays(i+6).AddHours(19).AddMinutes(1);
					CreateNotification(TITLE, ThirdDayMesseges[0], TimeSpan.FromMinutes((dayhourplan1 - DateTime.Now).TotalMinutes));
					CreateNotification(TITLE, ThirdDayMesseges[1], TimeSpan.FromMinutes((dayhourplan2 - DateTime.Now).TotalMinutes));
					dayhourplan1 = DateTime.Today.AddDays(i+9).AddHours(8).AddMinutes(1);
					dayhourplan2 = DateTime.Today.AddDays(i+9).AddHours(19).AddMinutes(1);
					CreateNotification(TITLE, ThirdDayMesseges[0], TimeSpan.FromMinutes((dayhourplan1 - DateTime.Now).TotalMinutes));
					CreateNotification(TITLE, ThirdDayMesseges[1], TimeSpan.FromMinutes((dayhourplan2 - DateTime.Now).TotalMinutes));
					dayhourplan1 = DateTime.Today.AddDays(i+12).AddHours(8).AddMinutes(1);
					dayhourplan2 = DateTime.Today.AddDays(i+12).AddHours(19).AddMinutes(1);
					CreateNotification(TITLE, FourthDayMesseges[0], TimeSpan.FromMinutes((dayhourplan1 - DateTime.Now).TotalMinutes));
					CreateNotification(TITLE, FirstDayMesseges[1], TimeSpan.FromMinutes((dayhourplan2 - DateTime.Now).TotalMinutes));
				}*/
				#endregion
			}
			else
			{

				CreateNotification(TITLE, SecondDayMesseges[2], TimeSpan.FromMinutes((day2hour8 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, SecondDayMesseges[3], TimeSpan.FromMinutes((day2hour19 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, ThirdDayMesseges[2], TimeSpan.FromMinutes((day3hour9 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, ThirdDayMesseges[3], TimeSpan.FromMinutes((day3hour20 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, FourthDayMesseges[2], TimeSpan.FromMinutes((day4hour10 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, FourthDayMesseges[3], TimeSpan.FromMinutes((day4hour21 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, FirstDayMesseges[2], TimeSpan.FromMinutes((day5hour7 - DateTime.Now).TotalMinutes));
				CreateNotification(TITLE, FirstDayMesseges[3], TimeSpan.FromMinutes((day5hour18 - DateTime.Now).TotalMinutes));
				
				if (PlayerPrefs.GetInt(FiveMinutesCounter, 0) % 10 == 0)
				{
					CreateNotification(TITLE, MinutesMesseges[2], TimeSpan.FromMinutes(5));

				}
				if (PlayerPrefs.GetInt(OneHourCounter, 0) % 5 == 0)
				{
					CreateNotification(TITLE, MinutesMesseges[3], TimeSpan.FromMinutes(60));
				}
				#region Old
				/*for (int i = 10; i < 70; i = i + 15)
				{
					DateTime dayhourplan1 = DateTime.Today.AddDays(i).AddHours(8).AddMinutes(1);
					DateTime dayhourplan2 = DateTime.Today.AddDays(i).AddHours(19).AddMinutes(1);
					CreateNotification(TITLE, FirstDayMesseges[2], TimeSpan.FromMinutes((dayhourplan1 - DateTime.Now).TotalMinutes));
					CreateNotification(TITLE, FirstDayMesseges[3], TimeSpan.FromMinutes((dayhourplan2 - DateTime.Now).TotalMinutes));
					dayhourplan1 = DateTime.Today.AddDays(i + 3).AddHours(8).AddMinutes(1);
					dayhourplan2 = DateTime.Today.AddDays(i + 3).AddHours(19).AddMinutes(1);
					CreateNotification(TITLE, SecondDayMesseges[2], TimeSpan.FromMinutes((dayhourplan1 - DateTime.Now).TotalMinutes));
					CreateNotification(TITLE, SecondDayMesseges[3], TimeSpan.FromMinutes((dayhourplan2 - DateTime.Now).TotalMinutes));
					dayhourplan1 = DateTime.Today.AddDays(i + 6).AddHours(8).AddMinutes(1);
					dayhourplan2 = DateTime.Today.AddDays(i + 6).AddHours(19).AddMinutes(1);
					CreateNotification(TITLE, ThirdDayMesseges[2], TimeSpan.FromMinutes((dayhourplan1 - DateTime.Now).TotalMinutes));
					CreateNotification(TITLE, ThirdDayMesseges[3], TimeSpan.FromMinutes((dayhourplan2 - DateTime.Now).TotalMinutes));
					dayhourplan1 = DateTime.Today.AddDays(i + 9).AddHours(8).AddMinutes(1);
					dayhourplan2 = DateTime.Today.AddDays(i + 9).AddHours(19).AddMinutes(1);
					CreateNotification(TITLE, ThirdDayMesseges[2], TimeSpan.FromMinutes((dayhourplan1 - DateTime.Now).TotalMinutes));
					CreateNotification(TITLE, ThirdDayMesseges[3], TimeSpan.FromMinutes((dayhourplan2 - DateTime.Now).TotalMinutes));
					dayhourplan1 = DateTime.Today.AddDays(i + 12).AddHours(8).AddMinutes(1);
					dayhourplan2 = DateTime.Today.AddDays(i + 12).AddHours(19).AddMinutes(1);
					CreateNotification(TITLE, FourthDayMesseges[2], TimeSpan.FromMinutes((dayhourplan1 - DateTime.Now).TotalMinutes));
					CreateNotification(TITLE, FirstDayMesseges[3], TimeSpan.FromMinutes((dayhourplan2 - DateTime.Now).TotalMinutes));
				}*/
				#endregion
			}
			_isStartCreatNotifications = false;
		}
    }

	public void CreateNotification(string title, string body, TimeSpan time)
	{
		NotificationManager.SendWithAppIcon(time, title, body, new Color(0, 0.6f, 1), NotificationIcon.Bell);
	}
}
