using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 25;//���ɵ���
    public GameObject rockPrefab;//ʯͷ
    public Transform handPos;//��ʯͷʱ�ֵ�����

    public void KickOff()//���ɷ���������˺�
    {
        if (attackTarget != null && transform.isFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();

            //��û��ɵķ���
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            //direction.Normalize(); 
            targetStats.GetComponent<NavMeshAgent>().isStopped = true;
            targetStats.GetComponent<NavMeshAgent>().velocity = direction * kickForce;
            targetStats.GetComponent<Animator>().SetTrigger("Dizzy");
            //�����˺�
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    //Animation Event
    public void ThrowRock()//��ʯͷ
    {
            var rock = Instantiate(rockPrefab, handPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
    }
}
