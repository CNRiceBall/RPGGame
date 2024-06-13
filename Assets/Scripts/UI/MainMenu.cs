using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button continueBtn;
    Button quitBtn;

    PlayableDirector director;//TimeLine����

    private void Awake()
    {
        newGameBtn = transform.GetChild(1).GetComponent<Button>();
        continueBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        newGameBtn.onClick.AddListener(PlayTimeLine);
        continueBtn.onClick.AddListener(ContinueGame);
        quitBtn.onClick.AddListener(QuitGame);

        director = FindObjectOfType<PlayableDirector>();

        director.stopped += NewGame;//��TimeLine����������֮�����NewGame����
    }

    void PlayTimeLine()
    {
        director.Play();
    }


    void NewGame(PlayableDirector obj)//�����ò���
    {
        PlayerPrefs.DeleteAll();
        //ת������
        SceneController.Instance.TransitionToFirstLevel();
    }

    void ContinueGame()
    {
        //ת����������ȡ����
        SceneController.Instance.TransitionToLoadGame();
    }

     void QuitGame()
    {
        //�˳���Ϸ
        Application.Quit();
        Debug.Log("�˳���Ϸ");
    }
}