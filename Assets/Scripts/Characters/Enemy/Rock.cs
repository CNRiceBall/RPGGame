using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Rock;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer, HitEnemy, HitNothing }

    private Rigidbody rb;

    public RockStates rockStates;//ö�ٱ�����ʯͷ��״̬

    [Header("Basic Settings")]

    public float force;//��ǰ�������

    public int damage;

    public GameObject target;//ʯͷ��Ŀ��

    private Vector3 direction;//���еķ���

    public GameObject breakEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;

        rockStates = RockStates.HitPlayer;
        FlyToTarget();//����Ŀ���
    }

    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)//ʯͷ�ٶ�С��1���ΪHitNothing״̬
        {
            rockStates = RockStates.HitNothing;
        }
    }

    public void FlyToTarget()//����Ŀ���
    {
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);//˲��ĳ����
    }
    private void OnCollisionEnter(Collision collision)
    {
        switch (rockStates)//����ʯͷ��ͬ��״̬
        {
            case RockStates.HitPlayer://����Player������˺�
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;//ֹͣ�ƶ�
                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;//����

                    collision.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");//ѣ��
                                                                                      //����˺�
                    var otherStats = collision.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);

                    rockStates = RockStates.HitNothing;
                }
                break;

            case RockStates.HitEnemy://��������״̬
                if (collision.gameObject.GetComponent<Golem>())//ȷ���Ƿ���ʯͷ��
                {
                    //��ʯͷ�˲����������˺�
                    var otherStats = collision.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);//������Ч
                    Destroy(gameObject);
                }
                break;
        }
    }
}
