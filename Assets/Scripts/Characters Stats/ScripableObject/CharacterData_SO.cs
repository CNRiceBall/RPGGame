using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;//最大血量

    public int currentHealth;//当前血量

    public int baseDefence;//基础防御

    public int currentDefence;//当前防御

    [Header("Kill")]
    public int killPoint;//死亡经验点

    [Header("Level")]
    public int currentLevel;//当前等级

    public int maxLevel;//最高等级

    public int baseExp;//升级需要的总经验值

    public int currentExp;//当前经验值

    public float levelBuff;//等级属性加成

    public float LevelMultiplier//等级提升百分比加成
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }
    public void UpdateExp(int point)//提升经验值
    {
        currentExp += point;//获取敌人的经验值

        if (currentExp >= baseExp)
        {
            LeveUp();//升级
        }
    }

    private void LeveUp()//升级
    {
        //所有你想提升的数据方法
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);//升级，不会超过最高等级
        baseExp += (int)(baseExp * LevelMultiplier);//下一级所需的经验提升

        maxHealth = (int)(maxHealth * LevelMultiplier);//最大血量提升
        currentHealth = maxHealth;//回复满血

        Debug.Log("LEVEL UP!" + currentLevel + "Max Health:" + maxHealth);
    }
}