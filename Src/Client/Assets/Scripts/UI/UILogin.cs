using Services;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;

public class UILogin : MonoBehaviour {

	public InputField username;//账号
	public InputField password;//密码
	public Button loginButton;//登录按钮
	void Start () {
		UserService.Instance.OnLogin = OnLogin;
	}
	
	
	void Update () {
		
	}

	public void OnLoginClickButton()
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
		UserService.Instance.SendLogin(username.text, password.text);
    }
    void OnLogin(Result result, string msg)
    {
        if (result == Result.Success)
        {
            SceneManager.Instance.LoadScene("CharSelect");
        }
        else
        {
            MessageBox.Show(msg, "错误", MessageBoxType.Error);
        }
    }
}
