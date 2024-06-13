using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{
    public enum TransitionType//ö�ٱ���
    {
        SameScene, DifferentScene
    }

    [Header("Transition Info")]//������Ϣ
    public string sceneName;//���͵ĳ�������
    public TransitionType transitionType;//���͵�����:�����˵�����ѡ��
    public TransitionDestination.DestinationTag destinationTag;//���͵�Ŀ��λ�ñ�ǩ

    private bool canTrans;//�ж��ܷ���

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canTrans == true)
        {
            //TODO:SceneController����
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