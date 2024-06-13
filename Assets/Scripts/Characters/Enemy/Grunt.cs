using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 10;//���ɵ���

    public void KickOff()//���ɷ���
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);

            //��û��ɵķ���
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();//��λ��

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.GetComponent<NavMeshAgent>().ResetPath();
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");//����ѣ�ζ���
        }
    }
}
