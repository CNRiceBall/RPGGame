using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>, IEndGameObserver//���͵���ģʽ//�ӿڣ����ܹ㲥
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

    public void TransitionToDestination(TransitionPoint transitionPoint)//���ͷ���
    {
        switch (transitionPoint.transitionType)//�ж�ͬ�������ͻ��ǲ�ͬ��������
        {
            case TransitionPoint.TransitionType.SameScene://ͬ��������
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionPoint.TransitionType.DifferentScene://��ͬ��������
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }

    IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)//����Ŀ�곡����Ŀ���
    {
        //TODO:��������

        if (SceneManager.GetActiveScene().name != sceneName)//��ͬ��������
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            yield break;
        }
        else//ͬ��������
        {
            player = GameManager.Instance.playerStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();
            playerAgent.enabled = false;//����ʱ�ر�NavMeshAgent
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            playerAgent.enabled = true;//�����꿪��NavMeshAgent
            yield return null;
        }
    }

    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)//�õ�Ŀ���� TransitionDestination
    {
        var entrance = FindObjectsOfType<TransitionDestination>();//������е� TransitionDestination
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
            yield return StartCoroutine(fade.FadeOut(2.5f));//��������
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, GameManager.Instance.GetEntrance().position, GameManager.Instance.GetEntrance().rotation);

            //��������
            SaveManager.Instance.SavePlayerData();
            yield return StartCoroutine(fade.FadeIn(2.5f));//���볡��
            yield break;
        }
    }

    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));//��������
        yield return SceneManager.LoadSceneAsync("Main");
        yield return StartCoroutine(fade.FadeIn(2f));//���볡��
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