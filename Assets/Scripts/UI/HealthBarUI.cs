using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthBarUIPrefab;//����Ѫ��
    public Transform barPoint;//Ѫ��������Ŀ��λ�ã�Ѫ����Ҫʵʱ�����������
    public bool alwaysVisible;//�Ƿ��ǳ��ÿɼ�
    public float visibleTime;//���ӻ�ʱ��      
    private float timeLeft; //Ѫ��ʣ�����ʾ��ʱ��

    Image healthSlider;//������
    Transform UIbar;//Ѫ����λ��
    Transform cam;//���λ��

    CharacterStats currentSatas;//��ǰ��CharacterStats�õ�����Ѫ��

    void Awake()
    {
        currentSatas = GetComponent<CharacterStats>();

        currentSatas.UpdateHealthBarOnAttack += UpdateHealthBar;
    }

    private void OnEnable()
    {
        cam = Camera.main.transform;

        foreach (Canvas canvas in FindObjectsOfType<Canvas>())//�ҵ�����Canvas���
        {
            if (canvas.renderMode == RenderMode.WorldSpace)//�ж��Ƿ�����������ϵ
            {
                UIbar = Instantiate(healthBarUIPrefab, canvas.transform).transform;//�Ƚ�Ѫ�����ɵ��������굱��
                timeLeft = visibleTime;//Ѫ����ʾ������ʱ�����¼�ʱ

                healthSlider = UIbar.GetChild(0).GetComponent<Image>();//�õ�������
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    private void UpdateHealthBar(int currentHealth, int maxHealth)//����Ѫ��
    {
        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);

        UIbar.gameObject.SetActive(true);//�ܵ�������ʱ��Ѫ��ǿ����Ϊ�ɼ�
        timeLeft = visibleTime;//Ѫ����ʾ������ʱ�����¼�ʱ

        float sliderPercent = (float)currentHealth / maxHealth;//������fillAmount����ֵ
        healthSlider.fillAmount = sliderPercent;
    }

    void LateUpdate()
    {
        if (UIbar != null)//��ֹѪ����ʧ�˶�����
        {
            UIbar.position = barPoint.position;//��Ѫ������ʵʱ����barPointĿ������
            UIbar.forward = -cam.forward;//��֤Ѫ��ʵʱ��׼�����

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