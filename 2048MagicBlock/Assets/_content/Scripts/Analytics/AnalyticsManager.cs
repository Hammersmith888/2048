using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
	[SerializeField] AppMetricaAnalyticsHandler _appMetricaAnalytics;
	[SerializeField] FirebaseAnalyticsHandler _firebaseAnalytics;
	public static AnalyticsManager Instance;
	
	void Awake()
	{
		Instance = this;
	}

	public void LogEvent(string eventName)
	{
		_appMetricaAnalytics.LogEvent(eventName);
		_firebaseAnalytics.LogEvent(eventName);
	}


}