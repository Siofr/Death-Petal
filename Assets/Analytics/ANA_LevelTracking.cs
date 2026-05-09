using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;

[Serializable]
public class ANA_PlayerData
{
    [SerializeField]
    public float time;
    [SerializeField]
    public Vector3 position;
}

public class ANA_LevelTracking : MonoBehaviour
{
    //public string dataPath;
    public Key uploadKey = Key.H;
    public float logFrequency = 0.5f;
    public Transform playerTransform;

[SerializeField]
    public List<ANA_PlayerData> _playerDataOverTime =  new List<ANA_PlayerData>();
    public List<string> jsonData = new List<string>();

    public Coroutine activeRoutine;
    void Start()
    {
        InvokeRepeating("LogPlayerData", logFrequency, logFrequency);

    }

    void LogPlayerData()
    {
        ANA_PlayerData newLog = new ANA_PlayerData
        {
            time = Time.time,
            position = playerTransform.position
        };

        _playerDataOverTime.Add(newLog);
    }

    string PrebakeOutput()
    {
        string output = "";

        output += "Frequency: " + logFrequency;

        return output;
    }

    public string stringURL = "https://docs.google.com/forms/u/0/d/e/1FAIpQLScfxZ2l-vbD4KfOK1X62GW2x0_LzXTCiyj-DLrtkeDeTgfK2w/formResponse";
    public string targetEntry = "entry.711705458";
    IEnumerator SavePlayerData()
    {
        print(_playerDataOverTime.Count);

        for(int i = 0; i < _playerDataOverTime.Count; i++)
        {
            jsonData.Add(JsonUtility.ToJson(_playerDataOverTime[i]));
        }
        

        var combinedString = "[" + string.Join(",", jsonData) + "]";

        //print(combinedString);
        //File.WriteAllText(dataPath, combinedString);

        WWWForm form = new WWWForm();

        form.AddField(targetEntry, combinedString);

        UnityWebRequest www = UnityWebRequest.Post(stringURL, form);

        yield return www.SendWebRequest();
        activeRoutine = null;
    }

    void OnDisable()
    {
        //StartCoroutine(SavePlayerData());
    }

    void Update()
    {
        if (Keyboard.current[uploadKey].isPressed && activeRoutine == null)
        {
            activeRoutine = StartCoroutine(SavePlayerData());
        }
    }
}
