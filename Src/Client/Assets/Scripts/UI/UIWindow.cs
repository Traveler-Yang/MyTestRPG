using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow : MonoBehaviour
{
    public delegate void CloseHandler(UIWindow sender, UIWindowResult result);
    public event CloseHandler OnClose; // 窗口关闭事件

    public virtual System.Type Type { get { return this.GetType(); } }

    public GameObject Root;

    /// <summary>
    /// UI窗口的默认选项类型
    /// </summary>
    public enum UIWindowResult
    {
        None,
        Yes,
        No,
    }

    /// <summary>
    /// 关闭UI窗口
    /// </summary>
    /// <param name="result"></param>
    public void Close(UIWindowResult result = UIWindowResult.None)
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
        UIManager.Instance.Close(this.Type);
        if(this.OnClose != null)
            this.OnClose(this, result);
        this.OnClose = null; // 清除事件订阅，避免内存泄漏
    }

    /// <summary>
    /// 关闭UI窗口的取消按钮点击事件
    /// </summary>
    public virtual void OnCloseClick()
    {
        this.Close();
    }

    /// <summary>
    /// 关闭UI窗口的确认按钮点击事件
    /// </summary>
    public virtual void OnYesClick()
    {
        this.Close(UIWindowResult.Yes);
    }

    public virtual void OnNoClick()
    {
        this.Close(UIWindowResult.No);
        this.Close();
    }
}
