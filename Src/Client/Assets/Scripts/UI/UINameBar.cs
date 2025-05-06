using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINameBar : MonoBehaviour {

    public Text avatarName;//玩家名字
    public Text avatarLevel;//玩家等级
	//public Image avatarIcon;//玩家头像

	public Character character;//当前的实体角色对象

    void Start () 
	{
		if (this.character != null)
		{
			
		}
	}

	void Update () 
	{
        this.UpdateInfo();

        this.transform.forward = Camera.main.transform.forward;//让信息条看向相机的位置
    }

    void UpdateInfo()
    {
        if (this.character != null)
        {
            string name = this.avatarName.text = character.Name;
            if (name != this.avatarName.name)
            {
                this.avatarName.text = name;
            }
            this.avatarLevel.text = character.Info.Level.ToString();
        }
    }
}
