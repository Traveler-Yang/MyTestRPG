using UnityEngine;

public class MouseControl : MonoBehaviour
{
    private bool uiMode = false;

    void Update()
    {
        // 按下 Alt 键进入 UI 模式
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            EnterUIMode();
        }

        // 松开 Alt 键恢复游戏模式
        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            ExitUIMode();
        }
    }

    void EnterUIMode()
    {
        uiMode = true;
        Cursor.lockState = CursorLockMode.None;  // 解锁鼠标
        Cursor.visible = true;                  // 显示鼠标
        // 这里可以通知 InputSystem/角色控制器暂停相机输入
    }

    void ExitUIMode()
    {
        uiMode = false;
        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        Cursor.visible = false;                  // 隐藏鼠标
        // 恢复相机/角色控制
    }

    public bool IsInUIMode()
    {
        return uiMode;
    }
}
