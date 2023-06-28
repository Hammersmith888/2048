using UnityEngine;
using Firebase;
using Firebase.Analytics;

public class AppMetricaAnalyticsHandler : MonoBehaviour
{
	public void LogEvent(string eventName)
	{
		AppMetrica.Instance.ReportEvent(eventName);
	}
}