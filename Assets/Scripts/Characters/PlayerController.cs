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

    private void OnEnable()//人物启用的时候注册订阅
    {
        GameManager.Instance.RigisterPlayer(characterStats);//注册GameManager
    }
    private void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget; //onMouseClicked事件添加注册 MoveToTarget（）方法
        MouseManager.Instance.OnEnemyClicked += EventAttack;

        SaveManager.Instance.LoadPlayerData();//拿到自己存储的数据
    }

    private void OnDisable()//当场景切换人物消失需要将这2个方法从Action取消订阅
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
            GameManager.Instance.NotifyObservers();//广播
        }

        SwitchAnimation();//实时切换动画
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()//实时切换动画
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);//sqrMagnitude将velocity转换为浮点数值
        anim.SetBool("Death", isDead);
    }

    public void MoveToTarget(Vector3 target) //必须包含参数Vector3，保证函数命名方式定义方式是和onMouseClicked完全一致
    {
        if (isDead) return;

        StopAllCoroutines();//打断攻击
        agent.isStopped = false;//可以进行移动
        agent.destination = target;

        //正常移动移动到目标位置距离一个身位
        agent.stoppingDistance = stopDistance;
    }

    private void EventAttack(GameObject target)
    {
        if (isDead) return;

        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attactData.criticalChance;
            StartCoroutine(MoveToAttackTarget());//协程:攻击敌人
        }
    }
 
    IEnumerator MoveToAttackTarget()//协程:攻击敌人
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attactData.attackRange;
 
        transform.LookAt(attackTarget.transform);//转向我的攻击目标

        //修改攻击范围参数
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
            //重置冷却时间
            lastAttackTime = characterStats.attactData.coolDown;
        }
    }

    //Animation Event
    void Hit()//造成伤害
    {
        if (attackTarget.CompareTag("Attackable"))//可攻击的物体
        {
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == RockStates.HitNothing)//判断是否是石头且状态为空
            {
                attackTarget.GetComponent<Rock>().rockStates = RockStates.HitEnemy;//将石头状态改为攻击敌人状态
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);//为石头添加一个Player前方方向的冲击力
            }
        }
        else//敌人
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();//临时变量

            targetStats.TakeDamage(characterStats, targetStats);//造成伤害
        }
    }
}
