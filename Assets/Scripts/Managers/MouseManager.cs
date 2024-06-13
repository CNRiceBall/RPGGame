using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;

    RaycastHit hitInfo; //����������ײ��������������Ϣ

    public event Action<Vector3> OnMouseClicked;

    public event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        SetCursorTexture();//����ָ�����ͼ
        MouseControl();//�����������������ֵ
    }

    void SetCursorTexture() //����ָ�����ͼ
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            //�л������ͼ
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto); //ƫ�ƣ�16��16��
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto); //ƫ�ƣ�16��16��
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto); //ƫ�ƣ�16��16��
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto); //ƫ�ƣ�16��16��
                    break;
            }
        }
    }

    void MouseControl()//�����������������ֵ
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClicked?.Invoke(hitInfo.point); //��ǰOnMouseClicked�¼������Ϊ�գ�������������ϵ����괫�ظ�����¼�(ִ�����м��뵽onMouseClicked�ĺ�������)
            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject); //��ǰOnEnemyClicked�¼������Ϊ�գ�����������˵�gameObject���ظ�����¼�(ִ�����м��뵽OnEnemyClicked�ĺ�������)
            }
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject); //��ǰOnEnemyClicked�¼������Ϊ�գ�����������˵�gameObject���ظ�����¼�(ִ�����м��뵽OnEnemyClicked�ĺ�������)
            }
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                OnMouseClicked?.Invoke(hitInfo.point); //��ǰOnMouseClicked�¼������Ϊ�գ�������������ϵ����괫�ظ�����¼�(ִ�����м��뵽onMouseClicked�ĺ�������)
            }
        }
    }
}