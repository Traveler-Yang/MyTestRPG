using Common.Data;
using Models;
using Network;
using SkillBridge.Message;
using System;
using UnityEngine;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);//角色进入地图的消息
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);//离开地图

        }

        public int CurrentMapId { get; private set; }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);//取消订阅
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);
        }

        public void Init()
        {

        }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:{0} [{1}]", response.mapId, response.Characters.Count);
            foreach (var cha in response.Characters)
            {
                //判断当前列表中的角色是否是自己
                if (User.Instance.CurrentCharacter.Id == cha.Id)
                {
                    //当前角色切换地图
                    User.Instance.CurrentCharacter = cha;
                }
                CharacterManager.Instance.AddCharacter(cha);//将进入地图的所有角色，发送给角色管理器
            }
            if (CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }

        /// <summary>
        /// 角色离开地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {

        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                SceneManager.Instance.LoadScene(map.Resource);
            }
            else
            {
                Debug.LogErrorFormat("EnterMap: Map {0} not extsted",mapId);
            }
        }
    }
}
