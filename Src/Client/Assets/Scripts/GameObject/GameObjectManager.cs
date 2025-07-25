﻿using Entities;
using Managers;
using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectManager : MonoSingleton<GameObjectManager>
{

    Dictionary<int, GameObject> Characters = new Dictionary<int, GameObject>();
    // Use this for initialization
    protected override void OnStart()
    {
        StartCoroutine(InitGameObjects());
        CharacterManager.Instance.OnCharacterEnter += OnCharacterEnter;
        CharacterManager.Instance.OncharacterLeave += OnCharacterLeave;
    }


    private void OnDestroy()
    {
        CharacterManager.Instance.OnCharacterEnter -= OnCharacterEnter;
        CharacterManager.Instance.OncharacterLeave -= OnCharacterLeave;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCharacterEnter(Character cha)
    {
        CreateCharacterObject(cha);
    }
    void OnCharacterLeave(Character character)
    {
        //判断角色是否还存在
        //如果已经不存在了，则代表已经删除了，返回
        //如果还存在，则删除这个角色
        if (!Characters.ContainsKey(character.entityId))
            return;

        if (Characters[character.entityId] != null)
        {
            Destroy(Characters[character.entityId]);
            this.Characters.Remove(character.entityId);
        }
    }

    IEnumerator InitGameObjects()
    {
        foreach (var cha in CharacterManager.Instance.Characters.Values)
        {
            CreateCharacterObject(cha);
            yield return null;
        }
    }

    private void CreateCharacterObject(Character character)
    {
        if (!Characters.ContainsKey(character.entityId) || Characters[character.entityId] == null)
        {
            Object obj = Resloader.Load<Object>(character.Define.Resource);
            if (obj == null)
            {
                Debug.LogErrorFormat("Character[{0}] Resource[{1}] not existed.", character.Define.TID, character.Define.Resource);
                return;
            }
            GameObject go = (GameObject)Instantiate(obj, this.transform);
            go.name = "Character_" + character.Id + "_" + character.Name;
            Characters[character.entityId] = go;

            UIWorldElementManager.Instance.AddCharacterNameBar(go.transform, character);
        }
        this.InitGameObject(Characters[character.entityId], character);
    }

    private void InitGameObject(GameObject go,Character character)
    {
        go.transform.position = GameObjectTool.LogicToWorld(character.position);
        go.transform.forward = GameObjectTool.LogicToWorld(character.direction);
        //Characters[character.Info.Id] = go;

        EntityContorller ec = go.GetComponent<EntityContorller>();
        if (ec != null)
        {
            ec.entity = character;
            ec.isPlayer = character.IsCurrentPlayer;
            ec.Ride(character.Info.Ride);
        }

        PlayerInputContorller pc = go.GetComponent<PlayerInputContorller>();
        if (pc != null)
        {
            if (character.IsCurrentPlayer)
            {
                User.Instance.CurrentCharacterObject = pc;
                MainPlayerCamera.Instance.player = go;
                pc.enabled = true;
                pc.character = character;
                pc.entityContorller = ec;
            }
            else
            {
                pc.enabled = false;
            }
        }
    }

    public RideController LoadRide(int rideId, Transform parent)
    {
        var rideDefine = DataManager.Instance.Rides[rideId];
        Object obj = Resloader.Load<Object>(rideDefine.Resource);
        if (obj == null)
        {
            Debug.LogErrorFormat("Ride[{0}] Resource[{1}] not existed.", rideDefine.ID, rideDefine.Resource);
            return null;
        }
        GameObject go = (GameObject)Instantiate(obj, parent);
        go.name = "Ride_" + rideDefine.ID + "_" + rideDefine.Name;
        return go.GetComponent<RideController>();
    }
}

