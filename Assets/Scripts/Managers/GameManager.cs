using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;
    private CinemachineFreeLook followCamera;//相机

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();//观察者列表

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    public void RigisterPlayer(CharacterStats player)//反向注册
    {
        playerStats = player;

        followCamera = FindObjectOfType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }

    public void AddObserver(IEndGameObserver observer)//让观察者主动添加到列表
    {
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)//让观察者移除列表
    {
        endGameObservers.Remove(observer);
    }

    public void NotifyObservers()//向所有的观察者广播
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
