using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class MiniMapManager : Singleton<MiniMapManager>
    {
        public UIMiniMap miniMap;

        /// <summary>
        /// 当前地图的包围盒
        /// </summary>
        private Collider miniMapBoundingBox;
        public Collider MiniMapBoundingBox
        {
            get
            {
                if (miniMapBoundingBox == null)
                {
                    miniMapBoundingBox = GameObject.Find("MiniMapBoundingBox").GetComponent<Collider>();
                }
                return miniMapBoundingBox;
            }
        }

        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacterObject == null)
                {
                    return null;
                }
                return User.Instance.CurrentCharacterObject.transform;
            }
        }
        /// <summary>
        /// 加载当前地图的迷你地图图片
        /// </summary>
        /// <returns></returns>
        public Sprite LoadCurrentMiniMap()
        {
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
        }
        /// <summary>
        /// 更新小地图
        /// </summary>
        /// <param name="miniMapBoundingBox">要更新的包围盒</param>

        public void UpdateMiniMap(Collider miniMapBoundingBox)
        {
            this.miniMapBoundingBox = miniMapBoundingBox;
            if (miniMap != null)
                miniMap.UpdateMap();
        }
    }
}
