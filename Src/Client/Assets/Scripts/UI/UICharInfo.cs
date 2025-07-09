using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharInfo : MonoBehaviour {

	public SkillBridge.Message.NCharacterInfo info;

	public Text charclass;//职业
	public Text charName;//昵称
	public Image highlight;//高亮图片

	public bool Selected//是否被选中
	{
		get { return highlight.IsActive(); }
		set
		{
			highlight.gameObject.SetActive(value);
		}
	}

	void Start () {
		if (info != null)
        {
			switch (info.Class)
			{
				case CharacterClass.Warrior:
					this.charclass.text = "战士";
                    break;
				case CharacterClass.Wizard:
                    this.charclass.text = "法师";
                    break;
				case CharacterClass.Archer:
                    this.charclass.text = "弓箭手";
                    break;
			}
			
            this.charName.text = info.Name;
        }
	}
	
	
	void Update () {
		
	}
}
