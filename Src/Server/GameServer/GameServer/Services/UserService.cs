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
            //注册消息
            //最后括号中填写的方法是通过客户端那里直接进行SendMessage后，直接可以执行到这里的方法(函数)
            //所有类似这样的写法，都是通过协议来与客户端和服务端进行通信的方法
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
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
                message.Response.userLogin.Userinfo.Id = (int)user.ID;
                message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Type = CharacterType.Player;
                    info.Class = (CharacterClass)c.Class;
                    info.Tid = c.ID;
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
                MapPosX = 5600,
                MapPosY = 4400,
                MapPosZ = 900,
            };
            character = DBService.Instance.Entities.Characters.Add(character);//将创建的角色表Add到Entities
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();//更新Entities

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.createChar = new UserCreateCharacterResponse();

            message.Response.createChar.Result = Result.Success;//结果值
            message.Response.createChar.Errormsg = "None";

            //把当前已经有的角色添加到列表中
            foreach (var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = 0;
                info.Name = c.Name;
                info.Type = CharacterType.Player;
                info.Class = (CharacterClass)c.Class;
                info.Tid = c.ID;
                message.Response.createChar.Characters.Add(info);
            }

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

            message.Response.gameEnter.Character = character.Info;//进入成功 发送初始角色信息给客户端

            //道具系统测试
            int itemId = 1;//假设我们要添加一个物品ID为1的道具
            bool hasItem = character.ItemManager.HasItem(itemId);//检查角色是否拥有这个物品
            Log.InfoFormat("HasItem[{0}]{1}", itemId, hasItem);
            if (hasItem)//如果角色拥有这个物品，则移除1个物品
            {
                character.ItemManager.RemoveItem(itemId, 1);
            }
            else//如果角色没有这个物品，则添加2个物品
            {
                character.ItemManager.AddItem(itemId, 2);
            }
            //取出一个物品出来查看一下
            Models.Item item = character.ItemManager.GetItem(itemId);

            Log.InfoFormat("Item[{0}][{1}]", itemId, item);

            byte[] data = PackageHandler.PackMessage(message);//将创建成功的消息打包成字节流，发送给客户端
            sender.SendData(data, 0, data.Length);
            sender.Session.Character = character;//一旦进入游戏，就会将选择的指定角色 赋值给 会话对象
            MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);//2.让角色进入地图
        }
        /// <summary>
        /// 接收游戏退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;//从客户端传过来的角色信息
            Log.InfoFormat("UserGameEnterRequest: CharacterID:{0}:{1} Map:{2}", character.Id, character.Info.Name, character.Info.mapId);//打印日志

            CharacterLeave(character);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.gameLeave = new UserGameLeaveResponse();
            message.Response.gameLeave.Result = Result.Success;//得到结果
            message.Response.gameLeave.Errormsg = "None";//错误为空

            byte[] data = PackageHandler.PackMessage(message);//将创建成功的消息打包成字节流，发送给客户端
            sender.SendData(data, 0, data.Length);
        }

        public void CharacterLeave(Character character)
        {
            CharacterManager.Instance.RemoveCharacter(character.Id);//移除从客户端传送过来的角色
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
