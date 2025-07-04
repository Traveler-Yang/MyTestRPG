using Common;
using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class UserService : Singleton<UserService>, IDisposable
    {
        public UnityEngine.Events.UnityAction<Result, string> OnLogin;
        public UnityEngine.Events.UnityAction<Result, string> OnRegister;
        public UnityEngine.Events.UnityAction<Result, string> OnCharacterCreate;

        NetMessage pendingMessage = null;
        bool connected = false;

        public UserService()
        {
            NetClient.Instance.OnConnect += OnGameServerConnect;
            NetClient.Instance.OnDisconnect += OnGameServerDisconnect;
            MessageDistributer.Instance.Subscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Subscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Subscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Subscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Subscribe<UserGameLeaveResponse>(this.OnGameLeave);
            //MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnCharacterEnter);

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<UserLoginResponse>(this.OnUserLogin);
            MessageDistributer.Instance.Unsubscribe<UserRegisterResponse>(this.OnUserRegister);
            MessageDistributer.Instance.Unsubscribe<UserCreateCharacterResponse>(this.OnUserCreateCharacter);
            MessageDistributer.Instance.Unsubscribe<UserGameEnterResponse>(this.OnGameEnter);
            MessageDistributer.Instance.Unsubscribe<UserGameLeaveResponse>(this.OnGameLeave);
            NetClient.Instance.OnConnect -= OnGameServerConnect;
            NetClient.Instance.OnDisconnect -= OnGameServerDisconnect;
        }

        public void Init()
        {

        }

        /// <summary>
        /// SendMessage方法
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="buildRequest">Request消息</param>
        private void Send<T>(Action<NetMessageRequest> buildRequest) where T : class
        {
            //构建消息
            var message = new NetMessage();
            message.Request = new NetMessageRequest();
            buildRequest?.Invoke(message.Request);//如果传进来的消息信息不为空，则调用
            //是否已连接，如果链接，则直接发送
            if (this.connected && NetClient.Instance.Connected)
            {
                this.pendingMessage = null;
                NetClient.Instance.SendMessage(message);
            }
            else//未连接，则缓存下来
            {
                this.pendingMessage = message;
                this.ConnectToServer();
            }
        }

        public void ConnectToServer()
        {
            Debug.Log("ConnectToServer() Start ");
            //NetClient.Instance.CryptKey = this.SessionId;
            NetClient.Instance.Init("127.0.0.1", 8000);
            NetClient.Instance.Connect();
        }


        void OnGameServerConnect(int result, string reason)
        {
            //Log.InfoFormat("LoadingMesager::OnGameServerConnect :{0} reason:{1}", result, reason);
            if (NetClient.Instance.Connected)
            {
                this.connected = true;
                if(this.pendingMessage!=null)
                {
                    NetClient.Instance.SendMessage(this.pendingMessage);
                    this.pendingMessage = null;
                }
            }
            else
            {
                if (!this.DisconnectNotify(result, reason))
                {
                    MessageBox.Show(string.Format("网络错误，无法连接到服务器！\n RESULT:{0} ERROR:{1}", result, reason), "错误", MessageBoxType.Error);
                }
            }
        }

        public void OnGameServerDisconnect(int result, string reason)
        {
            this.DisconnectNotify(result, reason);
            return;
        }

        bool DisconnectNotify(int result,string reason)
        {
            if (this.pendingMessage != null)
            {
                if (this.pendingMessage.Request.userLogin!=null)
                {
                    if (this.OnLogin != null)
                    {
                        this.OnLogin(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                else if(this.pendingMessage.Request.userRegister!=null)
                {
                    if (this.OnRegister != null)
                    {
                        this.OnRegister(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                else
                {
                    if (this.OnCharacterCreate != null)
                    {
                        this.OnCharacterCreate(Result.Failed, string.Format("服务器断开！\n RESULT:{0} ERROR:{1}", result, reason));
                    }
                }
                return true;
            }
            return false;
        }

        public void SendLogin(string user, string psw)
        {
            Debug.LogFormat("UserLoginRequest::user :{0} psw:{1}", user, psw);
            this.Send<UserLoginRequest>(req =>
            {
                req.userLogin = new UserLoginRequest
                {
                    User = user,
                    Passward = psw
                };
            });
        }

        void OnUserLogin(object sender, UserLoginResponse response)
        {
            Debug.LogFormat("OnLogin:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                Models.User.Instance.SetupUserInfo(response.Userinfo);
            }
            if (this.OnLogin != null)
            {
                this.OnLogin(response.Result, response.Errormsg);
            }
        }


        public void SendRegister(string user, string psw)
        {
            Debug.LogFormat("UserRegisterRequest::user :{0} psw:{1}", user, psw);
            this.Send<UserRegisterRequest>(req =>
            {
                req.userRegister = new UserRegisterRequest
                {
                    User = user,
                    Passward = psw
                };
            });
        }

        void OnUserRegister(object sender, UserRegisterResponse response)
        {
            Debug.LogFormat("OnUserRegister:{0} [{1}]", response.Result, response.Errormsg);

            if (this.OnRegister != null)
            {
                this.OnRegister(response.Result, response.Errormsg);
            }
        }
        public void SendCharacterCreate(string charatcterName, CharacterClass chaClass)
        {
            Debug.LogFormat("UserCreateCharacterRequest::charatcterName :{0} chaClass:{1}", charatcterName, chaClass);
            //NetMessage message = new NetMessage();
            //message.Request = new NetMessageRequest();
            //message.Request.createChar = new UserCreateCharacterRequest();
            //message.Request.createChar.Name = charatcterName;
            //message.Request.createChar.Class = chaClass;

            //if (this.connected && NetClient.Instance.Connected)
            //{
            //    this.pendingMessage = null;
            //    NetClient.Instance.SendMessage(message);
            //}
            //else
            //{
            //    this.pendingMessage = message;
            //    this.ConnectToServer();
            //}

            this.Send<UserCreateCharacterRequest>(req =>
            {
                req.createChar = new UserCreateCharacterRequest
                {
                    Name = charatcterName,
                    Class = chaClass
                };
            });
        }

        void OnUserCreateCharacter(object sender, UserCreateCharacterResponse response)
        {
            Debug.LogFormat("OnUserCreateCharacter:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                Models.User.Instance.Info.Player.Characters.Clear();
                Models.User.Instance.Info.Player.Characters.AddRange(response.Characters);
            }

            if (this.OnCharacterCreate != null)
            {
                this.OnCharacterCreate(response.Result, response.Errormsg);
            }
        }

        public void SendGameEnter(int characterIdx)
        {
            Debug.LogFormat("UserGameEnterRequest::charatcterId :{0}", characterIdx);
            //NetMessage message = new NetMessage();
            //message.Request = new NetMessageRequest();
            //message.Request.gameEnter = new UserGameEnterRequest();
            //message.Request.gameEnter.characterIdx = characterIdx;//将角色的类型赋值给协议
            //NetClient.Instance.SendMessage(message);//发送到服务端
            this.Send<UserGameEnterRequest>(req =>
            {
                req.gameEnter = new UserGameEnterRequest
                {
                    characterIdx = characterIdx
                };
            });
        }

        private void OnGameEnter(object sender, UserGameEnterResponse response)
        {
            Debug.LogFormat("OnGameEnter:{0} [{1}]", response.Result, response.Errormsg);

            if (response.Result == Result.Success)
            {
                if (response.Character != null)
                {
                    User.Instance.CurrentCharacter = response.Character;//将当前角色赋值给User
                    ItemManager.Instance.Init(response.Character.Items);//初始化物品
                    BagManager.Instance.Init(response.Character.Bag);//初始化背包
                    EquipManager.Instance.Init(response.Character.Equips);//初始化装备
                    QuestManager.Instance.Init(response.Character.Quests);//初始化任务
                    FriendManager.Instance.Init(response.Character.Friends);//初始化好友
                    TempManager.Instance.Init();//初始化组队
                }
            }
        }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnCharacterEnter(object sender, MapCharacterEnterResponse message)
        {
            //Debug.LogFormat("OnMapCharacterEnter:{0}", message.mapId);

            //NCharacterInfo info = message.Characters[0];
            //User.Instance.CurrentCharacter = info;
            //SceneManager.Instance.LoadScene(DataManager.Instance.Maps[message.mapId].Resource);//加载从服务端接收的地图ID并加载场景
           
        }
        /// <summary>
        /// 角色离开
        /// </summary>
        public void SendGameLeave()
        {
            Debug.Log("UserGameLeaveRequest");
            //NetMessage message = new NetMessage();
            //message.Request = new NetMessageRequest();
            //message.Request.gameLeave = new UserGameLeaveRequest();
            //NetClient.Instance.SendMessage(message);
            this.Send<UserGameLeaveRequest>(req =>
            {
                req.gameLeave = new UserGameLeaveRequest();
            });
        }

        /// <summary>
        /// 离开主城
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        void OnGameLeave(object sender, UserGameLeaveResponse response)
        {
            MapService.Instance.CurrentMapId = 0;
            User.Instance.CurrentCharacter = null;
            Debug.LogFormat("OnGameLeave:{0} [{1}]", response.Result, response.Errormsg);
        }

    }
}
