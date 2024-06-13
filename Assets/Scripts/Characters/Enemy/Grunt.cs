using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10;//击飞的力

    public void KickOff()//击飞方法
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);

            //获得击飞的方向
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();//单位化

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.GetComponent<NavMeshAgent>().ResetPath();
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");//播放眩晕动画
        }
    }
}
