using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour {

	public GameObject[] characters;

	private int currectCharacter = 0;
	public int CurrectCharacter 
	{
		get
		{
			return currectCharacter;
		}
		set
		{
			currectCharacter = value;
			UpdateCharacter();

        }
	}
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void UpdateCharacter()
	{
		for (int i = 0; i < 3; i++)
		{
			characters[i].SetActive(i == currectCharacter);
		}
	}
}
