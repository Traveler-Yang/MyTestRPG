using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Services
{
    class TempService : Singleton<TempService>, IDisposable
    {

        public TempService()
        {
            //MessageDistributer.Instance.Subscribe<TempInviteRequest>(this.OnTempInviteRequest);
            //MessageDistributer.Instance.Subscribe<TempInviteResponse>(this.OnTempInviteResponse);
            //MessageDistributer.Instance.Subscribe<TempInfoRequest>(this.OnTempInfo);
            //MessageDistributer.Instance.Subscribe<TempLeaveRequest>(this.OnTempLeave);
        }

        public void Dispose()
        {
            //MessageDistributer.Instance.Subscribe<TempInviteRequest>(this.OnTempInviteRequest);
            //MessageDistributer.Instance.Subscribe<TempInviteResponse>(this.OnTempInviteResponse);
            //MessageDistributer.Instance.Subscribe<TempInfoRequest>(this.OnTempInfo);
            //MessageDistributer.Instance.Subscribe<TempLeaveRequest>(this.OnTempLeave);
        }

        public void SendTempInviteRequest(int to_id, string to_name)
        {
            Debug.Log("SendTempInviteRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.tempInviteReq = new TempInviteRequest();
            message.Request.tempInviteReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.tempInviteReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.tempInviteReq.ToId = to_id;
            message.Request.tempInviteReq.ToName = to_name;
            //message.Request.tempInviteReq.TeamId
        }
    }
}
