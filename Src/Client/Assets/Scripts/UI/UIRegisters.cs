using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;
using SkillBridge.Message;

public class UIRegisters : MonoBehaviour {

	public InputField username;//账号
	public InputField password;//密码
	public InputField verifyPassword;//确认密码
	public Button registersButton;//注册按钮

    void Start () {
		UserService.Instance.OnRegister = OnRegister;
	}
	
	
	void Update () {

    }

    public void OnRegister(Result result, string msg)
    {
        MessageBox.Show(string.Format("结果:{0} msg:{1}", result, msg));
    }

    public void OnRegistersClickButton()
    {
		if (string.IsNullOrEmpty(username.text))
		{
            MessageBox.Show("请输入账号");
            return;
		}
        if (string.IsNullOrEmpty(password.text))
        {
            MessageBox.Show("请输入密码");
            return;
        }
        if (string.IsNullOrEmpty(verifyPassword.text))
        {
            MessageBox.Show("请输入确认密码");
            return;
        }
        if (password.text != verifyPassword.text)
        {
            MessageBox.Show("两次输入的密码不一致");
            return;
        }
        UserService.Instance.SendRegister(username.text, password.text);
    }
}
