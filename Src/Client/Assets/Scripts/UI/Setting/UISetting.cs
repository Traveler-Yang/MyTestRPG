using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISetting : UIWindow
{
    public Toggle toggleMusic;//���ֿ���
    public Toggle toggleSound;//��Ч����

    public Slider sliderMusic;//���ֻ���
    public Slider sliderSound;//��Ч����

    private void Start()
    {
        this.toggleMusic.isOn = Config.MusicOn;
        this.toggleSound.isOn = Config.SoundOn;
        this.sliderMusic.value = Config.MusicVolume;
        this.sliderSound.value = Config.SoundVolume;
    }

    /// <summary>
	/// ���ؽ�ɫѡ��ť
	/// </summary>
	public void BackToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");//�л�����Ϊ��ɫѡ��
        SoundManager.Instance.PlaySound(SoundDefine.Music_Select);//���Ž�ɫѡ������
        UserService.Instance.SendGameLeave();//���ͽ�ɫ�뿪����Ϣ��������
    }

    /// <summary>
    /// ���÷ֱ���
    /// </summary>
    public void OnClickSetResolution()
    {

    }

    /// <summary>
    /// �˳���Ϸ��ť
    /// </summary>
    public void OnClickExitGame()
    {
        UserService.Instance.SendGameLeave(true);
    }

    public override void OnYesClick()
    {
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
        PlayerPrefs.Save();
        base.OnYesClick();
    }

    public void MusicVolume(float vol)
    {
        Config.MusicVolume = (int)vol;
        PlaySound();
    }

    public void MusicToggle(bool on)
    {
        Config.MusicOn = on;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
    }

    public void SoundVolume(float vol)
    {
        Config.SoundVolume = (int)vol;
        PlaySound();
    }

    public void SoundToggle(bool on)
    {
        Config.SoundOn = on;
        SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Click);
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
