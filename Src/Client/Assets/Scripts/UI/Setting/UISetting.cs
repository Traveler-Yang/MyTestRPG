using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow
{
    /// <summary>
	/// 返回角色选择按钮
	/// </summary>
	public void BackToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");//切换场景为角色选择
        UserService.Instance.SendGameLeave();//发送角色离开的消息到服务器
    }

    /// <summary>
    /// 设置分辨率
    /// </summary>
    public void OnClickSetResolution()
    {

    }

    /// <summary>
    /// 退出游戏按钮
    /// </summary>
    public void OnClickExitGame()
    {
        UserService.Instance.SendGameLeave(true);
    }
}
