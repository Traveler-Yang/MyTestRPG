using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindow : MonoBehaviour
{
    private Animator animator;
    public virtual System.Type Type { get { return this.GetType(); } }

    /// <summary>
    /// UI窗口的默认选项类型
    /// </summary>
    public enum UIWindowResult
    {
        None,
        Yes,
        No,
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 关闭UI窗口
    /// </summary>
    /// <param name="result"></param>
    public void Close(UIWindowResult result = UIWindowResult.None)
    {
        UIManager.Instance.Colse(this.Type);
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
}
