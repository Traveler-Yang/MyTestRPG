using Managers;
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
        public void Init()
        {

        }

        public TempService()
        {
            MessageDistributer.Instance.Subscribe<TempInviteRequest>(this.OnTempInviteRequest);
            MessageDistributer.Instance.Subscribe<TempInviteResponse>(this.OnTempInviteResponse);
            MessageDistributer.Instance.Subscribe<TempInfoResponse>(this.OnTempInfo);
            MessageDistributer.Instance.Subscribe<TempLeaveResponse>(this.OnTempLeave);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Subscribe<TempInviteRequest>(this.OnTempInviteRequest);
            MessageDistributer.Instance.Subscribe<TempInviteResponse>(this.OnTempInviteResponse);
            MessageDistributer.Instance.Subscribe<TempInfoResponse>(this.OnTempInfo);
            MessageDistributer.Instance.Subscribe<TempLeaveResponse>(this.OnTempLeave);
        }

        /// <summary>
        /// 发送组队请求
        /// </summary>
        /// <param name="friendId"></param>
        /// <param name="friendName"></param>
        public void SendTempInviteRequest(int friendId, string friendName)
        {
            Debug.Log("[Client]：SendTempInviteRequest");
            NetClient.Instance.Request.tempInviteReq = new TempInviteRequest();
            NetClient.Instance.Request.tempInviteReq.FromId = User.Instance.CurrentCharacter.Id;
            NetClient.Instance.Request.tempInviteReq.FromName = User.Instance.CurrentCharacter.Name;
            NetClient.Instance.Request.tempInviteReq.ToId = friendId;
            NetClient.Instance.Request.tempInviteReq.ToName = friendName;
            NetClient.Instance.SendMessage(NetClient.Instance.request);
        }

        /// <summary>
        /// 收到组队是否同意响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnTempInviteResponse(object sender, TempInviteResponse message)
        {
            if (message.Result == Result.Success)
                MessageBox.Show(string.Format("[{0}] 加入了队伍", message.Request.ToName), "邀请组队成功");
            else
                MessageBox.Show(message.Errormsg, "邀请组队失败");
        }

        /// <summary>
        /// 发送是否同意请求
        /// </summary>
        /// <param name="accept">是否同意</param>
        /// <param name="request">请求信息</param>
        public void SendTempInviteResponse(bool accept, TempInviteRequest request)
        {
            Debug.Log("[Client]：SendTempInviteResponse");
            NetClient.Instance.Request.tempInviteRes = new TempInviteResponse();
            NetClient.Instance.Request.tempInviteRes.Result = accept ? Result.Success : Result.Failed;
            NetClient.Instance.Request.tempInviteRes.Errormsg = accept ? "对方同意了组队邀请" : "对方拒绝了组队邀请";
            NetClient.Instance.Request.tempInviteRes.Request = request;
            NetClient.Instance.SendMessage(NetClient.Instance.request);
        }

        /// <summary>
        /// 收到组队邀请响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnTempInviteRequest(object sender, TempInviteRequest request)
        {
            var confirm = MessageBox.Show(string.Format("[{0}] 邀请你加入队伍", request.FromName), "组队邀请", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () =>
            {
                //接受
                this.SendTempInviteResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                //拒绝
                this.SendTempInviteResponse(false, request);
            };
        }

        /// <summary>
        /// 接受组队更新响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnTempInfo(object sender, TempInfoResponse message)
        {
            Debug.Log("[Client]：OnTempInfo");
            TempManager.Instance.UpdateTempInfo(message.Team);
        }

        /// <summary>
        /// 发送离开队伍请求
        /// </summary>
        /// <param name="tempId"></param>
        /// <param name="characterId"></param>
        public void SendTempLeaveRequest(int tempId)
        {
            Debug.Log("[Client]：SendTempLeaveRequest");
            NetClient.Instance.Request.tempInviteReq = new TempInviteRequest();
            NetClient.Instance.Request.TempLeave = new TempLeaveRequest();
            NetClient.Instance.Request.TempLeave.TeamId = tempId;
            NetClient.Instance.Request.TempLeave.Characterid = User.Instance.CurrentCharacter.Id;
            NetClient.Instance.SendMessage(NetClient.Instance.request);
        }

        /// <summary>
        /// 接受离开队伍响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnTempLeave(object sender, TempLeaveResponse message)
        {
            Debug.Log("[Client]：OnTempLeave");
            if (message.Result == Result.Success)
            {
                TempManager.Instance.UpdateTempInfo(null);
                MessageBox.Show("退出成功", "退出队伍");
            }
            else
                MessageBox.Show("退出失败", "退出队伍");
        }
    }
}
