using UnityEngine;

public class MouseControl : MonoSingleton<MouseControl>
{
    private bool uiMode = false;
    private int uiPanelCount = 0; // 当前打开的UI数量

    protected override void OnStart()
    {
        this.ExitUIMode();
    }

    void Update()
    {
        // Alt 键切换（临时解锁）
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            EnterUIMode();
        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
            ExitUIMode();
    }

    public void EnterUIMode()
    {
        uiMode = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitUIMode()
    {
        // 如果有 UI 面板没关，不能退出 UI 模式
        if (uiPanelCount > 0) return;

        uiMode = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // UI 打开时调用
    public void OnUIOpen()
    {
        uiPanelCount++;
        EnterUIMode();
    }

    // UI 关闭时调用
    public void OnUIClose()
    {
        uiPanelCount = Mathf.Max(0, uiPanelCount - 1);
        if (uiPanelCount == 0)
        {
            ExitUIMode();
        }
    }

    public bool IsInUIMode()
    {
        return uiMode;
    }
}
