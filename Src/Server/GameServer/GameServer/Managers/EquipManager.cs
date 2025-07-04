using GameServer.Entities;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Services;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
        {
            //先判断数据中有没有这件装备
            //如果没有，则直接返回Failed
            Character character = sender.Session.Character;
            if (!character.ItemManager.Items.ContainsKey(itemId))
                return Result.Failed;

            UpdateEquip(character.TChar.Equips, slot, itemId, isEquip);

            DBService.Instance.Save();
            return Result.Success;
        }

        unsafe void UpdateEquip(byte[] equipData, int slot, int itemId, bool isEquip)
        {
            fixed (byte* pt = equipData)
            {
                int* slotId = (int*)(pt + slot * sizeof(int));
                if (isEquip)
                    *slotId = itemId;
                else
                    *slotId = 0;
            }
        }
    }
}
