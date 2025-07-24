using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : UIWindow
{
    public Toggle toggleMusic;//音乐开关
    public Toggle toggleSound;//音效开关

    public Slider sliderMusic;//音乐滑块
    public Slider sliderSound;//音效滑块

    private bool isInit = false;

    private void Start()
    {
        isInit = true;

        this.toggleMusic.isOn = Config.MusicOn;
        this.toggleSound.isOn = Config.SoundOn;
        this.sliderMusic.value = Config.MusicVolume;
        this.sliderSound.value = Config.SoundVolume;

        isInit = false;
    }

    /// <summary>
	/// 返回角色选择按钮
	/// </summary>
	public void BackToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");//切换场景为角色选择
        SoundManager.Instance.PlaySound(SoundDefine.Music_Select);//播放角色选择音乐
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

    public override void OnYesClick()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
        PlayerPrefs.Save();
        base.OnYesClick();
    }

    public void MusicVolume(float vol)
    {
        if (isInit) return; // 避免在初始化时触发事件
        Config.MusicVolume = (int)vol;
        PlaySound();
    }

    public void MusicToggle(bool on)
    {
        if (isInit) return; // 避免在初始化时触发事件
        Config.MusicOn = on;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
    }

    public void SoundVolume(float vol)
    {
        if (isInit) return; // 避免在初始化时触发事件
        Config.SoundVolume = (int)vol;
        PlaySound();
    }

    public void SoundToggle(bool on)
    {
        if (isInit) return; // 避免在初始化时触发事件
        Config.SoundOn = on;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Btn_1);
    }

    float lastPlay = 0;
    private void PlaySound()
    {
        if (Time.realtimeSinceStartup - lastPlay > 0.1)
        {
            lastPlay = Time.realtimeSinceStartup;
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        }
    }
}
