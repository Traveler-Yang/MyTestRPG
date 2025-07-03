using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITemp : UIWindow
{
    public Text tempTitle;//标题
    public List<UITempItem> members;//组队列表
    public ListView list;//列表
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 显示组队界面
    /// </summary>
    /// <param name="show">是否显示</param>
    public void ShowTeam(bool show)
    {
        if (show)
            UIManager.Instance.Show<UITemp>();
        else
            return;
    }

}
