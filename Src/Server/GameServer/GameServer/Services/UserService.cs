using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    internal class UserService : Singleton<UserService>
    {
        public void Init()
        {

        }

        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
        }
        /// <summary>
        /// 接收注册账号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0} Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "None";
            }

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        /// <summary>
        /// 接收登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0} Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "用户不存在";
            }
            else if (user.Password != request.Passward)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user;

                message.Response.userLogin.Result = Result.Success;
                message.Response.userLogin.Errormsg = "登录成功！";
                message.Response.userLogin.Userinfo = new NUserInfo();
                message.Response.userLogin.Userinfo.Id = 1;
                message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Class = (CharacterClass)c.Class;
                    message.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        /// <summary>
        /// 接收角色创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0} Class:{1}", request.Name, request.Class);//打印日志

            TCharacter character = new TCharacter()//new一个新的角色表
            {
                //将客户端传过来的数据赋值
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                //进入游戏时要在什么地方
                MapID = 1,//初始地图
                //初始位置坐标
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
            };
            DBService.Instance.Entities.Characters.Add(character);//将创建的角色表Add到Entities
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();//更新Entities

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.createChar = new UserCreateCharacterResponse();

            message.Response.createChar.Result = Result.Success;//结果值
            message.Response.createChar.Errormsg = "None";

            byte[] data = PackageHandler.PackMessage(message);//将创建成功的消息打包成字节流，发送给客户端
            sender.SendData(data, 0, data.Length);
        }

        /// <summary>
        /// 接收游戏进入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: CharacterID:{0}:{1} Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);//打印日志
            Character character = CharacterManager.Instance.AddCharacter(dbchar);//1.添加一个角色到角色管理器，并得到一个实体的Character

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.gameEnter = new UserGameEnterResponse();
            message.Response.gameEnter.Result = Result.Success;//结果值
            message.Response.gameEnter.Errormsg = "None";//错误信息

            byte[] data = PackageHandler.PackMessage(message);//将创建成功的消息打包成字节流，发送给客户端
            sender.SendData(data, 0, data.Length);
            sender.Session.Character = character;//一旦进入游戏，就会将选择的指定角色 赋值给 会话对象
            MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);//2.让角色进入地图
        }
    }
}
