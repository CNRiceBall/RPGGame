using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }  //վ׮��Ѳ�ߣ�׷��������

[RequireComponent(typeof(NavMeshAgent))]    //ȷ��NavMeshAgent���һ������

[RequireComponent(typeof(CharacterStats))]


public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyStates enemyStates;

    private NavMeshAgent agent;

    private Animator anim;

    private Collider coll;
    
    protected CharacterStats characterStats;

    [Header("Basic Settings")]

    public float sightRadius;//���ӷ�Χ 

    public bool isGuard;

    private float speed;

    protected GameObject attackTarget;

    public float lookAtTime;//�۲�ʱ��

    private float remainLookAtTime;//����鿴��ʱ��

    private float lastAttackTime;

    private Quaternion guardRotation;//��ʼ��ת�Ƕ�

    [Header("Patrol State")]

    public float patrolRange;

    private Vector3 wayPoint;

    private Vector3 guardPos;

    //bool��϶���
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
        if (isGuard)//�ж��Ƿ���վ׮��
        {
            enemyStates = EnemyStates.GUARD;
        }
        else//Ѳ�߹�
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();//�õ���ʼ�ƶ��ĵ�
        }
        //FIXME:�л��������޸ĵ�
        GameManager.Instance.AddObserver(this);//�ù۲���������ӵ��б�
    }

    //�л�����ʱ����
    //private void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);//�ù۲���������ӵ��б�
    //}

    private void OnDisable()//�������֮��ִ��
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);//�ù۲����Ƴ��б�
    }

    void Update()
    {
        if (characterStats.CurrentHealth == 0)
            isDead = true;//����
        if (!playerDead)
        {
            SwitchStates();//�л�״̬
            SwitchAnimation();//�л�����
            lastAttackTime -= Time.deltaTime;
        }
        if (playerDead)
        {
            GameManager.Instance.RemoveObServer(this);
        }

    }

    void SwitchAnimation()//�л�����
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    void SwitchStates() //�л�״̬
    {
        if (isDead)
            enemyStates = EnemyStates.DEAD;
 
        //�������Player,�л�ΪCHASE
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
                    agent.destination = guardPos;//�ص�վ׮��
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)//�ж�����֮��ľ���
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                    }
                }
                break;

            case EnemyStates.PATROL:

                isChase = false;
                agent.speed = speed * 0.5f;
                //�ж��Ƿ������Ѳ�ߵ�
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                        remainLookAtTime -= Time.deltaTime;
                    else
                    GetNewWayPoint();//������Ѳ�߷�Χ�ڵ�һ����
                }
                else
                {
                    isWalk = true;

                    agent.destination = wayPoint;
                }

                break;
            case EnemyStates.CHASE:
                //��϶���
                isWalk = false;
                isChase = true;

                agent.speed = speed;

                if (!FoundPlayer())
                {
                    //���лص���һ��״̬
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
                    agent.destination = attackTarget.transform.position;//׷Player
                } 
                //�ڹ�����Χ���򹥻�
                if (TargetInAttackRange() || TargetInSkillRange())//�ڹ�����Χ��
                {
                    isFollow = false;
                    agent.isStopped = true;

                    if (lastAttackTime < 0)//����
                    {
                        lastAttackTime = characterStats.attactData.coolDown;
                        //�����ж�
                        characterStats.isCritical = Random.value < characterStats.attactData.criticalChance;
                        //ִ�й���
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

    void Attack()//ִ�й���
    {
        transform.LookAt(attackTarget.transform);//���Ź���Ŀ��
        if (TargetInSkillRange())
        {
            //���ܹ�������
            anim.SetTrigger("Skill");
        }
        else if (TargetInAttackRange())
        {
            //����������
            anim.SetTrigger("Attack");
        } 
    }

    bool FoundPlayer()//�ڿ��ӷ�Χ��Ѱ��Player
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

    bool TargetInAttackRange()//�Ƿ��ܽ��н����빥��
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attactData.attackRange;
        else
            return false;
    }

    bool TargetInSkillRange()//�Ƿ��ܽ���Զ���빥��
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attactData.skillRange;
        else
            return false;
    }

    void GetNewWayPoint()//������Ѳ�߷�Χ�ڵ�һ����
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;

    }

    private void OnDrawGizmosSelected()//�ڶ���ѡ��ʱ����Gizmos
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);//����Ұ��Χ
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget != null && transform.isFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();//��ʱ����
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        //��ʤ����
        //ֹͣ�����ƶ�
        //ֹͣAgent
        anim.SetBool("Win", true);
        playerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }


}