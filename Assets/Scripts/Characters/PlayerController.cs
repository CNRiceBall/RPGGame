using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Rock;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    private Animator anim;

    private GameObject attackTarget;

    private float lastAttackTime;

    private CharacterStats characterStats;

    private bool isDead;

    private float stopDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance; 
    }

    private void OnEnable()//�������õ�ʱ��ע�ᶩ��
    {
        GameManager.Instance.RigisterPlayer(characterStats);//ע��GameManager
    }
    private void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget; //onMouseClicked�¼����ע�� MoveToTarget��������
        MouseManager.Instance.OnEnemyClicked += EventAttack;

        SaveManager.Instance.LoadPlayerData();//�õ��Լ��洢������
    }

    private void OnDisable()//�������л�������ʧ��Ҫ����2��������Actionȡ������
    {
        if (!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }

    private void Update()
    {
        isDead = characterStats.CurrentHealth == 0;

        if (isDead)
        {
            GameManager.Instance.NotifyObservers();//�㲥
        }

        SwitchAnimation();//ʵʱ�л�����
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()//ʵʱ�л�����
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);//sqrMagnitude��velocityת��Ϊ������ֵ
        anim.SetBool("Death", isDead);
    }

    public void MoveToTarget(Vector3 target) //�����������Vector3����֤����������ʽ���巽ʽ�Ǻ�onMouseClicked��ȫһ��
    {
        if (isDead) return;

        StopAllCoroutines();//��Ϲ���
        agent.isStopped = false;//���Խ����ƶ�
        agent.destination = target;

        //�����ƶ��ƶ���Ŀ��λ�þ���һ����λ
        agent.stoppingDistance = stopDistance;
    }

    private void EventAttack(GameObject target)
    {
        if (isDead) return;

        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attactData.criticalChance;
            StartCoroutine(MoveToAttackTarget());//Э��:��������
        }
    }
 
    IEnumerator MoveToAttackTarget()//Э��:��������
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attactData.attackRange;
 
        transform.LookAt(attackTarget.transform);//ת���ҵĹ���Ŀ��

        //�޸Ĺ�����Χ����
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attactData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;
        //Attack
        if (lastAttackTime < 0)
        {
            anim.SetTrigger("Attack");
            anim.SetBool("Critical", characterStats.isCritical);
            //������ȴʱ��
            lastAttackTime = characterStats.attactData.coolDown;
        }
    }

    //Animation Event
    void Hit()//����˺�
    {
        if (attackTarget.CompareTag("Attackable"))//�ɹ���������
        {
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == RockStates.HitNothing)//�ж��Ƿ���ʯͷ��״̬Ϊ��
            {
                attackTarget.GetComponent<Rock>().rockStates = RockStates.HitEnemy;//��ʯͷ״̬��Ϊ��������״̬
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);//Ϊʯͷ���һ��Playerǰ������ĳ����
            }
        }
        else//����
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();//��ʱ����

            targetStats.TakeDamage(characterStats, targetStats);//����˺�
        }
    }
}
