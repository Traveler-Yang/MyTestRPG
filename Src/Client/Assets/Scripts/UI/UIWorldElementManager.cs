using Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldElementManager : MonoSingleton<UIWorldElementManager> {

	public GameObject nameBarPrefab;//角色头顶的信息条

	private Dictionary<Transform, GameObject> elements = new Dictionary<Transform, GameObject>();

	void Start () {
		
	}
	
	
	void Update () {

	}

	public void AddCharacterNameBar(Transform owner, Character character)
	{
		GameObject goNameBar = Instantiate(nameBarPrefab, this.transform);//默认生成在当前管理器的根节点下
		goNameBar.name = "NameBar" + character.entityId;//给每个信息条的游戏对象添加名字 后方是当前角色对象的ID
		goNameBar.GetComponent<UIWorldElement>().owner = owner;
        goNameBar.GetComponent<UINameBar>().character = character;
		goNameBar.SetActive(true);
		this.elements[owner] = goNameBar;
    }

    public void RemoveCharacterNameBar(Transform owner)
    {
		if(this.elements.ContainsKey(owner))
        {
			Destroy(this.elements[owner]);
            this.elements.Remove(owner);
        }
    }
}
