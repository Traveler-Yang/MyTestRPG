using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using SkillBridge.Message;

namespace Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        /// <summary>
        /// 我在公会中的信息
        /// </summary>
        public NGuildMemberInfo MyMemberInfo;

        public NGuildInfo guildInfo;
        public void Init(NGuildInfo guild)
        {
            this.guildInfo = guild;
            if (guild == null)
            {
                MyMemberInfo = null;
                return;
            }
            foreach (var member in guild.Members)
            {
                //查找此公会中所有成员，找出自己，并给MyMemberInfo赋值
                if (member.characterId == User.Instance.CurrentCharacter.Id)
                {
                    MyMemberInfo = member;
                    break;
                }
            }
        }

        public bool HasGuild
        {
            get {  return this.guildInfo != null; }
        }
        /// <summary>
        /// 打开公会
        /// </summary>
        public void ShowGuild()
        {
            //判断有无公会
            if (this.HasGuild)
            {
                UIManager.Instance.Show<UIGuild>();
            }
            else//如果没有，则打开 创建or加入 的窗口
            {
                var win = UIManager.Instance.Show<UIGuildPopNoGuild>();
                win.OnClose += PopNoGuild_OnClose;
            }
        }

        private void PopNoGuild_OnClose(UIWindow sender, UIWindow.UIWindowResult result)
        {
            if (result == UIWindow.UIWindowResult.Yes)
            {
                //创建公会
                UIManager.Instance.Show<UIGuildPopCreate>();
            }
            else if (result == UIWindow.UIWindowResult.No)
            {
                //加入公会
                UIManager.Instance.Show<UIGuildList>();
            }
        }
    }
}
