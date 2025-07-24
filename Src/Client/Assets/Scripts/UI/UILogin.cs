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
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        if (string.IsNullOrEmpty(username.text))
		{
            MessageBox.Show("请输入账号", "提示", MessageBoxType.Information);
            return;
        }
		if (string.IsNullOrEmpty(password.text))
		{
            MessageBox.Show("请输入密码", "提示", MessageBoxType.Information);
            return;
        }
        UserService.Instance.SendLogin(username.text, password.text);
    }
    void OnLogin(Result result, string msg)
    {
        if (result == Result.Success)
        {
            SceneManager.Instance.LoadScene("CharSelect");
            SoundManager.Instance.PlayMusic(SoundDefine.Music_Select);
        }
        else
        {
            MessageBox.Show(msg, "错误", MessageBoxType.Error);
        }
    }
}
