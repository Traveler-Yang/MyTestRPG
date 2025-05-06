using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCity : MonoBehaviour {

	public Text avatarName;//玩家名字
	public Text avatarLevel;//玩家等级
	void Start () {
		this.UpdateAvatar();
	}

	void UpdateAvatar()
	{
		//将角色的名字赋值给UI显示
		this.avatarName.text = string.Format("{0} [{1}]", User.Instance.CurrentCharacter.Name, User.Instance.CurrentCharacter.Id);
		//将角色的等级赋值给UI显示
		this.avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
