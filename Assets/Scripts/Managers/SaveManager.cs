using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    string sceneName = "l evel";//���泡������
    public string SceneName { get { return PlayerPrefs.GetString(sceneName); } }

    public void Save(object data, string key)//��������
    {
        var jsonData = JsonUtility.ToJson(data, true);//��DataдΪjsonData
        PlayerPrefs.SetString(key, jsonData);//��jsonDataд��key
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);//���泡������
        PlayerPrefs.Save();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))//����������
        {
            SceneController.Instance.TransitionToMain();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();//����Player����
            Debug.Log("����Player����");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();//����Player����
            Debug.Log("����Player����");
        }
    }

    public void SavePlayerData()//����Player����
    {
        Save(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }
    public void LoadPlayerData()//����Player����
    {
        Load(GameManager.Instance.playerStats.characterData, GameManager.Instance.playerStats.characterData.name);
    }

    public void Load(object data, string key)//��������
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}
