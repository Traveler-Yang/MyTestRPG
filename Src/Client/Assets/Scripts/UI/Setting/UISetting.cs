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
	/// ���ؽ�ɫѡ��ť
	/// </summary>
	public void BackToCharSelect()
    {
        SceneManager.Instance.LoadScene("CharSelect");//�л�����Ϊ��ɫѡ��
        Services.UserService.Instance.SendGameLeave();//���ͽ�ɫ�뿪����Ϣ��������
    }
}
