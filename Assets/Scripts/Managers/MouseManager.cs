using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point, doorway, attack, target, arrow;

    RaycastHit hitInfo; //保存射线碰撞到的物体的相关信息

    public event Action<Vector3> OnMouseClicked;

    public event Action<GameObject> OnEnemyClicked;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        SetCursorTexture();//设置指针的贴图
        MouseControl();//返回鼠标左键点击返回值
    }

    void SetCursorTexture() //设置指针的贴图
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            //切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto); //偏移（16，16）
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto); //偏移（16，16）
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto); //偏移（16，16）
                    break;
                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto); //偏移（16，16）
                    break;
            }
        }
    }

    void MouseControl()//返回鼠标左键点击返回值
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClicked?.Invoke(hitInfo.point); //当前OnMouseClicked事件如果不为空，将点击到地面上的坐标传回给这个事件(执行所有加入到onMouseClicked的函数方法)
            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject); //当前OnEnemyClicked事件如果不为空，将点击到敌人的gameObject传回给这个事件(执行所有加入到OnEnemyClicked的函数方法)
            }
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject); //当前OnEnemyClicked事件如果不为空，将点击到敌人的gameObject传回给这个事件(执行所有加入到OnEnemyClicked的函数方法)
            }
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                OnMouseClicked?.Invoke(hitInfo.point); //当前OnMouseClicked事件如果不为空，将点击到地面上的坐标传回给这个事件(执行所有加入到onMouseClicked的函数方法)
            }
        }
    }
}