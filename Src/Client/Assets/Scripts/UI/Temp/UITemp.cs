using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITemp : UIWindow
{
    public Text tempTitle;//����
    public List<UITempItem> members;//����б�
    public ListView list;//�б�
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// ��ʾ��ӽ���
    /// </summary>
    /// <param name="show">�Ƿ���ʾ</param>
    public void ShowTeam(bool show)
    {
        if (show)
            UIManager.Instance.Show<UITemp>();
        else
            return;
    }

}
