using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINameBar : MonoBehaviour {

    public Text avatarName;//玩家名字
    public TextMeshProUGUI avatarLevel;//玩家等级
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
            this.avatarLevel.text = string.Format("Lv:[{0}]", character.Info.Level);
        }
    }
}
