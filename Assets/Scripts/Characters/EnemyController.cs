using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }  //站桩，巡逻，追击，死亡

[RequireComponent(typeof(NavMeshAgent))]    //确保NavMeshAgent组件一定存在

[RequireComponent(typeof(CharacterStats))]


public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyStates enemyStates;

    private NavMeshAgent agent;

    private Animator anim;

    private Collider coll;
    
    protected CharacterStats characterStats;

    [Header("Basic Settings")]

    public float sightRadius;//可视范围 

    public bool isGuard;

    private float speed;

    protected GameObject attackTarget;

    public float lookAtTime;//观察时间

    private float remainLookAtTime;//仍需查看的时间

    private float lastAttackTime;

    private Quaternion guardRotation;//初始旋转角度

    [Header("Patrol State")]

    public float patrolRange;

    private Vector3 wayPoint;

    private Vector3 guardPos;

    //bool配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }

    void Start()
    {
        if (isGuard)//判断是否是站桩怪
        {
            enemyStates = EnemyStates.GUARD;
        }
        else//巡逻怪
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();//得到初始移动的点
        }
        //FIXME:切换场景后修改掉
        GameManager.Instance.AddObserver(this);//让观察者主动添加到列表
    }

    //切换场景时启用
    //private void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);//让观察者主动添加到列表
    //}

    private void OnDisable()//销毁完成之后执行
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);//让观察者移除列表
    }

    void Update()
    {
        if (characterStats.CurrentHealth == 0)
            isDead = true;//死亡
        if (!playerDead)
        {
            SwitchStates();//切换状态
            SwitchAnimation();//切换动画
            lastAttackTime -= Time.deltaTime;
        }
        if (playerDead)
        {
            GameManager.Instance.RemoveObServer(this);
        }

    }

    void SwitchAnimation()//切换动画
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    void SwitchStates() //切换状态
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;
 
        //如果发现Player,切换为CHASE
        else if (FoundPlayer())
        { 
            enemyStates = EnemyStates.CHASE;
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;//回到站桩点
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)//判断两点之间的距离
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;

            case EnemyStates.PATROL:

                isChase = false;
                agent.speed = speed * 0.5f;
                //判断是否到了随机巡逻点
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                    GetNewWayPoint();//随机获得巡逻范围内的一个点
                }
                else
                {
                    isWalk = true;

                    agent.destination = wayPoint;
                }

                break;
            case EnemyStates.CHASE:
                //配合动画
                isWalk = false;
                isChase = true;

                agent.speed = speed;

                if (!FoundPlayer())
                {
                    //拉托回到上一个状态
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }

                    else if (isGuard)
                enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;

                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;//追Player
                } 
                //在攻击范围内则攻击
                if (TargetInAttackRange() || TargetInSkillRange())//在攻击范围内
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)//攻击
                    {
                        lastAttackTime = characterStats.attactData.coolDown;
                        //暴击判断
                        characterStats.isCritical = Random.value < characterStats.attactData.criticalChance;
                        //执行攻击
                        Attack();
                    }
                }
                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    void Attack()//执行攻击
    {
        transform.LookAt(attackTarget.transform);//看着攻击目标
        if (TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("Skill");
        }
        else if (TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        } 
    }

    bool FoundPlayer()//在可视范围内寻找Player
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()//是否能进行近距离攻击
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attactData.attackRange;
        else
            return false;
    }

    bool TargetInSkillRange()//是否能进行远距离攻击
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attactData.skillRange;
        else
            return false;
    }

    void GetNewWayPoint()//随机获得巡逻范围内的一个点
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;

    }

    private void OnDrawGizmosSelected()//在对象被选中时绘制Gizmos
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);//画视野范围
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget != null && transform.isFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();//临时变量
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        //获胜动画
        //停止所有移动
        //停止Agent
        anim.SetBool("Win", true);
        playerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }


}