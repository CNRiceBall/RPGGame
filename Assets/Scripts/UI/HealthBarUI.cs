using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthBarUIPrefab;//敌人血条
    public Transform barPoint;//血条创建的目标位置，血条需要实时等于这个坐标
    public bool alwaysVisible;//是否是长久可见
    public float visibleTime;//可视化时间      
    private float timeLeft; //血条剩余可显示的时间

    Image healthSlider;//滑动条
    Transform UIbar;//血条的位置
    Transform cam;//相机位置

    CharacterStats currentSatas;//当前的CharacterStats拿到它的血量

    void Awake()
    {
        currentSatas = GetComponent<CharacterStats>();

        currentSatas.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())//找到所有Canvas组件
        {
            if (canvas.renderMode == RenderMode.WorldSpace)//判断是否是世界坐标系
            {
                UIbar = Instantiate(healthBarUIPrefab, canvas.transform).transform;//先将血条生成到世界坐标当中
                timeLeft = visibleTime;//血量显示出来的时候重新计时

                healthSlider = UIbar.GetChild(0).GetComponent<Image>();//拿到滑动条
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)//更新血条
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);

        UIbar.gameObject.SetActive(true);//受到攻击的时候将血条强行设为可见
        timeLeft = visibleTime;//血量显示出来的时候重新计时

        float sliderPercent = (float)currentHealth / maxHealth;//滑动条fillAmount参数值
        healthSlider.fillAmount = sliderPercent;
    }

    void LateUpdate()
    {
        if (UIbar != null)//防止血条消失了而报错
        {
            UIbar.position = barPoint.position;//让血条坐标实时等于barPoint目标坐标
            UIbar.forward = -cam.forward;//保证血条实时对准摄像机

            if (timeLeft <= 0 && !alwaysVisible)
            {
                UIbar.gameObject.SetActive(false);
            }
            else
            {
                timeLeft -= Time.deltaTime;
            }
        }
    }
}       