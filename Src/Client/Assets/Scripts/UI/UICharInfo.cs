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
            this.charclass.text = info.Class.ToString();
            this.charName.text = info.Name;
        }
	}
	
	
	void Update () {
		
	}
}
