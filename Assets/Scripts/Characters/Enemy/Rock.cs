using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Rock;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer, HitEnemy, HitNothing }

    private Rigidbody rb;

    public RockStates rockStates;//枚举变量，石头的状态

    [Header("Basic Settings")]

    public float force;//向前冲击的力

    public int damage;

    public GameObject target;//石头的目标

    private Vector3 direction;//飞行的方向

    public GameObject breakEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;

        rockStates = RockStates.HitPlayer;
        FlyToTarget();//朝着目标飞
    }

    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)//石头速度小于1则变为HitNothing状态
        {
            rockStates = RockStates.HitNothing;
        }
    }

    public void FlyToTarget()//朝着目标飞
    {
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);//瞬间的冲击力
    }
    private void OnCollisionEnter(Collision collision)
    {
        switch (rockStates)//根据石头不同的状态
        {
            case RockStates.HitPlayer://击退Player并造成伤害
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;//停止移动
                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;//击退

                    collision.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");//眩晕
                                                                                      //造成伤害
                    var otherStats = collision.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);

                    rockStates = RockStates.HitNothing;
                }
                break;

            case RockStates.HitEnemy://反击敌人状态
                if (collision.gameObject.GetComponent<Golem>())//确认是否是石头人
                {
                    //给石头人产生反击的伤害
                    var otherStats = collision.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);//产生特效
                    Destroy(gameObject);
                }
                break;
        }
    }
}
