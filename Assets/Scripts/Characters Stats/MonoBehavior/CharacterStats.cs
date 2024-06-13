using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;

    public CharacterData_SO templateData;//ģ������

    public CharacterData_SO characterData;

    public AttactData_SO attactData;

    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (templateData != null) 
            characterData = Instantiate(templateData);//����һ��ģ������
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get//��
        {
            if (characterData != null)
                return characterData.maxHealth;
            else return 0;
        }
        set//д
        {
            characterData.maxHealth = value;//���������ֵ
        }
    }
    public int CurrentHealth
    {
        get//��
        {
            if (characterData != null)
                return characterData.currentHealth;
            else return 0;
        }
        set//д
        {
            characterData.currentHealth = value;//���������ֵ
        }
    }
    public int BaseDefence
    {
        get//��
        {
            if (characterData != null)
                return characterData.baseDefence;
            else return 0;
        }
        set//д
        {
            characterData.baseDefence = value;//���������ֵ
        }
    }
    public int CurrentDefence
    {
        get//��
        {
            if (characterData != null)
                return characterData.currentDefence;
            else return 0;
        }
        set//д
        {
            characterData.currentDefence = value;//���������ֵ
        }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);//��֤�˺������Ǹ�ֵ
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);//��֤Ѫ�������Ǹ�ֵ


        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        //TODO:Update UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //TODO:����Update
        if (CurrentHealth <= 0)//������������
        {
            attacker.characterData.UpdateExp(characterData.killPoint);//��������������ֵ
        }
    }

    public void TakeDamage(int damage, CharacterStats defener)//�������ߵ����������
    {
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);//��֤�˺������Ǹ�ֵ
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);//��֤Ѫ�������Ǹ�ֵ
        //Update UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);//���²���
        //����Update
        if (CurrentHealth <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
        }
    }

    private int CurrentDamage()//��ǰ�˺�
    {
        float coreDamage = UnityEngine.Random.Range(attactData.minDamge, attactData.maxDamge);//�����˺�

        if (isCritical)//����
        {
            coreDamage *= attactData.criticalMultiplier;
        }

        return (int)coreDamage;
    }
 
    #endregion
}