using Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISetting : UIWindow
{
    /// <summary>
	/// ���ؽ�ɫѡ��ť
	/// </summary>
	public void BackToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");//�л�����Ϊ��ɫѡ��
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
}
