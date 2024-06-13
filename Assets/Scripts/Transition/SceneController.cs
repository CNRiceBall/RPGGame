using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>, IEndGameObserver//泛型单例模式//接口：接受广播
{
    public GameObject playerPrefab;

    public SceneFader sceneFaderPrefab;

    bool fadeFinshed;

    GameObject player;

    NavMeshAgent playerAgent;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinshed = true;
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)//传送方法
    {
        switch (transitionPoint.transitionType)//判断同场景传送还是不同场景传送
        {
            case TransitionPoint.TransitionType.SameScene://同场景传送
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene://不同场景传送
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)//传送目标场景、目标点
    {
        //TODO:保存数据

        if (SceneManager.GetActiveScene().name != sceneName)//不同场景传送
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            yield break;
        }
        else//同场景传送
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;//传送时关闭NavMeshAgent
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;//传送完开启NavMeshAgent
            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)//得到目标点的 TransitionDestination
    {
        var entrance = FindObjectsOfType<TransitionDestination>();//获得所有的 TransitionDestination
        for (int i = 0; i < entrance.Length; i++)
        {
            if (entrance[i].destinationTag == destinationTag)
            {
                return entrance[i];
            }
        }
        return null;
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    public void TransitionToLoadGame()//Continue Game
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Game"));
    }

    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);

        if (scene != "")
        {
            yield return StartCoroutine(fade.FadeOut(2.5f));//渐出场景
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            //保存数据
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2.5f));//渐入场景
            yield break;
        }
    }

    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));//渐出场景
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn(2f));//渐入场景
        yield break;
    }

    public void EndNotify()
    {
        if (fadeFinshed)
        {
            fadeFinshed = false;
            StartCoroutine(LoadMain());
        }
    }
}