using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType//枚举变量
    {
        SameScene, DifferentScene
    }

    [Header("Transition Info")]//传送信息
    public string sceneName;//传送的场景名字
    public TransitionType transitionType;//传送的类型:下拉菜单进行选择
    public TransitionDestination.DestinationTag destinationTag;//传送的目标位置标签

    private bool canTrans;//判断能否传送

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans == true)
        {
            //TODO:SceneController传送
            SceneController.Instance.TransitionToDestination(this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canTrans = false;
        }
    }
}