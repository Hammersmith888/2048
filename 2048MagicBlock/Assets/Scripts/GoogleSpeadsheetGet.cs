using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class GoogleSpeadsheetGet : MonoBehaviour
{
	public enum QueryType
	{
		// Create
		createObject,       // Adds a row to a existing table type with the specified fields.
		createTable,        // Adds worksheet to the file, with the specified headers.
							// Retrieve
		getObjects,         // Returns an array of objects by field.
		getTable,           // Returns a complete table (worksheet) from the spreadsheet.
		getAllTables,       // Returns all worksheets on the spreadsheet.
							// Update
		updateObjects,      // Updates a field in one or more object(s) specified by field.
							// Delete
		deleteObjects       // Deletes object(s) specified by field.
	}

	public const string MSG_OBJ_CREATED_OK = "OBJ_CREATED_OK";
	public const string MSG_TBL_CREATED_OK = "TBL_CREATED_OK";
	public const string MSG_OBJ_DATA = "OBJ_DATA";
	public const string MSG_TBL_DATA = "TBL_DATA";
	public const string MSG_TBLS_DATA = "TBLS_DATA";
	public const string MSG_OBJ_UPDT = "OBJ_UPT_OK";
	public const string MSG_OBJ_DEL = "OBJ_DEL_OK";
	public const string MSG_MISS_PARAM = "MISSING_PARAM";
	public const string MSG_CONN_ERR = "CONN_ERROR";
	public const string MSG_TIME_OUT = "TIME_OUT";
	public const string TYPE_END = "_ENDTYPE\n";
	public const string TYPE_STRT = "TYPE_";
	public const string MSG_BAD_PASS = "PASS_ERROR";

	string currentStatus = "";

	private const string webServiceUrl = "https://script.google.com/macros/s/AKfycbzc93zTnjcRUtbqtlBtzJkEbkwhlMP8W-iuOnCn/exec";
	private const string spreadsheetId = "1FdPg0cN_rg8Coe8V7kBFWXeA3ecRI8wPcnP6enBIlgg"; // If this is a fixed value could also be setup on the webservice to save POST request size.
	private const string servicePassword = "bibizot2017";
	private const float timeOutLimit = 50f;
	private bool usePOST = true;

	private UnityWebRequest _www;
	private double _elapsedTime;
	private double _startTime;

	// Suscribe to this event if want to handle the response as it comes.
	public class CallBackEventRaw : UnityEvent<string> { }
	public static CallBackEventRaw rawResponseCallback = new CallBackEventRaw();

	public class CallBackEventProcessed : UnityEvent<QueryType, List<string>, List<string>> { }
	public CallBackEventProcessed processedResponseCallback = new CallBackEventProcessed();

	public event Action<QueryType, List<string>, List<string>> onGetData;
	public event Action<QueryType, List<string>, List<string>> onGetDataFromLocalData;
	public event Action errorGetData;

	private bool isCallbackRegistered;

	public static GoogleSpeadsheetGet instance;

	public event Action<float> onLoadingUpdate;

    private void Awake()
    {
		instance = this;
    }

    public void RetrieveParameters()
	{
		Debug.Log("Retrieve parameters from Google sheet..");
		if (!isCallbackRegistered)
		{
			isCallbackRegistered = true;
			processedResponseCallback.AddListener(SetOfflineBalanceVariables);
		}
		GetAllTables(true);
	}

	private void GetAllTables(bool runtime = true)
	{
		Dictionary<string, string> form = new Dictionary<string, string>();
		form.Add("action", QueryType.getAllTables.ToString());

		CreateRequest(form);
	}

	private void SetOfflineBalanceVariables(QueryType query, List<string> objTypeNames, List<string> jsonData)
	{
		_www = null;


		onGetData?.Invoke(query,objTypeNames,jsonData);
		//onGetDataFromLocalData?.Invoke(query,objTypeNames,jsonData);
	}

	private void CreateRequest(Dictionary<string, string> form)
	{
		form.Add("ssid", spreadsheetId);
		form.Add("pass", servicePassword);

		if (usePOST)
		{
			UpdateStatus("Establishing Connection at URL " + webServiceUrl);
			_www = UnityWebRequest.Post(webServiceUrl, form);
		}
		else // Use GET.
		{
			string urlParams = "?";
			foreach (KeyValuePair<string, string> item in form)
			{
				urlParams += item.Key + "=" + item.Value + "&";
			}
			UpdateStatus("Establishing Connection at URL " + webServiceUrl + urlParams);
			_www = UnityWebRequest.Get(webServiceUrl + urlParams);

		}

		_startTime = Time.timeSinceLevelLoad;
		_www.SendWebRequest();
	}

	private void Update()
    {
		if (_www == null)
			return;

		onLoadingUpdate?.Invoke(_www.downloadProgress);
		while (!_www.isDone)
		{
			onLoadingUpdate?.Invoke(_www.downloadProgress);
			_elapsedTime = Time.timeSinceLevelLoad - _startTime;
			if (_elapsedTime >= timeOutLimit)
			{
				Debug.Log("Time out");
				ProcessResponse("TIME_OUT", (float)_elapsedTime);
				errorGetData?.Invoke();
				_www = null;
			}
			return;
		}

		if (_www.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.Log("Connection error");
			ProcessResponse(MSG_CONN_ERR + "Connection error after " + _elapsedTime + " seconds: " + _www.error, (float)_elapsedTime);
			errorGetData?.Invoke();
			_www = null;
			return;
		}

		//System.IO.File.WriteAllText(Application.streamingAssetsPath + "/deb.txt", www.downloadHandler.text);

		ProcessResponse(_www.downloadHandler.text, (float)_elapsedTime);
		//
	}


	private void ProcessResponse(string response, float time)
	{
		// Optionally, developers can handle the response process themselves.
		rawResponseCallback.Invoke(response);
		//Debug.Log($"ProcessResponse: {response}");
		bool unpacked = false;

		if (response.StartsWith(MSG_OBJ_DATA))
		{
			UnpackJson(response);
			response = MSG_OBJ_DATA;
			unpacked = true;
		}

		if (response.StartsWith(MSG_TBL_DATA))
		{
			UnpackJson(response);
			response = MSG_TBL_DATA;
			unpacked = true;
		}

		if (response.StartsWith(MSG_TBLS_DATA))
		{
			UnpackJson(response);
			response = MSG_TBLS_DATA;
			unpacked = true;
		}

		if (response.StartsWith(MSG_BAD_PASS))
		{
			response = MSG_BAD_PASS;
		}

		string errorMsg = "Undefined connection error.";
		if (response.StartsWith(MSG_CONN_ERR))
		{
			errorMsg = response.Substring(MSG_CONN_ERR.Length);
			response = MSG_CONN_ERR;
		}

		string timeApendix = " Time: " + time.ToString();
		string logOutput = "";

		switch (response)
		{
			case MSG_OBJ_CREATED_OK:
				logOutput = "Object saved correctly.";
				break;

			case MSG_TBL_CREATED_OK:
				logOutput = "Worksheet table created correctly.";
				break;

			case MSG_OBJ_DATA:
				logOutput = "Object data received correctly.";
				break;

			case MSG_TBL_DATA:
				logOutput = "Table data received correctly.";
				break;

			case MSG_TBLS_DATA:
				logOutput = "All DB data received correctly.";
				break;

			case MSG_OBJ_UPDT:
				logOutput = "Object updated correctly.";
				break;

			case MSG_OBJ_DEL:
				logOutput = "Object deleted from DB.";
				break;

			case MSG_MISS_PARAM:
				logOutput = "Parsing Error: Missing parameters.";
				break;

			case MSG_TIME_OUT:
				logOutput = "Operation timed out, connection aborted. Check your internet connection and try again.";
				break;

			case MSG_CONN_ERR:
				logOutput = errorMsg;
				break;

			case MSG_BAD_PASS:
				logOutput = "Error: password incorrect.";
				break;

			default:
				logOutput = "Undefined server response: \n" + response; // verbose.
																		//logOutput = "Undefined server response.";
				break;
		}

		//Debug.Log($"------------- updating balance status: {logOutput + timeApendix}");

		UpdateStatus(logOutput + timeApendix);

		if (!unpacked)
			rawResponseCallback.Invoke(response);
	}

	private void UnpackJson(string response)
	{
		string smallerString = "TYPE_пуши_ENDTYPE\n";
		int index = response.IndexOf(smallerString);
		if (index != -1)
		{
			response = "TBLS_DATA\n"
			+ "TYPE_Task_ENDTYPE\n"
			+ "[{\"\":\"\",\"google док ГДД: \":\"\"}]\n" 
			+ "TYPE_пуши_ENDTYPE\n"
			+ response.Substring(index + smallerString.Length);
		}

		/*
		string filePath = Path.Combine(Application.streamingAssetsPath+"/googleDataGame" + ".txt");

		using (StreamWriter writer = File.AppendText(filePath))
		{
			// Write the text to the file
			writer.WriteLine(response);
			Debug.LogError("response--"+response);
		}*/
		
		PlayerPrefs.SetString("GoogleSheetResponseRequest", response);

		List<string> objTypeName = new List<string>();
		List<string> jsonData = new List<string>();
		string parsed = "";
		QueryType returnType = QueryType.getObjects;

		Debug.LogError("set in prefs new data google sheet");

		// Response for GetObjectsByField()
		if (response.StartsWith(MSG_OBJ_DATA))
		{
			parsed = response.Substring(MSG_OBJ_DATA.Length + 1);
			objTypeName.Add(parsed.Substring(0, parsed.IndexOf(TYPE_END)));
			jsonData.Add(parsed.Substring(parsed.IndexOf(TYPE_END) + TYPE_END.Length));
			returnType = QueryType.getObjects;
		}

		// Response for GetTable()
		if (response.StartsWith(MSG_TBL_DATA))
		{
			parsed = response.Substring(MSG_TBL_DATA.Length + 1);
			objTypeName.Add(parsed.Substring(0, parsed.IndexOf(TYPE_END)));
			jsonData.Add(parsed.Substring(parsed.IndexOf(TYPE_END) + TYPE_END.Length));
			returnType = QueryType.getTable;
		}

		// Response for GetAllTables()
		if (response.StartsWith(MSG_TBLS_DATA))
		{
			parsed = response.Substring(MSG_TBLS_DATA.Length + 1);

			// First split creates substrings containing type and content on each one.
			string[] separator = new string[] { TYPE_STRT };
			string[] split = parsed.Split(separator, System.StringSplitOptions.None);

			// Second split gives the final lists of type names and data on different lists.
			separator = new string[] { TYPE_END };
			for (int i = 0; i < split.Length; i++)
			{
				if (split[i] == "")
					continue;



				string[] secSplit = split[i].Split(separator, System.StringSplitOptions.None);
				objTypeName.Add(secSplit[0]);
				jsonData.Add(secSplit[1]);
				//Debug.Log(secSplit[0]);
				//Debug.Log(secSplit[1]);
				//Debug.Log("======");
			}
			returnType = QueryType.getAllTables;
		}

		// The callback returns:
		// * The return type, to identify which was the original request.
		// * An array of types names.
		// * An array of json strings (each string containing an array of objects).
		
		processedResponseCallback.Invoke(returnType, objTypeName, jsonData);
	}

	public void UnpackJsonLocalData()
	{
		string response = "TBLS_DATA\n"
			+ "TYPE_Task_ENDTYPE\n"
			+ "[{\"\":\"\",\"google док ГДД: \":\"\"}]\n"
			+ "TYPE_пуши_ENDTYPE\n"
			+ "[{\"\":\"1-st_push\",\"Merge\":\"РУ\n🎮 Эй приятель! Зайди и повеселись сейчас!\n👉 Тук-тук Доброе утро!\n\nАНГ\n🎮 Hey mate! Сome and have some fun now!\n👉 Knock Knock Good Morning!\",\"-Маленькие локал пуши:\nКаждый 10 выход спустя 5 минут\nКаждый пятый выход спустя 1 час\nНа второй день в 7:01\nНа второй день в 18:01\nНа третий день в 8:01\nНа третий день в 19:01\nНа четвертый день в 9:01\nНа четвертый день в 20:01\nНа пятый день в 10:01\nНа пятый день в 21:01\n\n-Большие пуши OneSignal\nВ обед. С 13 до 15 где-то. У меня вчера пришел без картинки, сегодняшний пуш попробую взять картинку не по нашей ссылке, а залить ее туда, куда предлагает onesignal\":\"\"},{\"\":\"2-nd_push\",\"Merge\":\"РУ\n⚡️ Попробуй побить свой рекорд!\n👉 Получи монеты за вход.\n\nАНГ\n⚡️ Try to beat your record!\n👉 Get coins for entry.\",\"-Маленькие локал пуши:\nКаждый 10 выход спустя 5 минут\nКаждый пятый выход спустя 1 час\nНа второй день в 7:01\nНа второй день в 18:01\nНа третий день в 8:01\nНа третий день в 19:01\nНа четвертый день в 9:01\nНа четвертый день в 20:01\nНа пятый день в 10:01\nНа пятый день в 21:01\n\n-Большие пуши OneSignal\nВ обед. С 13 до 15 где-то. У меня вчера пришел без картинки, сегодняшний пуш попробую взять картинку не по нашей ссылке, а залить ее туда, куда предлагает onesignal\":\"\"},{\"\":\"3-th_push\",\"Merge\":\"РУ\n🎮  Хорошего понемножку, но наша игра радует всегда!😈\n👉 Новые ежедневные цели ждут вас!\n\nАНГ\n🎁 A little bit of good, but our game always pleases!😈\n⚡️New Daily Goals awaits you!\",\"-Маленькие локал пуши:\nКаждый 10 выход спустя 5 минут\nКаждый пятый выход спустя 1 час\nНа второй день в 7:01\nНа второй день в 18:01\nНа третий день в 8:01\nНа третий день в 19:01\nНа четвертый день в 9:01\nНа четвертый день в 20:01\nНа пятый день в 10:01\nНа пятый день в 21:01\n\n-Большие пуши OneSignal\nВ обед. С 13 до 15 где-то. У меня вчера пришел без картинки, сегодняшний пуш попробую взять картинку не по нашей ссылке, а залить ее туда, куда предлагает onesignal\":\"\"}]\n"
			+ "TYPE_Локализация_ENDTYPE\n"
			+ "[{\"_id\":\"id\",\"\":\"ref\",\"_ru\":\"RU\",\"_en\":\"EN\",\"_ro\":\"RO\",\"_fr\":\"FR\",\"_de\":\"DE\",\"_sp\":\"SP\",\"_it\":\"IT\",\"_jp\":\"JP\",\"_ko\":\"KO\",\"_tr\":\"TR\"},{\"_id\":\"t_001\",\"\":\"1 - http://joxi.ru/EA4JNGMTXlK8Vm\",\"_ru\":\"Игра окончена\",\"_en\":\"Game over\",\"_ro\":\"Joc încheiat\",\"_fr\":\"Jeu terminé\",\"_de\":\"Spiel ist aus\",\"_sp\":\"Juego terminado\",\"_it\":\"Game Over\",\"_jp\":\"ゲームオーバー\",\"_ko\":\"게임 끝\",\"_tr\":\"Oyun bitti\"},{\"_id\":\"t_002\",\"\":\"2 - http://joxi.ru/EA4JNGMTXlK8Vmm\",\"_ru\":\"Заново\",\"_en\":\"Retry\",\"_ro\":\"Din nou\",\"_fr\":\"Recommencez\",\"_de\":\"Wiederholen\",\"_sp\":\"Rever\",\"_it\":\"Riprovare\",\"_jp\":\"リトライ\",\"_ko\":\"다시 해 보다\",\"_tr\":\"Tekrarlamak\"},{\"_id\":\"t_003\",\"\":\"1 - http://joxi.ru/4AkV3DbUjJ5qD2\",\"_ru\":\"Разблокирован усилитель\",\"_en\":\"Booster unlocked\",\"_ro\":\"Booster deblocat\",\"_fr\":\"Booster déverrouillé\",\"_de\":\"Booster freigeschaltet\",\"_sp\":\"Refuerzo desbloqueado\",\"_it\":\"Booster sbloccato\",\"_jp\":\"ブースターロックが解除されました\",\"_ko\":\"부스터 잠금 해제\",\"_tr\":\"Booster kilidini açmış\"},{\"_id\":\"t_004\",\"\":\"2 - http://joxi.ru/4AkV3DbUjJ5qD2\",\"_ru\":\"Пила\",\"_en\":\"Saw\",\"_ro\":\"Fierăstrău\",\"_fr\":\"Scie\",\"_de\":\"Säge\",\"_sp\":\"Serruchar\",\"_it\":\"Sega\",\"_jp\":\"見た\",\"_ko\":\"봤다\",\"_tr\":\"Testere\"},{\"_id\":\"t_005\",\"\":\"3 - http://joxi.ru/4AkV3DbUjJ5qD2\",\"_ru\":\"Разрушает связи между блоками\",\"_en\":\"Break links between blocks\",\"_ro\":\"Spargeți legăturile între blocuri\",\"_fr\":\"Rompre les liens entre les blocs\",\"_de\":\"Links zwischen Blöcken brechen\",\"_sp\":\"Romper los enlaces entre bloques\",\"_it\":\"Rompi i collegamenti tra i blocchi\",\"_jp\":\"ブロック間のリンクを破壊します\",\"_ko\":\"블록 사이의 링크를 중단합니다\",\"_tr\":\"Bloklar arasındaki bağlantıları kırın\"},{\"_id\":\"t_006\",\"\":\"4 - http://joxi.ru/4AkV3DbUjJ5qD2\",\"_ru\":\"ОК\",\"_en\":\"OK\",\"_ro\":\"Bine\",\"_fr\":\"D'ACCORD\",\"_de\":\"OK\",\"_sp\":\"DE ACUERDO\",\"_it\":\"OK\",\"_jp\":\"OK\",\"_ko\":\"좋아요\",\"_tr\":\"TAMAM\"},{\"_id\":\"t_007\",\"\":\" http://joxi.ru/v290QzZC4g9QKm\",\"_ru\":\"Перетягивайте и соединяйте блоки с одинаковым значением\",\"_en\":\"Drag and drop tiles with same value\",\"_ro\":\"Trageți și aruncați plăci cu aceeași valoare\",\"_fr\":\"Faire glisser et déposer les carreaux avec la même valeur\",\"_de\":\"Ziehen- und Dropfliesen mit gleichem Wert\",\"_sp\":\"Arrastre y suelte los azulejos con el mismo valor\",\"_it\":\"Trascinare e rilasciare le piastrelle con lo stesso valore\",\"_jp\":\"同じ値でタイルをドラッグアンドドロップします\",\"_ko\":\"동일한 값으로 타일을 드래그 앤 드롭합니다\",\"_tr\":\"Aynı değerde karoları sürükleyin ve bırakın\"},{\"_id\":\"t_008\",\"\":\"http://joxi.ru/MAjv0DKUdNEb7A\",\"_ru\":\"Отлично!\",\"_en\":\"Great!\",\"_ro\":\"Grozav!\",\"_fr\":\"Super!\",\"_de\":\"Großartig!\",\"_sp\":\"¡Excelente!\",\"_it\":\"Grande!\",\"_jp\":\"素晴らしい！\",\"_ko\":\"엄청난!\",\"_tr\":\"Harika!\"},{\"_id\":\"t_009\",\"\":\"http://joxi.ru/xAepGwdcM47N02\",\"_ru\":\"Тап для старта\",\"_en\":\"Tap to play\",\"_ro\":\"Apăsați pentru a juca\",\"_fr\":\"Taper pour jouer\",\"_de\":\"Berühre um zu spielen\",\"_sp\":\"Toque para jugar\",\"_it\":\"Tocca per giocare\",\"_jp\":\"タップして再生します\",\"_ko\":\"플레이하려면 탭합니다\",\"_tr\":\"Oynamak İçin Dokunun\"},{\"_id\":\"t_010\",\"\":\"1 - http://joxi.ru/l2Z7vGBHljO4VA\",\"_ru\":\"Часы\",\"_en\":\"Clock\",\"_ro\":\"Ceas\",\"_fr\":\"Horloge\",\"_de\":\"Uhr\",\"_sp\":\"Reloj\",\"_it\":\"Orologio\",\"_jp\":\"時計\",\"_ko\":\"시계\",\"_tr\":\"Saat\"},{\"_id\":\"t_011\",\"\":\"2 - http://joxi.ru/l2Z7vGBHljO4VA\",\"_ru\":\"Заполняет шкалу времени\",\"_en\":\"Fill-up the timer\",\"_ro\":\"Completați cronometrul\",\"_fr\":\"Remplissez la minuterie\",\"_de\":\"Füllen Sie den Timer auf\",\"_sp\":\"Llenar el temporizador\",\"_it\":\"Riempire il timer\",\"_jp\":\"タイマーを埋めます\",\"_ko\":\"타이머를 채우십시오\",\"_tr\":\"Zamanlayıcıyı doldurun\"},{\"_id\":\"t_012\",\"\":\"1 - http://joxi.ru/brR3Dy4SBqMgem\",\"_ru\":\"Коллекция\",\"_en\":\"Collection\",\"_ro\":\"Colectie\",\"_fr\":\"Collection\",\"_de\":\"Sammlung\",\"_sp\":\"Recopilación\",\"_it\":\"Collezione\",\"_jp\":\"コレクション\",\"_ko\":\"수집\",\"_tr\":\"Toplamak\"},{\"_id\":\"t_013\",\"\":\"2 - http://joxi.ru/brR3Dy4SBqMgem\",\"_ru\":\"Темы\",\"_en\":\"Themes\",\"_ro\":\"Teme\",\"_fr\":\"Thèmes\",\"_de\":\"Themen\",\"_sp\":\"Temas\",\"_it\":\"Temi\",\"_jp\":\"テーマ\",\"_ko\":\"테마\",\"_tr\":\"Temalar\"},{\"_id\":\"t_014\",\"\":\"3 - http://joxi.ru/brR3Dy4SBqMgem\",\"_ru\":\"Блоки\",\"_en\":\"Blocks\",\"_ro\":\"Blocuri\",\"_fr\":\"Blocs\",\"_de\":\"Blöcke\",\"_sp\":\"Bloques\",\"_it\":\"Blocchi\",\"_jp\":\"ブロック\",\"_ko\":\"블록\",\"_tr\":\"Bloklar\"},{\"_id\":\"t_015\",\"\":\"4 - http://joxi.ru/brR3Dy4SBqMgem\",\"_ru\":\"Наборы\",\"_en\":\"Sets\",\"_ro\":\"Seturi\",\"_fr\":\"Sets\",\"_de\":\"Sets\",\"_sp\":\"Sets\",\"_it\":\"Imposta\",\"_jp\":\"セット\",\"_ko\":\"세트\",\"_tr\":\"Setler\"},{\"_id\":\"t_016\",\"\":\"5 - http://joxi.ru/brR3Dy4SBqMgem\",\"_ru\":\"Эффекты\",\"_en\":\"Effects\",\"_ro\":\"Efecte\",\"_fr\":\"Effets\",\"_de\":\"Auswirkungen\",\"_sp\":\"Efectos\",\"_it\":\"Effetti\",\"_jp\":\"効果\",\"_ko\":\"효과\",\"_tr\":\"Etkileri\"},{\"_id\":\"t_017\",\"\":\"6 - http://joxi.ru/brR3Dy4SBqMgem\",\"_ru\":\"Открыть\",\"_en\":\"Open\",\"_ro\":\"Deschis\",\"_fr\":\"Ouvrir\",\"_de\":\"Offen\",\"_sp\":\"Abierto\",\"_it\":\"Aprire\",\"_jp\":\"開ける\",\"_ko\":\"열려 있는\",\"_tr\":\"Açık\"},{\"_id\":\"t_018\",\"\":\"1 - http://joxi.ru/5mdlkV8UqlY3lm\",\"_ru\":\"Колесо фортуны\",\"_en\":\"Fortune wheel\",\"_ro\":\"Roată de avere\",\"_fr\":\"Roue de fortune\",\"_de\":\"Glücksrad\",\"_sp\":\"Rueda de la fortuna\",\"_it\":\"Ruota della fortuna\",\"_jp\":\"フォーチュンホイール\",\"_ko\":\"재산 휠\",\"_tr\":\"Servet tekerleği\"},{\"_id\":\"t_019\",\"\":\"2 - http://joxi.ru/5mdlkV8UqlY3lm\",\"_ru\":\"Остановить\",\"_en\":\"Stop\",\"_ro\":\"Stop\",\"_fr\":\"Arrêt\",\"_de\":\"Stoppen\",\"_sp\":\"Detener\",\"_it\":\"Fermare\",\"_jp\":\"ストップ\",\"_ko\":\"멈추다\",\"_tr\":\"Durmak\"},{\"_id\":\"t_020\",\"\":\"1 - http://joxi.ru/LmGYVKxcBOvldA\",\"_ru\":\"Превосходно!\",\"_en\":\"Amazing!\",\"_ro\":\"Uimitor!\",\"_fr\":\"Incroyable!\",\"_de\":\"Toll!\",\"_sp\":\"¡Asombroso!\",\"_it\":\"Sorprendente!\",\"_jp\":\"すばらしい！\",\"_ko\":\"놀라운!\",\"_tr\":\"İnanılmaz!\"},{\"_id\":\"t_021\",\"\":\"2 - http://joxi.ru/LmGYVKxcBOvldA\",\"_ru\":\"Далее\",\"_en\":\"Next\",\"_ro\":\"Următorul\",\"_fr\":\"Suivant\",\"_de\":\"Nächste\",\"_sp\":\"Próximo\",\"_it\":\"Prossimo\",\"_jp\":\"次\",\"_ko\":\"다음\",\"_tr\":\"Sonraki\"},{\"_id\":\"t_022\",\"\":\"http://joxi.ru/Vm6gyE0t3NQKR2\",\"_ru\":\"Воскреснуть\",\"_en\":\"Revive\",\"_ro\":\"Reînvie\",\"_fr\":\"Relancer\",\"_de\":\"Beleben\",\"_sp\":\"Reanimar\",\"_it\":\"RIVENDI\",\"_jp\":\"復活します\",\"_ko\":\"부활\",\"_tr\":\"Canlandırmak\"},{\"_id\":\"t_023\",\"\":\"1 - http://joxi.ru/MAjv0DKUdNwoEA\",\"_ru\":\"Стать VIP\",\"_en\":\"Become a VIP\",\"_ro\":\"Deveniți VIP\",\"_fr\":\"Devenir VIP\",\"_de\":\"Ein VIP werden\",\"_sp\":\"Convertirse en un VIP\",\"_it\":\"Diventare un VIP\",\"_jp\":\"VIPになります\",\"_ko\":\"VIP가 되십시오\",\"_tr\":\"VIP Olun\"},{\"_id\":\"t_024\",\"\":\"2 - http://joxi.ru/MAjv0DKUdNwoEA\",\"_ru\":\"Новинка\",\"_en\":\"new\",\"_ro\":\"nou\",\"_fr\":\"nouveau\",\"_de\":\"neu\",\"_sp\":\"nuevo\",\"_it\":\"nuovo\",\"_jp\":\"新しい\",\"_ko\":\"새로운\",\"_tr\":\"yeni\"},{\"_id\":\"t_025\",\"\":\"3 - http://joxi.ru/MAjv0DKUdNwoEA\",\"_ru\":\"Бесплатные жизни в каждой игре\",\"_en\":\"Free lives for every game\",\"_ro\":\"Vieți liberi pentru fiecare joc\",\"_fr\":\"Vie libre pour chaque jeu\",\"_de\":\"Kostenloses Leben für jedes Spiel\",\"_sp\":\"Vidas gratis para cada juego\",\"_it\":\"Vite gratuite per ogni gioco\",\"_jp\":\"すべてのゲームの自由生活\",\"_ko\":\"모든 게임에 대한 자유 생활\",\"_tr\":\"Her oyun için ücretsiz hayat\"},{\"_id\":\"t_026\",\"\":\"4 - http://joxi.ru/MAjv0DKUdNwoEA\",\"_ru\":\"Удаление межстраничной и баннерной рекламы\",\"_en\":\"Remove interstitial & banner ads\",\"_ro\":\"Eliminați anunțurile interstițiale și banner\",\"_fr\":\"Supprimer les annonces interstitives et de bannières\",\"_de\":\"Entfernen Sie interstitielle und Banneranzeigen\",\"_sp\":\"Eliminar anuncios intersticiales y banner\",\"_it\":\"Rimuovi gli annunci interstiziali e banner\",\"_jp\":\"Interstitial＆Banner広告を削除します\",\"_ko\":\"간질 및 배너 광고를 제거하십시오\",\"_tr\":\"İnterstisyel ve Banner reklamlarını kaldırın\"},{\"_id\":\"t_027\",\"\":\"5 - http://joxi.ru/MAjv0DKUdNwoEA\",\"_ru\":\"Старт с больших чисел\",\"_en\":\"Start from high number\",\"_ro\":\"Începeți de la un număr mare\",\"_fr\":\"Commencez par un nombre élevé\",\"_de\":\"Beginnen Sie mit hoher Anzahl\",\"_sp\":\"Comience desde un número alto\",\"_it\":\"Inizia da un numero elevato\",\"_jp\":\"高い数から始めます\",\"_ko\":\"높은 숫자에서 시작하십시오\",\"_tr\":\"Yüksek sayıdan başlayın\"},{\"_id\":\"t_028\",\"\":\"6 - http://joxi.ru/MAjv0DKUdNwoEA\",\"_ru\":\"x5 бесплатных круток ежедневно\",\"_en\":\"x5 Free spin everyday\",\"_ro\":\"x5 spin gratuit în fiecare zi\",\"_fr\":\"x5 Spin gratuit tous les jours\",\"_de\":\"X5 Free Spin jeden Tag\",\"_sp\":\"x5 giro gratis todos los días\",\"_it\":\"X5 Spin gratuito ogni giorno\",\"_jp\":\"x5毎日無料スピン\",\"_ko\":\"X5 무료 스핀 매일\",\"_tr\":\"x5 her gün ücretsiz spin\"},{\"_id\":\"t_029\",\"\":\"7 - http://joxi.ru/MAjv0DKUdNwoEA\",\"_ru\":\"Ежегодно\",\"_en\":\"Yearly\",\"_ro\":\"Anual\",\"_fr\":\"Annuel\",\"_de\":\"Jährlich\",\"_sp\":\"Anual\",\"_it\":\"Annuale\",\"_jp\":\"毎年\",\"_ko\":\"매년\",\"_tr\":\"Yıllık\"},{\"_id\":\"t_030\",\"\":\"8 - http://joxi.ru/MAjv0DKUdNwoEA\",\"_ru\":\"Ежемесячно\",\"_en\":\"Monthly\",\"_ro\":\"Lunar\",\"_fr\":\"Mensuel\",\"_de\":\"Monatlich\",\"_sp\":\"Mensual\",\"_it\":\"Mensile\",\"_jp\":\"毎月\",\"_ko\":\"월간 간행물\",\"_tr\":\"Aylık\"},{\"_id\":\"t_031\",\"\":\"9 - http://joxi.ru/MAjv0DKUdNwoEA\",\"_ru\":\"Еженедельно\",\"_en\":\"Weekly\",\"_ro\":\"Săptămânal\",\"_fr\":\"Hebdomadaire\",\"_de\":\"Wöchentlich\",\"_sp\":\"Semanalmente\",\"_it\":\"settimanalmente\",\"_jp\":\"毎週\",\"_ko\":\"주간\",\"_tr\":\"Haftalık\"},{\"_id\":\"t_032\",\"\":\"10 - http://joxi.ru/MAjv0DKUdNwoEA\",\"_ru\":\"Срок действия подписки\",\"_en\":\"Subscription term\",\"_ro\":\"Termen de abonament\",\"_fr\":\"Terme d'abonnement\",\"_de\":\"Abonnementbegriff\",\"_sp\":\"Término de suscripción\",\"_it\":\"Termine di abbonamento\",\"_jp\":\"サブスクリプション用語\",\"_ko\":\"구독 용어\",\"_tr\":\"Abonelik süresi\"},{\"_id\":\"t_033\",\"\":\"11 - http://joxi.ru/MAjv0DKUdNwoEA\",\"_ru\":\"После покупки этой подписки вы получите: бесплатные продолжения при проигрышах, возможность начать игру с больших чисел, 5 бесплатных круток колеса фортуны ежедневно, удаление межстраничной и баннерной рекламы. Это - Автоматически продлеваемая подписка. Подписками можно управлять и автоматическое продление можно отключить через Google Play Store > вкладка Меню > Подписки.\",\"_en\":\"After buying this subscription, you will receive: free continue every game over, start from high number, 5 times free spin remove interstitial & banner ads. This is an Auto-renewable subscription. Subscriptions can be managed and auto-renewal may be turned off via Google Play Store > Menu tab > Subscriptions\",\"_ro\":\"După ce ați cumpărat acest abonament, veți primi: Continuați gratuit fiecare joc, începeți de la număr mare, de 5 ori gratuit spin eliminați anunțurile interstițiale și banner. Acesta este un abonament auto-regenerabil. Abonamentele pot fi gestionate și reînnoirea automată poate fi oprită prin Google Play Store> Fila Meniu> Abonamente\",\"_fr\":\"Après avoir acheté cet abonnement, vous recevrez: Continuez gratuitement à chaque jeu, commencez à partir d'un nombre élevé, 5 fois gratuit Spin Retirez les publicités interstitives et bannières. Il s'agit d'un abonnement auto-renouvelable. Les abonnements peuvent être gérés et le renouvellement automatique peut être désactivé via Google Play Store> Onglet Menu> Abonnements\",\"_de\":\"Nach dem Kauf dieses Abonnements erhalten Sie: KOSTENLOSE FORET JEDES SPIEL, FOHMEN SIE VON UNTERSCHIEDEN, FÜNFSTELLE FREE Spin Entfernen Sie Interstitial- und Banner -Anzeigen. Dies ist ein automatisch erneuerbares Abonnement. Abonnements können verwaltet und automatisch übernommen werden, können über Google Play Store> Menü-Registerkarte> Abonnements ausgeschaltet werden\",\"_sp\":\"Después de comprar esta suscripción, recibirá: Continúe gratis en cada juego, comience desde un número alto, 5 veces gratis, elimine los anuncios intersticiales y de banner. Esta es una suscripción auto-renovable. Las suscripciones se pueden administrar y la renovación automática se puede desactivar a través de Google Play Store> Menú pestaña> suscripciones\",\"_it\":\"Dopo aver acquistato questo abbonamento, riceverai: Continua gratuitamente ogni partita, ricomincia da un numero elevato, 5 volte gratuito Rimuovi gli annunci interstiziali e banner. Questo è un abbonamento a rinnovabile auto. Gli abbonamenti possono essere gestiti e il rinnovo automatico può essere disattivato tramite Google Play Store> Scheda del menu> Abbonamenti\",\"_jp\":\"このサブスクリプションを購入した後、あなたは次のように受け取ります：無料ゲームを続け、高い数から始め、5回のフリースピンはインタースティアルおよびバナー広告を削除します。これは自動再生可能なサブスクリプションです。サブスクリプションは管理でき、Auto-RenewalはGoogle Playストア> [メニュー]タブ> [サブスクリプション]を介してオフにすることができます\",\"_ko\":\"이 구독을 구매 한 후에는 다음과 같이 받게됩니다. 무료 모든 게임 오버를 무료로 계속하고, 5 배 무료 스핀 interstitial & banner 광고에서 5 배에서 시작합니다. 이것은 자동 갱신 가능한 구독입니다. 구독을 관리 할 수 ​​있고 Google Play 스토어> 메뉴 탭> 구독을 통해 자동 갱신을 해제 할 수 있습니다.\",\"_tr\":\"Bu aboneliği satın aldıktan sonra: Ücretsiz her oyuna devam edin, yüksek sayıdan başlayın, 5 kat ücretsiz spin interstitial ve banner reklamlarını kaldırın. Bu otomatik olarak yenilenebilir bir aboneliktir. Abonelikler yönetilebilir ve otomatik yenileme Google Play Store> Menü sekmesi> Abonelikler aracılığıyla kapatılabilir\"},{\"_id\":\"t_034\",\"\":\"1 - http://joxi.ru/gmvLRDoHejwgMA\",\"_ru\":\"Получить VIP\",\"_en\":\"Get VIP\",\"_ro\":\"Ia VIP\",\"_fr\":\"Obtenir VIP\",\"_de\":\"Holen Sie sich VIP\",\"_sp\":\"Obtener VIP\",\"_it\":\"Ottieni VIP\",\"_jp\":\"VIPを取得します\",\"_ko\":\"VIP를 얻으십시오\",\"_tr\":\"VIP Al\"},{\"_id\":\"t_035\",\"\":\"2 - http://joxi.ru/gmvLRDoHejwgMA\",\"_ru\":\"от\",\"_en\":\"from\",\"_ro\":\"din\",\"_fr\":\"depuis\",\"_de\":\"aus\",\"_sp\":\"de\",\"_it\":\"da\",\"_jp\":\"から\",\"_ko\":\"~에서\",\"_tr\":\"itibaren\"},{\"_id\":\"t_036\",\"\":\"3 - http://joxi.ru/gmvLRDoHejwgMA\",\"_ru\":\"Возобновить\",\"_en\":\"Resume\",\"_ro\":\"Relua\",\"_fr\":\"CV\",\"_de\":\"Fortsetzen\",\"_sp\":\"Reanudar\",\"_it\":\"Riprendere\",\"_jp\":\"履歴書\",\"_ko\":\"재개하다\",\"_tr\":\"Sürdürmek\"},{\"_id\":\"t_037\",\"\":\"4 - http://joxi.ru/gmvLRDoHejwgMA\",\"_ru\":\"Заново\",\"_en\":\"Restart\",\"_ro\":\"Repornire\",\"_fr\":\"Redémarrage\",\"_de\":\"Neu starten\",\"_sp\":\"Reanudar\",\"_it\":\"Ricomincia\",\"_jp\":\"再起動\",\"_ko\":\"재시작\",\"_tr\":\"Tekrar başlat\"},{\"_id\":\"t_038\",\"\":\"5 - http://joxi.ru/gmvLRDoHejwgMA\",\"_ru\":\"Обучение\",\"_en\":\"Tutorial\",\"_ro\":\"Tutorial\",\"_fr\":\"Didacticiel\",\"_de\":\"Lernprogramm\",\"_sp\":\"Tutorial\",\"_it\":\"Tutorial\",\"_jp\":\"チュートリアル\",\"_ko\":\"지도 시간\",\"_tr\":\"Öğretici\"},{\"_id\":\"t_039\",\"\":\"1 - http://joxi.ru/GrqQMDotzBw5Dr\",\"_ru\":\"Пропустить\",\"_en\":\"Skip\",\"_ro\":\"Ocolire\",\"_fr\":\"Sauter\",\"_de\":\"Überspringen\",\"_sp\":\"Saltar\",\"_it\":\"Saltare\",\"_jp\":\"スキップ\",\"_ko\":\"건너뛰다\",\"_tr\":\"Atlamak\"},{\"_id\":\"t_039\",\"\":\"2 - http://joxi.ru/GrqQMDotzBw5Dr\",\"_ru\":\"Перемещайте мешающие кубы для объединения других\",\"_en\":\"Move tile out of the way to make a match\",\"_ro\":\"Mutați țiglă din drum pentru a face un meci\",\"_fr\":\"Déplacez les carreaux à l'écart pour faire un match\",\"_de\":\"Bewegen Sie die Fliese aus dem Weg, um ein Match zu machen\",\"_sp\":\"Mueva el azulejo fuera del camino para hacer un partido\",\"_it\":\"Sposta le piastrelle per fare una partita\",\"_jp\":\"邪魔にならないようにタイルを動かす\",\"_ko\":\"일치하는 길에서 타일을 옮기십시오.\",\"_tr\":\"Bir eşleşme yapmak için fayans yolundan uzaklaşın\"},{\"_id\":\"t_040\",\"\":\"http://joxi.ru/DrlwaDnhKqe75A\",\"_ru\":\"А сможешь добраться до\",\"_en\":\"Can you merge to\",\"_ro\":\"Poți să te contopiți\",\"_fr\":\"Pouvez-vous fusionner\",\"_de\":\"Kannst du dich verschmelzen?\",\"_sp\":\"¿Puedes fusionarte con\",\"_it\":\"Puoi unire a\",\"_jp\":\"合併できますか\",\"_ko\":\"합병 할 수 있습니까?\",\"_tr\":\"Birleşebilir misin\"},{\"_id\":\"t_041\",\"\":\"1 - http://joxi.ru/8An4XYvCNdn7jm\",\"_ru\":\"Игра окончена\",\"_en\":\"Game over\",\"_ro\":\"Joc încheiat\",\"_fr\":\"Jeu terminé\",\"_de\":\"Spiel ist aus\",\"_sp\":\"Juego terminado\",\"_it\":\"Game Over\",\"_jp\":\"ゲームオーバー\",\"_ko\":\"게임 끝\",\"_tr\":\"Oyun bitti\"},{\"_id\":\"t_042\",\"\":\"2 - http://joxi.ru/8An4XYvCNdn7jm\",\"_ru\":\"Очки\",\"_en\":\"Score\",\"_ro\":\"Scor\",\"_fr\":\"Score\",\"_de\":\"Punktzahl\",\"_sp\":\"Puntaje\",\"_it\":\"Punto\",\"_jp\":\"スコア\",\"_ko\":\"점수\",\"_tr\":\"Gol\"},{\"_id\":\"t_043\",\"\":\"3 - http://joxi.ru/8An4XYvCNdn7jm\",\"_ru\":\"Лучший результат\",\"_en\":\"Best\",\"_ro\":\"Cel mai bun\",\"_fr\":\"Meilleur\",\"_de\":\"Am besten\",\"_sp\":\"Mejor\",\"_it\":\"Migliore\",\"_jp\":\"一番\",\"_ko\":\"최상의\",\"_tr\":\"En iyi\"},{\"_id\":\"t_044\",\"\":\"4 - http://joxi.ru/8An4XYvCNdn7jm\",\"_ru\":\"Заново\",\"_en\":\"Restart\",\"_ro\":\"Repornire\",\"_fr\":\"Redémarrage\",\"_de\":\"Neu starten\",\"_sp\":\"Reanudar\",\"_it\":\"Ricomincia\",\"_jp\":\"再起動\",\"_ko\":\"재시작\",\"_tr\":\"Tekrar başlat\"},{\"_id\":\"t_045\",\"\":\"1 - http://joxi.ru/VrwKoDOToLaOWr\",\"_ru\":\"Ежедневная награда\",\"_en\":\"Daily reward\",\"_ro\":\"Recompensa zilnică\",\"_fr\":\"Récompense quotidienne\",\"_de\":\"Tägliche Belohnung\",\"_sp\":\"Recompensa diaria\",\"_it\":\"Ricompensa quotidiana\",\"_jp\":\"毎日の報酬\",\"_ko\":\"매일 보상\",\"_tr\":\"Günlük ödül\"},{\"_id\":\"t_046\",\"\":\"2 - http://joxi.ru/VrwKoDOToLaOWr\",\"_ru\":\"Заходите каждый день чтобы не пропустить!\",\"_en\":\"Visit each day so you don't miss out!\",\"_ro\":\"Vizitați în fiecare zi, astfel încât să nu ratați!\",\"_fr\":\"Visitez chaque jour pour ne pas manquer!\",\"_de\":\"Besuchen Sie jeden Tag, damit Sie nicht verpassen!\",\"_sp\":\"¡Visite todos los días para que no te lo pierdas!\",\"_it\":\"Visita ogni giorno in modo da non perdere!\",\"_jp\":\"毎日訪れて、お見逃しなく！\",\"_ko\":\"당신이 놓치지 않도록 매일 방문하십시오!\",\"_tr\":\"Kaçırmamak için her günü ziyaret edin!\"},{\"_id\":\"t_047\",\"\":\"3 - http://joxi.ru/VrwKoDOToLaOWr\",\"_ru\":\"Сегодня\",\"_en\":\"Today\",\"_ro\":\"Astăzi\",\"_fr\":\"Aujourd'hui\",\"_de\":\"Heute\",\"_sp\":\"Hoy\",\"_it\":\"Oggi\",\"_jp\":\"今日\",\"_ko\":\"오늘\",\"_tr\":\"Bugün\"},{\"_id\":\"t_048\",\"\":\"4 - http://joxi.ru/VrwKoDOToLaOWr\",\"_ru\":\"День\",\"_en\":\"Day\",\"_ro\":\"Zi\",\"_fr\":\"Jour\",\"_de\":\"Tag\",\"_sp\":\"Día\",\"_it\":\"Giorno\",\"_jp\":\"日\",\"_ko\":\"낮\",\"_tr\":\"Gün\"},{\"_id\":\"t_049\",\"\":\"5 - http://joxi.ru/VrwKoDOToLaOWr\",\"_ru\":\"Монеты\",\"_en\":\"Coins\",\"_ro\":\"Monede\",\"_fr\":\"Pièces de monnaie\",\"_de\":\"Münzen\",\"_sp\":\"Monedas\",\"_it\":\"Monete\",\"_jp\":\"コイン\",\"_ko\":\"동전\",\"_tr\":\"Paralar\"},{\"_id\":\"t_050\",\"\":\"6 - http://joxi.ru/VrwKoDOToLaOWr\",\"_ru\":\"Забрать\",\"_en\":\"Claim\",\"_ro\":\"Colectați\",\"_fr\":\"Collectez\",\"_de\":\"Sammeln\",\"_sp\":\"Recopilar\",\"_it\":\"Raccogliere\",\"_jp\":\"収集\",\"_ko\":\"모으다\",\"_tr\":\"Toplamak\"},{\"_id\":\"t_051\",\"\":\"1 - http://joxi.ru/bmoDoepCOGwRj2\",\"_ru\":\"Ежедневные задания\",\"_en\":\"Daily goals\",\"_ro\":\"Obiective zilnice\",\"_fr\":\"Objectifs quotidiens\",\"_de\":\"Tägliche Ziele\",\"_sp\":\"Objetivos diarios\",\"_it\":\"Obiettivi quotidiani\",\"_jp\":\"毎日の目標\",\"_ko\":\"일일 목표\",\"_tr\":\"Günlük Hedefler\"},{\"_id\":\"t_052\",\"\":\"2 - http://joxi.ru/bmoDoepCOGwRj2\",\"_ru\":\"Крутить\",\"_en\":\"Spin\",\"_ro\":\"A învârti\",\"_fr\":\"Rotation\",\"_de\":\"Drehen\",\"_sp\":\"Girar\",\"_it\":\"Rotazione\",\"_jp\":\"スピン\",\"_ko\":\"회전\",\"_tr\":\"Döndürmek\"},{\"_id\":\"t_053\",\"\":\"3 - http://joxi.ru/bmoDoepCOGwRj2\",\"_ru\":\"Монеты даром\",\"_en\":\"Free coins\",\"_ro\":\"Monede gratis\",\"_fr\":\"Pièces gratuites\",\"_de\":\"Freie Münzen\",\"_sp\":\"Monedas gratis\",\"_it\":\"Monete gratis\",\"_jp\":\"無料のコイン\",\"_ko\":\"무료 동전\",\"_tr\":\"Ücretsiz Paralar\"},{\"_id\":\"t_054\",\"\":\"4 - http://joxi.ru/bmoDoepCOGwRj2\",\"_ru\":\"Играть\",\"_en\":\"Play\",\"_ro\":\"Joaca\",\"_fr\":\"Jouer\",\"_de\":\"Spielen\",\"_sp\":\"Jugar\",\"_it\":\"Giocare\",\"_jp\":\"遊ぶ\",\"_ko\":\"놀다\",\"_tr\":\"Oynamak\"},{\"_id\":\"t_055\",\"\":\"5 - http://joxi.ru/bmoDoepCOGwRj2\",\"_ru\":\"Бесконечно\",\"_en\":\"Endless\",\"_ro\":\"Fără sfârşit\",\"_fr\":\"Sans fin\",\"_de\":\"Endlos\",\"_sp\":\"Sin fin\",\"_it\":\"Infinito\",\"_jp\":\"無限\",\"_ko\":\"끝이 없습니다\",\"_tr\":\"Sonsuz\"},{\"_id\":\"t_056\",\"\":\"6 - http://joxi.ru/bmoDoepCOGwRj2\",\"_ru\":\"Статистика\",\"_en\":\"Statistics\",\"_ro\":\"Statistici\",\"_fr\":\"Statistiques\",\"_de\":\"Statistiken\",\"_sp\":\"Estadísticas\",\"_it\":\"Statistiche\",\"_jp\":\"統計\",\"_ko\":\"통계\",\"_tr\":\"İstatistik\"},{\"_id\":\"t_057\",\"\":\"7 - http://joxi.ru/bmoDoepCOGwRj2\",\"_ru\":\"Магазин\",\"_en\":\"Shop\",\"_ro\":\"Magazin\",\"_fr\":\"Boutique\",\"_de\":\"Geschäft\",\"_sp\":\"Comercio\",\"_it\":\"Negozio\",\"_jp\":\"店\",\"_ko\":\"가게\",\"_tr\":\"Mağaza\"},{\"_id\":\"t_058\",\"\":\"8 - http://joxi.ru/bmoDoepCOGwRj2\",\"_ru\":\"Свинья копилка\",\"_en\":\"Piggy bank\",\"_ro\":\"Pușculiță\",\"_fr\":\"Tirelire\",\"_de\":\"Sparschwein\",\"_sp\":\"Hucha\",\"_it\":\"Salvadanaio\",\"_jp\":\"貯金箱\",\"_ko\":\"돼지 저금통\",\"_tr\":\"Kumbara\"},{\"_id\":\"t_059\",\"\":\"9 - http://joxi.ru/bmoDoepCOGwRj2\",\"_ru\":\"Коллекция\",\"_en\":\"Collection\",\"_ro\":\"Colectie\",\"_fr\":\"Collection\",\"_de\":\"Sammlung\",\"_sp\":\"Recopilación\",\"_it\":\"Collezione\",\"_jp\":\"コレクション\",\"_ko\":\"수집\",\"_tr\":\"Toplamak\"},{\"_id\":\"t_060\",\"\":\"http://joxi.ru/5mdlkV8UqlneQm\",\"_ru\":\"Накопите как минимум 300 золотых, чтобы потом приобрести по отличной цене\",\"_en\":\"Add at least 300 golds to the Piggy Bank to buy it  at a great deal\",\"_ro\":\"Adăugați cel puțin 300 de aur la Piggy Bank pentru a -l cumpăra la o mare cantitate\",\"_fr\":\"Ajoutez au moins 300 médailles d'or à la tirelire pour l'acheter à beaucoup\",\"_de\":\"Fügen Sie dem Sparschwein mindestens 300 Golds hinzu\",\"_sp\":\"Agregue al menos 300 oro al piggy Bank para comprarlo en una gran oferta\",\"_it\":\"Aggiungi almeno 300 ori al salvadanaio per acquistarlo in molto\",\"_jp\":\"少なくとも300ゴールドを貯金箱に加えて大量に購入する\",\"_ko\":\"돼지 은행에 최소 300 골드를 추가하여 많이 구입하십시오.\",\"_tr\":\"Piggy Bank'a en az 300 altın ekleyin, çok fazla satın almak için\"},{\"_id\":\"t_061\",\"\":\"1 - http://joxi.ru/v290QzZC4gqRRm\",\"_ru\":\"Специальное предложение\",\"_en\":\"Special offer\",\"_ro\":\"Oferta speciala\",\"_fr\":\"Offre spéciale\",\"_de\":\"Sonderangebot\",\"_sp\":\"Oferta especial\",\"_it\":\"Offerta speciale\",\"_jp\":\"特別なオファー\",\"_ko\":\"특별 메뉴\",\"_tr\":\"Özel teklif\"},{\"_id\":\"t_062\",\"\":\"2 - http://joxi.ru/v290QzZC4gqRRm\",\"_ru\":\"Набор мастера\",\"_en\":\"Master bundle\",\"_ro\":\"BUNDER MASTER\",\"_fr\":\"Paquet de maître\",\"_de\":\"Master -Bundle\",\"_sp\":\"Paquete maestro\",\"_it\":\"Pacchetto maestro\",\"_jp\":\"マスターバンドル\",\"_ko\":\"마스터 번들\",\"_tr\":\"Ana paket\"},{\"_id\":\"t_063\",\"\":\"3 - http://joxi.ru/v290QzZC4gqRRm\",\"_ru\":\"Супер набор\",\"_en\":\"Super bundle\",\"_ro\":\"Super Bundle\",\"_fr\":\"Super paquet\",\"_de\":\"Super Bundle\",\"_sp\":\"Super Bundle\",\"_it\":\"Super Bundle\",\"_jp\":\"スーパーバンドル\",\"_ko\":\"슈퍼 번들\",\"_tr\":\"Süper paket\"},{\"_id\":\"t_066\",\"\":\"1 - http://joxi.ru/GrqQMDotzBKYlr\",\"_ru\":\"Монеты даром\",\"_en\":\"Free coins\",\"_ro\":\"Monede gratis\",\"_fr\":\"Pièces gratuites\",\"_de\":\"Freie Münzen\",\"_sp\":\"Monedas gratis\",\"_it\":\"Monete gratis\",\"_jp\":\"無料のコイン\",\"_ko\":\"무료 동전\",\"_tr\":\"Ücretsiz Paralar\"},{\"_id\":\"t_068\",\"\":\"3 - http://joxi.ru/GrqQMDotzBKYlr\",\"_ru\":\"Получи бесплатные монеты посмотрев видео\",\"_en\":\"Get free coins by watching a video\",\"_ro\":\"Obțineți monede gratuite urmărind un videoclip\",\"_fr\":\"Obtenez des pièces gratuites en regardant une vidéo\",\"_de\":\"Holen Sie sich kostenlose Münzen, indem Sie sich ein Video ansehen\",\"_sp\":\"Obtenga monedas gratis viendo un video\",\"_it\":\"Ottieni monete gratuite guardando un video\",\"_jp\":\"ビデオを見て無料のコインを入手してください\",\"_ko\":\"비디오를 보면서 무료 동전을 받으십시오\",\"_tr\":\"Bir video izleyerek ücretsiz paralar alın\"},{\"_id\":\"t_069\",\"\":\"4 - http://joxi.ru/GrqQMDotzBKYlr\",\"_ru\":\"Смотреть\",\"_en\":\"Watch\",\"_ro\":\"Ceas\",\"_fr\":\"Montre\",\"_de\":\"Betrachten\",\"_sp\":\"Mirar\",\"_it\":\"Orologio\",\"_jp\":\"時計\",\"_ko\":\"보다\",\"_tr\":\"Kol saati\"},{\"_id\":\"t_070\",\"\":\"1 - http://joxi.ru/E2pYvDocvoeDPA\",\"_ru\":\"Ежедневные задания\",\"_en\":\"Daily goals\",\"_ro\":\"Obiective zilnice\",\"_fr\":\"Objectifs quotidiens\",\"_de\":\"Tägliche Ziele\",\"_sp\":\"Objetivos diarios\",\"_it\":\"Obiettivi quotidiani\",\"_jp\":\"毎日の目標\",\"_ko\":\"일일 목표\",\"_tr\":\"Günlük Hedefler\"},{\"_id\":\"t_071\",\"\":\"2 - http://joxi.ru/E2pYvDocvoeDPA\",\"_ru\":\"Достигнуть числа 15 в нормальном режиме\",\"_en\":\"Reach to number 15 in normal mode \",\"_ro\":\"Ajungeți la numărul 15 în modul normal\",\"_fr\":\"Atteindre le numéro 15 en mode normal\",\"_de\":\"Greifen Sie auf Nummer 15 im normalen Modus\",\"_sp\":\"Alcanzar al número 15 en modo normal\",\"_it\":\"Raggiungere il numero 15 in modalità normale\",\"_jp\":\"通常モードで15番に到達します\",\"_ko\":\"일반 모드에서 15 번에 도달하십시오\",\"_tr\":\"Normal modda 15 numaraya ulaşın\"},{\"_id\":\"t_072\",\"\":\"3 - http://joxi.ru/E2pYvDocvoeDPA\",\"_ru\":\"Завершено\",\"_en\":\"Completed\",\"_ro\":\"Efectuat\",\"_fr\":\"Complété\",\"_de\":\"Vollendet\",\"_sp\":\"Terminado\",\"_it\":\"Completato\",\"_jp\":\"完了しました\",\"_ko\":\"완전한\",\"_tr\":\"Tamamlanmış\"},{\"_id\":\"t_073\",\"\":\"4 - http://joxi.ru/E2pYvDocvoeDPA\",\"_ru\":\"Использовать кнопку [button icon] 1 раз\",\"_en\":\"Use button [button icon] 1 time \",\"_ro\":\"Utilizați butonul [pictograma butonului] 1 dată\",\"_fr\":\"Utiliser le bouton [icône du bouton] 1 fois\",\"_de\":\"Verwenden Sie die Schaltfläche [Tastensymbol] 1 Mal\",\"_sp\":\"Use el botón [icono del botón] 1 vez\",\"_it\":\"Utilizzare il pulsante [icona del pulsante] 1 ora\",\"_jp\":\"ボタン[ボタンアイコン]を1回使用します\",\"_ko\":\"사용 버튼 [버튼 아이콘] 1 시간\",\"_tr\":\"1 kez kullanın düğmesi [düğme simgesi]\"},{\"_id\":\"t_074\",\"\":\"5 - http://joxi.ru/E2pYvDocvoeDPA\",\"_ru\":\"Награда\",\"_en\":\"Reward\",\"_ro\":\"Răsplată\",\"_fr\":\"Récompense\",\"_de\":\"Belohnen\",\"_sp\":\"Premio\",\"_it\":\"Ricompensa\",\"_jp\":\"褒美\",\"_ko\":\"보상\",\"_tr\":\"Ödül\"},{\"_id\":\"t_075\",\"\":\"6 - http://joxi.ru/E2pYvDocvoeDPA\",\"_ru\":\"Просмотреть видео за награду\",\"_en\":\"Watch a video reward\",\"_ro\":\"Urmăriți o recompensă video\",\"_fr\":\"Regarder une récompense vidéo\",\"_de\":\"Sehen Sie sich eine Video -Belohnung an\",\"_sp\":\"Mira una recompensa de video\",\"_it\":\"Guarda una ricompensa video\",\"_jp\":\"ビデオ報酬を見る\",\"_ko\":\"비디오 보상을보십시오\",\"_tr\":\"Bir Video Ödül İzleyin\"},{\"_id\":\"t_076\",\"\":\"7 - http://joxi.ru/E2pYvDocvoeDPA\",\"_ru\":\"Вперёд\",\"_en\":\"Go\",\"_ro\":\"Merge\",\"_fr\":\"Aller\",\"_de\":\"Gehen\",\"_sp\":\"Ir\",\"_it\":\"Andare\",\"_jp\":\"行く\",\"_ko\":\"가다\",\"_tr\":\"Gitmek\"},{\"_id\":\"t_077\",\"\":\"1 - http://joxi.ru/12ML1nMHgyGEVm\",\"_ru\":\"Специальное предложение\",\"_en\":\"Special offer\",\"_ro\":\"Oferta speciala\",\"_fr\":\"Offre spéciale\",\"_de\":\"Sonderangebot\",\"_sp\":\"Oferta especial\",\"_it\":\"Offerta speciale\",\"_jp\":\"特別なオファー\",\"_ko\":\"특별 메뉴\",\"_tr\":\"Özel teklif\"},{\"_id\":\"t_078\",\"\":\"2 - http://joxi.ru/12ML1nMHgyGEVm\",\"_ru\":\"На 300% больше\",\"_en\":\"%300 more\",\"_ro\":\"%Încă 300\",\"_fr\":\"% 300 de plus\",\"_de\":\"%300 weitere\",\"_sp\":\"%300 más\",\"_it\":\"%300 in più\",\"_jp\":\"％300その他\",\"_ko\":\"%300 더\",\"_tr\":\"%300 daha\"},{\"_id\":\"t_079\",\"\":\"http://joxi.ru/brR3Dy4SBZ3L4m\",\"_ru\":\"Начать с чисел\",\"_en\":\"Start from\",\"_ro\":\"Începe de la\",\"_fr\":\"Commencer à partir de\",\"_de\":\"Beginne am\",\"_sp\":\"Empezar desde\",\"_it\":\"Inizia da\",\"_jp\":\"から始まる\",\"_ko\":\"에서 시작하다\",\"_tr\":\"Dan başla\"},{\"_id\":\"t_080\",\"\":\"\",\"_ru\":\"Награда!\",\"_en\":\"Rewards!\",\"_ro\":\"Recompense!\",\"_fr\":\"Récompenses!\",\"_de\":\"Belohnung!\",\"_sp\":\"¡Recompensas!\",\"_it\":\"Premi!\",\"_jp\":\"報酬！\",\"_ko\":\"보상!\",\"_tr\":\"Ödüller!\"},{\"_id\":\"t_081\",\"\":\"http://joxi.ru/GrqNOdPtzpX8Qm\",\"_ru\":\"Бесплатно!\",\"_en\":\"Free!\",\"_ro\":\"Gratis!\",\"_fr\":\"Libérer!\",\"_de\":\"Frei!\",\"_sp\":\"¡Gratis!\",\"_it\":\"Gratuito!\",\"_jp\":\"無料！\",\"_ko\":\"무료로!\",\"_tr\":\"Ücretsiz!\"},{\"_id\":\"t_082\",\"\":\"\",\"_ru\":\"Загрузка\",\"_en\":\"Loading\",\"_ro\":\"Se încarcă\",\"_fr\":\"Chargement\",\"_de\":\"Wird geladen\",\"_sp\":\"Cargando\",\"_it\":\"Caricamento\",\"_jp\":\"読み込み\",\"_ko\":\"로딩\",\"_tr\":\"Yükleniyor\"},{\"_id\":\"t_083\",\"\":\"\",\"_ru\":\"Упаковка\",\"_en\":\"Package\",\"_ro\":\"Pachet\",\"_fr\":\"Emballer\",\"_de\":\"Paket\",\"_sp\":\"Paquete\",\"_it\":\"Pacchetto\",\"_jp\":\"パッケージ\",\"_ko\":\"패키지\",\"_tr\":\"Paketi\"},{\"_id\":\"t_084\",\"\":\"\",\"_ru\":\"Пауза\",\"_en\":\"Pause\",\"_ro\":\"Pauză\",\"_fr\":\"Pause\",\"_de\":\"Pause\",\"_sp\":\"Pausa\",\"_it\":\"Pausa\",\"_jp\":\"一時停止\",\"_ko\":\"정지시키다\",\"_tr\":\"Duraklat\"},{\"_id\":\"t_085\",\"\":\"\",\"_ru\":\"Таблица лидеров\",\"_en\":\"Leaderboard\",\"_ro\":\"Clasament\",\"_fr\":\"Classement\",\"_de\":\"Bestenliste\",\"_sp\":\"Tabla de clasificación\",\"_it\":\"Classifica\",\"_jp\":\"リーダーボード\",\"_ko\":\"리더 보드\",\"_tr\":\"Liderler Sıralaması\"},{\"_id\":\"t_086\",\"\":\"\",\"_ru\":\"Локальный\",\"_en\":\"Local\",\"_ro\":\"Local\",\"_fr\":\"Local\",\"_de\":\"Lokal\",\"_sp\":\"Local\",\"_it\":\"Locale\",\"_jp\":\"地元\",\"_ko\":\"현지의\",\"_tr\":\"Yerel\"},{\"_id\":\"t_087\",\"\":\"\",\"_ru\":\"Глобальный\",\"_en\":\"Global\",\"_ro\":\"Global\",\"_fr\":\"Mondial\",\"_de\":\"Global\",\"_sp\":\"Global\",\"_it\":\"Globale\",\"_jp\":\"グローバル\",\"_ko\":\"글로벌\",\"_tr\":\"Küresel\"},{\"_id\":\"t_088\",\"\":\"\",\"_ru\":\"Обычный\",\"_en\":\"Normal\",\"_ro\":\"Normal\",\"_fr\":\"Normal\",\"_de\":\"Normal\",\"_sp\":\"Normal\",\"_it\":\"Normale\",\"_jp\":\"普通\",\"_ko\":\"정상\",\"_tr\":\"Normal\"},{\"_id\":\"t_089\",\"\":\"\",\"_ru\":\"Запустить\",\"_en\":\"Start\",\"_ro\":\"start\",\"_fr\":\"Commencer\",\"_de\":\"Start\",\"_sp\":\"Comenzar\",\"_it\":\"Inizio\",\"_jp\":\"始める\",\"_ko\":\"시작\",\"_tr\":\"Başlangıç\"},{\"_id\":\"t_090\",\"\":\"\",\"_ru\":\"Собирать x3\",\"_en\":\"Claim x3\",\"_ro\":\"Colectați x3\",\"_fr\":\"Collectez x3\",\"_de\":\"Sammeln x3\",\"_sp\":\"Recoger x3\",\"_it\":\"Raccogli X3\",\"_jp\":\"x3を収集します\",\"_ko\":\"X3를 수집하십시오\",\"_tr\":\"X3 topla\"},{\"_id\":\"t_091\",\"\":\"\",\"_ru\":\"Ваша награда\",\"_en\":\"Your reward\",\"_ro\":\"Recompensa ta\",\"_fr\":\"Ta récompense\",\"_de\":\"Deine Belohnung\",\"_sp\":\"Tu recompensa\",\"_it\":\"Il tuo premio\",\"_jp\":\"あなたの報酬\",\"_ko\":\"당신의 보상\",\"_tr\":\"Ödülün\"},{\"_id\":\"t_092\",\"\":\"\",\"_ru\":\"Приходите каждый день, чтобы не пропустить!\",\"_en\":\"Come every day, so you don't miss out!\",\"_ro\":\"Vino în fiecare zi, ca să nu ratezi!\",\"_fr\":\"Venez tous les jours, donc vous ne manquez pas!\",\"_de\":\"Komm jeden Tag, damit du nicht verpasst!\",\"_sp\":\"¡Ven todos los días, para que no te pierdas!\",\"_it\":\"Vieni ogni giorno, quindi non ti perdi!\",\"_jp\":\"毎日来て、あなたはお見逃しなく！\",\"_ko\":\"매일 오세요. 그래서 당신은 놓치지 않습니다!\",\"_tr\":\"Her gün gel, yani kaçırmayın!\"},{\"_id\":\"t_093\",\"\":\"\",\"_ru\":\"Объединить блоки\",\"_en\":\"Merge the blocks\",\"_ro\":\"Îmbinați blocurile\",\"_fr\":\"Fusionner les blocs\",\"_de\":\"Verschmelzen die Blöcke\",\"_sp\":\"Fusionar los bloques\",\"_it\":\"Unire i blocchi\",\"_jp\":\"ブロックをマージします\",\"_ko\":\"블록을 병합하십시오\",\"_tr\":\"Blokları birleştirin\"},{\"_id\":\"t_094\",\"\":\"\",\"_ru\":\"Посмотреть рекламное видео\",\"_en\":\"Watch a video ad \",\"_ro\":\"Urmăriți un anunț video\",\"_fr\":\"Regarder une annonce vidéo\",\"_de\":\"Sehen Sie sich eine Videoanzeige an\",\"_sp\":\"Mira un anuncio de video\",\"_it\":\"Guarda un annuncio video\",\"_jp\":\"ビデオ広告を見る\",\"_ko\":\"비디오 광고를 시청하십시오\",\"_tr\":\"Bir video reklamı izleyin\"},{\"_id\":\"t_095\",\"\":\"\",\"_ru\":\"Соберите 1 блок 15 в обычном режиме\",\"_en\":\"Collect 1 block 15 in Normal Mode\",\"_ro\":\"Colectați 1 bloc 15 în modul normal\",\"_fr\":\"Collectez 1 bloc 15 en mode normal\",\"_de\":\"Sammeln Sie 1 Block 15 im normalen Modus\",\"_sp\":\"Recolectar 1 bloque 15 en modo normal\",\"_it\":\"Raccogli 1 blocco 15 in modalità normale\",\"_jp\":\"通常モードで1ブロック15を収集します\",\"_ko\":\"일반 모드에서 1 블록 15를 수집하십시오\",\"_tr\":\"Normal modda 1 blok 15 toplayın\"},{\"_id\":\"t_096\",\"\":\"\",\"_ru\":\"Крути колесо фортуны 1 раз\",\"_en\":\"Spin the wheel of fortune for 1 time\",\"_ro\":\"Rotiți roata averii timp de o dată\",\"_fr\":\"Faites tourner la roue de la fortune pendant 1 fois\",\"_de\":\"Drehen Sie das Glücksrad für 1 Mal\",\"_sp\":\"Gira la rueda de la fortuna por 1 tiempo\",\"_it\":\"Girare la ruota della fortuna per 1 volta\",\"_jp\":\"ホイールオブフォーチュンを1回回転させます\",\"_ko\":\"Fortune의 바퀴를 한 번 돌리십시오\",\"_tr\":\"Fortune Wheel'i 1 kez döndür\"},{\"_id\":\"t_097\",\"\":\"\",\"_ru\":\"Использовать воскрешение 1 раз \",\"_en\":\"Use revive 1 time\",\"_ro\":\"Folosiți Revive 1 Time\",\"_fr\":\"Utilisez Revive 1 fois\",\"_de\":\"Verwenden Sie 1 Mal Revive\",\"_sp\":\"Use Revive 1 Time\",\"_it\":\"Usa RIVEVE 1 volta\",\"_jp\":\"Revive 1を使用してください\",\"_ko\":\"1 회 부활을 사용하십시오\",\"_tr\":\"Revive 1'i kullanın\"},{\"_id\":\"t_098\",\"\":\"\",\"_ru\":\"Используйте часы или пилу 1 раз\",\"_en\":\"Use the clock or saw 1 time\",\"_ro\":\"Folosiți ceasul sau a văzut 1 dată\",\"_fr\":\"Utilisez l'horloge ou scie 1 fois\",\"_de\":\"Verwenden Sie die Uhr oder Säge 1 Mal\",\"_sp\":\"Usa el reloj o vio 1 vez\",\"_it\":\"Usa l'orologio o visto 1 volta\",\"_jp\":\"時計を使用するか、1回目を使用します\",\"_ko\":\"시계를 사용하거나 한 번 보았습니다\",\"_tr\":\"Saati kullanın veya 1 kez gör\"},{\"_id\":\"t_099\",\"\":\"http://joxi.ru/LmGNXq6tByVqNr\",\"_ru\":\"Остановить бесплатно\",\"_en\":\"Stop Free\",\"_ro\":\"Opriți -vă gratuit\",\"_fr\":\"S'arrêter gratuitement\",\"_de\":\"Hör auf frei\",\"_sp\":\"Stop Free\",\"_it\":\"Fermati\",\"_jp\":\"無料で止めてください\",\"_ko\":\"무료로 중지하십시오\",\"_tr\":\"Bedava Durun\"},{\"_id\":\"t_100\",\"\":\"\",\"_ru\":\"По умолчанию\",\"_en\":\"Default\",\"_ro\":\"Mod implicit\",\"_fr\":\"Défaut\",\"_de\":\"Standard\",\"_sp\":\"Por defecto\",\"_it\":\"Predefinito\",\"_jp\":\"デフォルト\",\"_ko\":\"기본\",\"_tr\":\"Varsayılan\"},{\"_id\":\"t_101\",\"\":\"\",\"_ru\":\"Джунгли\",\"_en\":\"Jungles\",\"_ro\":\"Jungle\",\"_fr\":\"Jungles\",\"_de\":\"Dschungel\",\"_sp\":\"Selvas\",\"_it\":\"Giungle\",\"_jp\":\"ジャングル\",\"_ko\":\"정글\",\"_tr\":\"Ormanlar\"},{\"_id\":\"t_102\",\"\":\"\",\"_ru\":\"Неон\",\"_en\":\"Neon\",\"_ro\":\"Neon\",\"_fr\":\"Néon\",\"_de\":\"Neon\",\"_sp\":\"Neón\",\"_it\":\"Neon\",\"_jp\":\"ネオン\",\"_ko\":\"네온\",\"_tr\":\"Neon\"},{\"_id\":\"t_103\",\"\":\"\",\"_ru\":\"Подводный\",\"_en\":\"Underwater\",\"_ro\":\"Sub apă\",\"_fr\":\"Sous-marin\",\"_de\":\"Unterwasser\",\"_sp\":\"Submarino\",\"_it\":\"Sott'acqua\",\"_jp\":\"水中\",\"_ko\":\"수중\",\"_tr\":\"Su altı\"},{\"_id\":\"t_104\",\"\":\"\",\"_ru\":\"Космос\",\"_en\":\"Space\",\"_ro\":\"Spaţiu\",\"_fr\":\"Espace\",\"_de\":\"Raum\",\"_sp\":\"Espacio\",\"_it\":\"Spazio\",\"_jp\":\"空\",\"_ko\":\"공간\",\"_tr\":\"Uzay\"},{\"_id\":\"t_105\",\"\":\"\",\"_ru\":\"Лава\",\"_en\":\"Lava\",\"_ro\":\"Lavă\",\"_fr\":\"Lave\",\"_de\":\"Lava\",\"_sp\":\"Lava\",\"_it\":\"Lava\",\"_jp\":\"溶岩\",\"_ko\":\"용암\",\"_tr\":\"Lav\"},{\"_id\":\"t_106\",\"\":\"\",\"_ru\":\"Лед\",\"_en\":\"Ice\",\"_ro\":\"Gheaţă\",\"_fr\":\"Glace\",\"_de\":\"Eis\",\"_sp\":\"Hielo\",\"_it\":\"Ghiaccio\",\"_jp\":\"氷\",\"_ko\":\"얼음\",\"_tr\":\"buz\"},{\"_id\":\"t_107\",\"\":\"\",\"_ru\":\"Небо\",\"_en\":\"Sky\",\"_ro\":\"Cer\",\"_fr\":\"Ciel\",\"_de\":\"Himmel\",\"_sp\":\"Cielo\",\"_it\":\"Cielo\",\"_jp\":\"空\",\"_ko\":\"하늘\",\"_tr\":\"Gökyüzü\"},{\"_id\":\"t_108\",\"\":\"\",\"_ru\":\"Египет\",\"_en\":\"Egypt\",\"_ro\":\"Egipt\",\"_fr\":\"Egypte\",\"_de\":\"Ägypten\",\"_sp\":\"Egipto\",\"_it\":\"Egitto\",\"_jp\":\"エジプト\",\"_ko\":\"이집트\",\"_tr\":\"Mısır\"},{\"_id\":\"t_109\",\"\":\"\",\"_ru\":\"Рейтинг\",\"_en\":\"Ranking\",\"_ro\":\"Clasament\",\"_fr\":\"Classement\",\"_de\":\"Rangfolge\",\"_sp\":\"Clasificación\",\"_it\":\"classifica\",\"_jp\":\"ランキング\",\"_ko\":\"순위\",\"_tr\":\"Sıralama\"},{\"_id\":\"t_110\",\"\":\"\",\"_ru\":\"Слияние 20 раз\",\"_en\":\"Merge 20 times\",\"_ro\":\"Îmbinați de 20 de ori\",\"_fr\":\"Fusionner 20 fois\",\"_de\":\"20 Mal verschmelzen\",\"_sp\":\"Fusionarse 20 veces\",\"_it\":\"Unire 20 volte\",\"_jp\":\"20回マージします\",\"_ko\":\"20 번 병합\",\"_tr\":\"20 kez birleştir\"},{\"_id\":\"t_111\",\"\":\"\",\"_ru\":\"Оцените нас\",\"_en\":\"Rate us\",\"_ro\":\"Ne evalua\",\"_fr\":\"Évaluez nous\",\"_de\":\"Bewerten Sie uns\",\"_sp\":\"Nos califica\",\"_it\":\"Valutaci\",\"_jp\":\"私たちを評価してください\",\"_ko\":\"우리를 평가하십시오\",\"_tr\":\"Bizi değerlendirin\"},{\"_id\":\"t_112\",\"\":\"\",\"_ru\":\"Если вам нравится играть, пожалуйста, оцените 5 звезд, чтобы поддержать нас!\",\"_en\":\"If you enjoy playing, please rate 5 stars to encourage us!\",\"_ro\":\"Dacă vă place să jucați, vă rugăm să evaluați 5 stele pentru a ne încuraja!\",\"_fr\":\"Si vous aimez jouer, veuillez évaluer 5 étoiles pour nous encourager!\",\"_de\":\"Wenn Sie gerne spielen, bewerten Sie bitte 5 Sterne, um uns zu ermutigen!\",\"_sp\":\"Si te gusta jugar, ¡califique 5 estrellas para alentarnos!\",\"_it\":\"Se ti piace giocare, per favore, valuta 5 stelle per incoraggiarci!\",\"_jp\":\"プレイを楽しんでいる場合は、5つ星を評価して私たちを励ましてください！\",\"_ko\":\"당신이 연주를 즐기고 있다면, 우리를 격려하기 위해 별 5 개를 평가하십시오!\",\"_tr\":\"Oynamaktan hoşlanıyorsanız, lütfen bizi teşvik etmek için 5 yıldız değerlendirin!\"},{\"_id\":\"t_113\",\"\":\"\",\"_ru\":\"Загрузка рекламы спонсора\",\"_en\":\"Loading sponsor ad\",\"_ro\":\"Încărcarea anunțului sponsor\",\"_fr\":\"Chargement de l'annonce du sponsor\",\"_de\":\"Ladesponsor AD\",\"_sp\":\"Cargando anuncio de patrocinador\",\"_it\":\"Caricamento dell'annuncio sponsor\",\"_jp\":\"スポンサー広告の読み込み\",\"_ko\":\"스폰서 광고로드\",\"_tr\":\"Yükleme Sponsor reklamı\"},{\"_id\":\"t_114\",\"\":\"\",\"_ru\":\"Скоро будет\",\"_en\":\"Coming soon\",\"_ro\":\"In curand\",\"_fr\":\"À venir\",\"_de\":\"Demnächst\",\"_sp\":\"Muy pronto\",\"_it\":\"Prossimamente\",\"_jp\":\"近日公開\",\"_ko\":\"곧 올 것입니다\",\"_tr\":\"Yakında gelecek\"},{\"_id\":\"t_115\",\"\":\"\",\"_ru\":\"Рекламная пауза\",\"_en\":\"AD Break\",\"_ro\":\"Pauză publicitară\",\"_fr\":\"Pause publicitaire\",\"_de\":\"Anzeigenpause\",\"_sp\":\"Ruptura de anuncios\",\"_it\":\"AD BREAK\",\"_jp\":\"広告休憩\",\"_ko\":\"광고 브레이크\",\"_tr\":\"Reklam kırılması\"},{\"_id\":\"t_116\",\"\":\"\",\"_ru\":\"Бесплатно\",\"_en\":\"Free\",\"_ro\":\"Gratuit\",\"_fr\":\"Gratuit\",\"_de\":\"Frei\",\"_sp\":\"Gratis\",\"_it\":\"Gratuito\",\"_jp\":\"無料\",\"_ko\":\"무료\",\"_tr\":\"Özgür\"}]\n"
			+ "TYPE_аналитика_ENDTYPE\n"
			+ "[{\"id_event\":\"play_tap\",\"описание\":\"Игрок нажал на кнопку Play\",\"-/+\":\"+\"},{\"id_event\":\"endless_tap\",\"описание\":\"Игрок нажал на кнопку Endless\",\"-/+\":\"+\"},{\"id_event\":\"tutorial_start\",\"описание\":\"Туториал при первом запуске игры\",\"-/+\":\"+\"},{\"id_event\":\"tutorial_complete\",\"описание\":\"Игрок завершил туториал\",\"-/+\":\"+\"},{\"id_event\":\"merge_blocks ''указывать количество соединенных блоков''\",\"описание\":\"Сколько раз игрок соединяет блоки\",\"-/+\":\"+\"},{\"id_event\":\"great_block10\",\"описание\":\"Когда игрок получает в первый раз за сессию блок с цыфтрой 10\",\"-/+\":\"+\"},{\"id_event\":\"great_block15\",\"описание\":\"Когда игрок получает в первый раз за сессию блок с цыфтрой 15\",\"-/+\":\"+\"},{\"id_event\":\"great_block20\",\"описание\":\"Когда игрок получает в первый раз за сессию блок с цыфтрой 20\",\"-/+\":\"+\"},{\"id_event\":\"great_block25\",\"описание\":\"Когда игрок получает в первый раз за сессию блок с цыфтрой 25\",\"-/+\":\"+\"},{\"id_event\":\"start_from5\",\"описание\":\"Игрок начал новую сессию с цифрой 5\",\"-/+\":\"+\"},{\"id_event\":\"start_from10\",\"описание\":\"Игрок начал новую сессию с цифрой10\",\"-/+\":\"+\"},{\"id_event\":\"start_from18\",\"описание\":\"Игрок начал новую сессию с цифрой18\",\"-/+\":\"+\"},{\"id_event\":\"wheel_free\",\"описание\":\"Игрок остановил колесо за бесплатные крутки\",\"-/+\":\"+\"},{\"id_event\":\"wheel_coin\",\"описание\":\"Игрок остановил колесо за монеты\",\"-/+\":\"+\"},{\"id_event\":\"wheel_ad\",\"описание\":\"Игрок остановил колесо за рекламу\",\"-/+\":\"+\"},{\"id_event\":\"clime_ad\",\"описание\":\"Игрок нажала на clime x3\",\"-/+\":\"+\"},{\"id_event\":\"revive_coin\",\"описание\":\"Игрок воскрес за монеты\",\"-/+\":\"+\"},{\"id_event\":\"revive_ad\",\"описание\":\"Игрок воскрес за рекламу\",\"-/+\":\"+\"},{\"id_event\":\"revive_adwatch\",\"описание\":\"Игрок нажимает на кнопку рекламы из попап Revive\",\"-/+\":\"+\"},{\"id_event\":\"goals_entry\",\"описание\":\"Игрок зашел в попап Daily goals\",\"-/+\":\"+\"},{\"id_event\":\"goals_done \"указать сколько миссий сделал игрок'\",\"описание\":\"Игрок завершил миссию ''указать сколько ин выполнил''\",\"-/+\":\"+\"},{\"id_event\":\"goals_reward\",\"описание\":\"Игрок забирает награду за ежедневные миссии\",\"-/+\":\"+\"},{\"id_event\":\"daily_reward \"Указать тут число для\"\",\"описание\":\"Игрок заберает награду за вход\",\"-/+\":\"+\"},{\"id_event\":\"ranking_open\",\"описание\":\"Игрок открыл попап рейтинга\",\"-/+\":\"+\"},{\"id_event\":\"pause_open\",\"описание\":\"Игрок нажал на кнопку паузы\",\"-/+\":\"+\"},{\"id_event\":\"music_tap\",\"описание\":\"Игрок нажал на иконку музыки из попапа паузы\",\"-/+\":\"-\"},{\"id_event\":\"sound_tap\",\"описание\":\"Игрок нажал на иконку звуков из попапа паузы\",\"-/+\":\"-\"},{\"id_event\":\"resume_tap\",\"описание\":\"Игрок нажал на кнопку продолжить из попапа паузы\",\"-/+\":\"+\"},{\"id_event\":\"retry_tap\",\"описание\":\"Игрок нажал на кнопку павтра из попапа паузы\",\"-/+\":\"+\"},{\"id_event\":\"tutorial_tap\",\"описание\":\"Игрок нажал на кнопку туториал из попапа паузы\",\"-/+\":\"+\"},{\"id_event\":\"vip_tap\",\"описание\":\"Игрок нажал на кнопку VIP из попапа паузы\",\"-/+\":\"+\"},{\"id_event\":\"offer_tap\",\"описание\":\"Игрок открыл попап спешл оффер\",\"-/+\":\"+\"},{\"id_event\":\"vip_open\",\"описание\":\"Игрок зашел в попап VIP через меню игры\",\"-/+\":\"+\"},{\"id_event\":\"ad_watch\",\"описание\":\"Игрок смотрит рекламу через попап Free Coins\",\"-/+\":\"+\"},{\"id_event\":\"shop_tap\",\"описание\":\"Игрок зашел в попап магазина\",\"-/+\":\"+\"},{\"id_event\":\"package_tap\",\"описание\":\"Игрок нажал на иконку Package\",\"-/+\":\"+\"},{\"id_event\":\"block_buy\",\"описание\":\"Игроку купил новый скин для блоков\",\"-/+\":\"+\"},{\"id_event\":\"block_free\",\"описание\":\"Игрок открыл новый скин для блоков с помощью ключа(бесплатно)\",\"-/+\":\"+\"},{\"id_event\":\"theme_buy\",\"описание\":\"Игрок купил новую фоновую тему\",\"-/+\":\"+\"},{\"id_event\":\"effect_buy\",\"описание\":\"Игрок купил новый эфект\",\"-/+\":\"+\"},{\"id_event\":\"\",\"описание\":\"https://docs.google.com/spreadsheets/d/1NIK7CB-q5JYr3B5XbDj1TbFmTMB2IUEfSX4RiATd-Vo/edit#gid=0\",\"-/+\":\"\"},{\"id_event\":\"\",\"описание\":\"Подключить после выхода игры ивенты для издателя\",\"-/+\":\"\"}]\n"
			+ "TYPE_Balance_ENDTYPE\n"
			+ "[{\"_id\":\"\",\"\":\"\",\"_value\":\"\"},{\"_id\":\"Переменная\",\"\":\"изначальное значение (чтобы не забыть)\",\"_value\":\"Значение\"},{\"_id\":\"couplesMinSize\",\"\":2,\"_value\":2},{\"_id\":\"couplesAEnable\",\"\":10,\"_value\":10},{\"_id\":\"couplesMaxSizeA\",\"\":2,\"_value\":2},{\"_id\":\"couplesBEnable\",\"\":15,\"_value\":15},{\"_id\":\"\",\"\":\"\",\"_value\":\"\"},{\"_id\":\"maxValue1\",\"\":10,\"_value\":10},{\"_id\":\"bigChanceMin1\",\"\":1,\"_value\":1},{\"_id\":\"bigChanceMax1\",\"\":5,\"_value\":5},{\"_id\":\"midChanceMin1\",\"\":1,\"_value\":1},{\"_id\":\"smallChanceMin1\",\"\":1,\"_value\":1},{\"_id\":\"smallChanceMax1\",\"\":5,\"_value\":5},{\"_id\":\"\",\"\":\"\",\"_value\":\"\"},{\"_id\":\"maxValue2\",\"\":15,\"_value\":15},{\"_id\":\"bigChanceMin2\",\"\":4,\"_value\":4},{\"_id\":\"bigChanceMax2\",\"\":9,\"_value\":9},{\"_id\":\"midChanceMin2\",\"\":4,\"_value\":4},{\"_id\":\"smallChanceMin2\",\"\":1,\"_value\":1},{\"_id\":\"smallChanceMax2\",\"\":3,\"_value\":3},{\"_id\":\"\",\"\":\"\",\"_value\":\"\"},{\"_id\":\"maxValue3\",\"\":20,\"_value\":20},{\"_id\":\"bigChanceMin3\",\"\":6,\"_value\":6},{\"_id\":\"bigChanceMax3\",\"\":14,\"_value\":14},{\"_id\":\"midChanceMin3\",\"\":6,\"_value\":6},{\"_id\":\"smallChanceMin3\",\"\":1,\"_value\":1},{\"_id\":\"smallChanceMax3\",\"\":3,\"_value\":3},{\"_id\":\"\",\"\":\"\",\"_value\":\"\"},{\"_id\":\"maxValue4\",\"\":25,\"_value\":25},{\"_id\":\"bigChanceMin4\",\"\":8,\"_value\":8},{\"_id\":\"bigChanceMax4\",\"\":17,\"_value\":17},{\"_id\":\"midChanceMin4\",\"\":10,\"_value\":10},{\"_id\":\"smallChanceMin4\",\"\":1,\"_value\":1},{\"_id\":\"smallChanceMax4\",\"\":3,\"_value\":3},{\"_id\":\"\",\"\":\"\",\"_value\":\"\"},{\"_id\":\"maxValue5\",\"\":\"~\",\"_value\":0},{\"_id\":\"bigChanceMin5\",\"\":15,\"_value\":15},{\"_id\":\"bigChanceMax5\",\"\":21,\"_value\":21},{\"_id\":\"midChanceMin5\",\"\":10,\"_value\":10},{\"_id\":\"smallChanceMin5\",\"\":1,\"_value\":1},{\"_id\":\"smallChanceMax5\",\"\":3,\"_value\":3},{\"_id\":\"\",\"\":\"\",\"_value\":\"\"},{\"_id\":\"wheelFortuneChance\",\"\":5,\"_value\":20},{\"_id\":\"wheelAttachedSec\",\"\":60,\"_value\":60},{\"_id\":\"wheelButtonSec\",\"\":60,\"_value\":60},{\"_id\":\"timerSec\",\"\":15,\"_value\":15},{\"_id\":\"timeBoosterHelp\",\"\":2,\"_value\":2},{\"_id\":\"keyChance\",\"\":5,\"_value\":5},{\"_id\":\"moneyCellChance\",\"\":5,\"_value\":5},{\"_id\":\"pauseBetweenMusicMin\",\"\":10,\"_value\":10},{\"_id\":\"pauseBetweenMusicMax\",\"\":20,\"_value\":20},{\"_id\":\"timerRateUsShow1\",\"\":180,\"_value\":180},{\"_id\":\"timerRateUsShow2\",\"\":600,\"_value\":600}]\n"
			+ "TYPE_INAP_ENDTYPE\n"
			+ "[{\"id purchase\":\"\",\"Type\":\"\",\"price\":\"\"},{\"id purchase\":\"com.oriplay.project2048.specialoffer\",\"Type\":\"Consumable\",\"price\":\"99.99$\"},{\"id purchase\":\"com.oriplay.project2048.masterbundle\",\"Type\":\"Consumable\",\"price\":\"6.99$\"},{\"id purchase\":\"com.oriplay.project2048.superbundle\",\"Type\":\"Consumable\",\"price\":\"19.99$\"},{\"id purchase\":\"com.oriplay.project2048.removeads\",\"Type\":\"nonConsumable\",\"price\":\"2.99$\"},{\"id purchase\":\"com.oriplay.project2048.1500coins\",\"Type\":\"Consumable\",\"price\":\"4.99$\"},{\"id purchase\":\"com.oriplay.project2048.700coins\",\"Type\":\"Consumable\",\"price\":\"2.99$\"},{\"id purchase\":\"com.oriplay.project2048.300coins\",\"Type\":\"Consumable\",\"price\":\"1.99$\"},{\"id purchase\":\"com.oriplay.project2048.100coins\",\"Type\":\"Consumable\",\"price\":\"0.99$\"},{\"id purchase\":\"com.oriplay.project2048.weeklysubscription\",\"Type\":\"Subscription\",\"price\":\"0.99$\"},{\"id purchase\":\"com.oriplay.project2048.monthlysubscription\",\"Type\":\"Subscription\",\"price\":\"2.99$\"},{\"id purchase\":\"com.oriplay.project2048.yearlysubscription\",\"Type\":\"Subscription\",\"price\":\"9.99$\"}]\n"
			+ "TYPE_задачи Андрей_ENDTYPE\n"
			+ "[{\"коммит\":\"\",\"date\":\"2021-06-24T21:00:00.000Z\",\"h real\":\"\",\"h ~\":1,\"имя\":\"Андрей\",\"тип\":\"правки\",\"Задачи - Андроид - 02.06.21\":\"\",\"-/ok\":\"-\"}]\n";


		response = PlayerPrefs.GetString("GoogleSheetResponseRequest", response);

		List<string> objTypeName = new List<string>();
		List<string> jsonData = new List<string>();
		string parsed = "";
		QueryType returnType = QueryType.getObjects;

		// Response for GetObjectsByField()
		if (response.StartsWith(MSG_OBJ_DATA))
		{
			parsed = response.Substring(MSG_OBJ_DATA.Length + 1);
			objTypeName.Add(parsed.Substring(0, parsed.IndexOf(TYPE_END)));
			jsonData.Add(parsed.Substring(parsed.IndexOf(TYPE_END) + TYPE_END.Length));
			returnType = QueryType.getObjects;
		}

		// Response for GetTable()
		if (response.StartsWith(MSG_TBL_DATA))
		{
			parsed = response.Substring(MSG_TBL_DATA.Length + 1);
			objTypeName.Add(parsed.Substring(0, parsed.IndexOf(TYPE_END)));
			jsonData.Add(parsed.Substring(parsed.IndexOf(TYPE_END) + TYPE_END.Length));
			returnType = QueryType.getTable;
		}

		// Response for GetAllTables()
		if (response.StartsWith(MSG_TBLS_DATA))
		{
			parsed = response.Substring(MSG_TBLS_DATA.Length + 1);

			// First split creates substrings containing type and content on each one.
			string[] separator = new string[] { TYPE_STRT };
			string[] split = parsed.Split(separator, System.StringSplitOptions.None);

			// Second split gives the final lists of type names and data on different lists.
			separator = new string[] { TYPE_END };
			for (int i = 0; i < split.Length; i++)
			{
				if (split[i] == "")
					continue;



				string[] secSplit = split[i].Split(separator, System.StringSplitOptions.None);
				objTypeName.Add(secSplit[0]);
				jsonData.Add(secSplit[1]);
				//Debug.Log(secSplit[0]);
				//Debug.Log(secSplit[1]);
				//Debug.Log("======");
			}
			returnType = QueryType.getAllTables;
		}
		
		LocalizationLoader.Instance.SetOfflineTextsEditor(returnType, objTypeName, jsonData);
		////onGetDataFromLocalData?.Invoke(returnType, objTypeName, jsonData);
		BalanceLoader.Instance.GoogleSheets_onGetData(returnType, objTypeName, jsonData);

		//onGetData?.Invoke(returnType, objTypeName, jsonData);
		//processedResponseCallback.Invoke(returnType, objTypeName, jsonData);
	}

	private void UpdateStatus(string status)
	{
		currentStatus = status;
	}
}
