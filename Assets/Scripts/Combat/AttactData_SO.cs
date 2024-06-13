using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttactData_SO : ScriptableObject
{
    public float attackRange;//��������

    public float skillRange;//Զ�̹�������

    public float coolDown;//��ȴʱ��

    public int minDamge;//��С������ֵ

    public int maxDamge;//��󹥻���ֵ

    public float criticalMultiplier;//�����ӳ�

    public float criticalChance;//������
} 