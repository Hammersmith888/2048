using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;
using System;

public struct userAttributes { }
public struct appAttributes { }

public class BwLinks
{
    public List<string> Link;
}

public class RemoteController : MonoBehaviour
{
    public static RemoteController Instance;

  
    public int timeForAd = 60;
 
  

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ConfigManager.FetchCompleted += CacheConfigValues;
        ConfigManager.FetchConfigs<userAttributes, appAttributes>(new userAttributes(), new appAttributes());
    }

    private void OnDestroy()
    {
        ConfigManager.FetchCompleted -= CacheConfigValues;
    }



    private void CacheConfigValues(ConfigResponse response)
    {
        
        timeForAd = ConfigManager.appConfig.GetInt("timeForAd");
     
    }

    
}