using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;
    private CinemachineFreeLook followCamera;//���

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();//�۲����б�

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RigisterPlayer(CharacterStats player)//����ע��
    {
        playerStats = player;

        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    public void AddObserver(IEndGameObserver observer)//�ù۲���������ӵ��б�
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)//�ù۲����Ƴ��б�
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()//�����еĹ۲��߹㲥
    {
        foreach (var observer in endGameObservers)
        {
            observer.EndNotify();
        }
    }

    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.A)
                return item.transform;
        }
        return null;
    }

    internal void RemoveObServer(EnemyController enemyController)
    {
        throw new NotImplementedException();
    }
}
