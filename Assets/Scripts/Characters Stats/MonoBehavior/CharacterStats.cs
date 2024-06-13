using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;

    public CharacterData_SO templateData;//模板数据

    public CharacterData_SO characterData;

    public AttactData_SO attactData;

    [HideInInspector]
    public bool isCritical;

    void Awake()
    {
        if (templateData != null) 
            characterData = Instantiate(templateData);//生成一份模板数据
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get//读
        {
            if (characterData != null)
                return characterData.maxHealth;
            else return 0;
        }
        set//写
        {
            characterData.maxHealth = value;//输入进来的值
        }
    }
    public int CurrentHealth
    {
        get//读
        {
            if (characterData != null)
                return characterData.currentHealth;
            else return 0;
        }
        set//写
        {
            characterData.currentHealth = value;//输入进来的值
        }
    }
    public int BaseDefence
    {
        get//读
        {
            if (characterData != null)
                return characterData.baseDefence;
            else return 0;
        }
        set//写
        {
            characterData.baseDefence = value;//输入进来的值
        }
    }
    public int CurrentDefence
    {
        get//读
        {
            if (characterData != null)
                return characterData.currentDefence;
            else return 0;
        }
        set//写
        {
            characterData.currentDefence = value;//输入进来的值
        }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker, CharacterStats defener)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defener.CurrentDefence, 0);//保证伤害不会是负值
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);//保证血量不会是负值


        if (attacker.isCritical)
        {
            defener.GetComponent<Animator>().SetTrigger("Hit");
        }
        //TODO:Update UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //TODO:经验Update
        if (CurrentHealth <= 0)//被攻击者死亡
        {
            attacker.characterData.UpdateExp(characterData.killPoint);//攻击者提升经验值
        }
    }

    public void TakeDamage(int damage, CharacterStats defener)//被攻击者调用这个方法
    {
        int currentDamage = Mathf.Max(damage - defener.CurrentDefence, 0);//保证伤害不会是负值
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);//保证血量不会是负值
        //Update UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);//更新参数
        //经验Update
        if (CurrentHealth <= 0)
        {
            GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
        }
    }

    private int CurrentDamage()//当前伤害
    {
        float coreDamage = UnityEngine.Random.Range(attactData.minDamge, attactData.maxDamge);//核心伤害

        if (isCritical)//暴击
        {
            coreDamage *= attactData.criticalMultiplier;
        }

        return (int)coreDamage;
    }
 
    #endregion
}