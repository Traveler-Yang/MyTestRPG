﻿using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Entities;
using Managers;
using System.Linq;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public UnityAction<Character> OnCharacterEnter;

        public UnityAction<Character> OncharacterLeave;

        public CharacterManager()
        {

        }

        public void Dispose()
        {
            
        }

        public void Init()
        {

        }

        public void Clear()
        {
            int[] keys = this.Characters.Keys.ToArray();
            foreach (var key in keys)
            {
                this.RemoveCharacter(key);
            }
            this.Characters.Clear();
        }
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="cha"></param>
        public void AddCharacter(NCharacterInfo cha)
        {
            Debug.LogFormat("AddCharacter: {0} : {1} Map:{2} Entity:{3}", cha.Id, cha.Name, cha.mapId, cha.Entity.String());
            Character character = new Character(cha);
            this.Characters[cha.EntityId] = character;
            EntityManager.Instance.AddEntity(character);

            if (OnCharacterEnter != null)
            {
                OnCharacterEnter(character);
            }
        }
        /// <summary>
        /// 移除玩家
        /// </summary>
        /// <param name="entityId"></param>
        public void RemoveCharacter(int entityId)
        {
            Debug.LogFormat("RemoveCharacter:{0}",entityId);

            if (this.Characters.ContainsKey(entityId))
            {
                EntityManager.Instance.RemoveEntity(this.Characters[entityId]);
                if (OncharacterLeave != null)
                {
                    OncharacterLeave(this.Characters[entityId]);
                }
                this.Characters.Remove(entityId);
            }
        }

        /// <summary>
        /// 根据id获取角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Character GetCharacter(int id)
        {
            Character character;
            this.Characters.TryGetValue(id, out character);
            return character;
        }
    }
}
