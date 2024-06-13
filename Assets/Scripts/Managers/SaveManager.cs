using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    string sceneName = "l evel";//保存场景名字
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    public void Save(object data, string key)//保存数据
    {
        var jsonData = JsonUtility.ToJson(data, true);//将Data写为jsonData
        PlayerPrefs.SetString(key, jsonData);//将jsonData写入key
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);//保存场景名字
        PlayerPrefs.Save();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))//返回主场景
        {
            SceneController.Instance.TransitionToMain();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();//保存Player数据
            Debug.Log("保存Player数据");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();//加载Player数据
            Debug.Log("加载Player数据");
        }
    }

    public void SavePlayerData()//保存Player数据
    {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }
    public void LoadPlayerData()//加载Player数据
    {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void Load(object data, string key)//加载数据
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}
