using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    class GuildService : Singleton<GuildService>, IDisposable
    {

        public GuildService()
        {
            //MessageDistributer.Instance.Subscribe<GuildCreatResponse>(this.OnTempInviteRequest);
            //MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnTempInviteResponse);
            //MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnTempInfo);
            //MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnTempLeave);
            //MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnTempDisband);
        }
        public void Dispose()
        {
            //MessageDistributer.Instance.Unsubscribe<TempInviteRequest>(this.OnTempInviteRequest);
            //MessageDistributer.Instance.Unsubscribe<TempInviteResponse>(this.OnTempInviteResponse);
            //MessageDistributer.Instance.Unsubscribe<TempInfoResponse>(this.OnTempInfo);
            //MessageDistributer.Instance.Unsubscribe<TempLeaveResponse>(this.OnTempLeave);
            //MessageDistributer.Instance.Unsubscribe<TempDisbandTempResponse>(this.OnTempDisband);
        }

        public void Init()
        {

        }

        public void SendGuildCreate(string text1, string text2)
        {
            
        }
    }
}
