using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Character Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;//���Ѫ��

    public int currentHealth;//��ǰѪ��

    public int baseDefence;//��������

    public int currentDefence;//��ǰ����

    [Header("Kill")]
    public int killPoint;//���������

    [Header("Level")]
    public int currentLevel;//��ǰ�ȼ�

    public int maxLevel;//��ߵȼ�

    public int baseExp;//������Ҫ���ܾ���ֵ

    public int currentExp;//��ǰ����ֵ

    public float levelBuff;//�ȼ����Լӳ�

    public float LevelMultiplier//�ȼ������ٷֱȼӳ�
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }
    public void UpdateExp(int point)//��������ֵ
    {
        currentExp += point;//��ȡ���˵ľ���ֵ

        if (currentExp >= baseExp)
        {
            LeveUp();//����
        }
    }

    private void LeveUp()//����
    {
        //�����������������ݷ���
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);//���������ᳬ����ߵȼ�
        baseExp += (int)(baseExp * LevelMultiplier);//��һ������ľ�������

        maxHealth = (int)(maxHealth * LevelMultiplier);//���Ѫ������
        currentHealth = maxHealth;//�ظ���Ѫ

        Debug.Log("LEVEL UP!" + currentLevel + "Max Health:" + maxHealth);
    }
}