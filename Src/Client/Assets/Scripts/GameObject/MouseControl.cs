using UnityEngine;

public class MouseControl : MonoSingleton<MouseControl>
{
    private bool uiMode = false;
    private int uiPanelCount = 0; // 当前打开的UI数量

    public bool justSwitched = false; // 🚨 新增标志位

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
        justSwitched = true; // 进入UI模式时标记
    }

    public void ExitUIMode()
    {
        uiMode = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        justSwitched = true; // 退出UI模式时标记
    }

    public bool IsInUIMode() => uiMode;

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
}
