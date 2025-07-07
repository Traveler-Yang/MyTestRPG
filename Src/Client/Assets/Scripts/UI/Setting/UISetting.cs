using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow
{
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    /// <summary>
	/// 返回角色选择按钮
	/// </summary>
	public void BackToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");//切换场景为角色选择
        Services.UserService.Instance.SendGameLeave();//发送角色离开的消息到服务器
    }
}
