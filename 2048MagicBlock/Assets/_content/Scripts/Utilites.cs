using System;
using System.Collections;
using UnityEngine;

public static class Utilites
{
    public static void sendEmail(string toEmail, string emailSubject, string emailBody)
    {
        emailSubject = System.Uri.EscapeUriString(emailSubject);
        emailBody = System.Uri.EscapeUriString(emailSubject);
        Application.OpenURL("mailto:" + toEmail + "?subject=" + emailSubject + "&body=" + emailBody);
    }

    public static IEnumerator ExecuteAfterTime(float time, Action action)
    {
        yield return new WaitForSeconds(time);

        action?.Invoke();
    }

    //TODO: Педелать в Binary
    public static void SaveBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public static bool LoadBool(string key, bool defaultVal = false)
    {
        return PlayerPrefs.GetInt(key, defaultVal ? 1 : 0) == 0 ? false : true;
    }

    #region UNUSED

    public static Quaternion LookAt2D(Vector3 src, Vector3 target)
    {
        Vector3 vectorToTarget = target - src;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg - 90;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    #endregion
}

