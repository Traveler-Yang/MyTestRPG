using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class MiniMapManager : Singleton<MiniMapManager>
    {
        public Sprite LoadCurrentMiniMap()
        {
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
        }
    }
}
