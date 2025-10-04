using UnityEngine;

public class MouseControl : MonoSingleton<MouseControl>
{
    private bool uiMode = false;
    private int uiPanelCount = 0; // 当前打开的UI数量
    private bool altHeld = false; // 是否按住了 Alt
    public bool justSwitched = false;

    protected override void OnStart()
    {
        ExitUIMode();
    }

    void Update()
    {
        // Alt 键切换（临时解锁）
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            altHeld = true;
            UpdateCursorState();
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt) || Input.GetKeyUp(KeyCode.RightAlt))
        {
            altHeld = false;
            UpdateCursorState();
        }
    }

    private void UpdateCursorState()
    {
        bool shouldBeInUI = uiPanelCount > 0 || altHeld;
        if (shouldBeInUI != uiMode)
        {
            uiMode = shouldBeInUI;
            justSwitched = true;

            if (uiMode)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
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
        UpdateCursorState();
    }

    // UI 关闭时调用
    public void OnUIClose()
    {
        uiPanelCount = Mathf.Max(0, uiPanelCount - 1);
        UpdateCursorState();
    }
}
