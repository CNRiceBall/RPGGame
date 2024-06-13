using UnityEngine;

[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttactData_SO : ScriptableObject
{
    public float attackRange;//攻击距离

    public float skillRange;//远程攻击距离

    public float coolDown;//冷却时间

    public int minDamge;//最小攻击数值

    public int maxDamge;//最大攻击数值

    public float criticalMultiplier;//暴击加成

    public float criticalChance;//暴击率
} 