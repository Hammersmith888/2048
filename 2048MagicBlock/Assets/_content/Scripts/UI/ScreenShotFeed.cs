using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShotFeed : MonoBehaviour
{
    private bool isFocus = false;

    private string shareSubject, shareMessege;
    private bool isProcessing = false;
    private string screenShotName;

    private void OnApplicationFocus(bool focus)
    {
        isFocus = focus;
    }

    public void OnShareButtonClick()
    {
        screenShotName = "Share";
        shareSubject = " ";
        shareMessege = " ";

        shareScreenShot();
        
    }
    private void shareScreenShot()
    {
        if(!isProcessing)
        {
            StartCoroutine(ShareScreenShotInAndroid);
        }
    }

    private void StartCoroutine(Func<IEnumerator> shareScreenShotInAndroid)
    {
        throw new NotImplementedException();
    }

    public IEnumerator ShareScreenShotInAndroid()
    {
        isProcessing = true;
        yield return new WaitForEndOfFrame();
        Texture2D screenTexture = new Texture2D(512, 512, TextureFormat.RGB24, true);
        screenTexture.Apply();
        byte[] dateToSave = Resources.Load<TextAsset>(screenShotName).bytes;
        string destination = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyyy") + ".png");
        Debug.Log(destination);
        File.WriteAllBytes(destination, dateToSave);

        if(!Application.isEditor)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);

            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), uriObject);
            intentObject.Call<AndroidJavaObject>("setType", "image/png");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), shareSubject);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareMessege);

            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.unityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentactivity");
            AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share your high score");
            currentActivity.Call("startActivity", chooser);
        }
        yield return new WaitUntil(() => isFocus);
        isProcessing = false;
    }

}