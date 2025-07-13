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
            Log.InfoFormat("UserService OnRegister: User:{0} Pass:{1}", request.User, request.Passward);

            sender.Session.Response.userRegister = new UserRegisterResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errormsg = "None";
            }
            sender.SendResPonse();
        }

        /// <summary>
        /// 接收登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserService OnLogin: User:{0} Pass:{1}", request.User, request.Passward);

            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user == null)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户不存在";
            }
            else if (user.Password != request.Passward)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user;

                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "登录成功！";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Type = CharacterType.Player;
                    info.Class = (CharacterClass)c.Class;
                    info.ConfigId = c.ID;
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }
            sender.SendResPonse();
        }

        /// <summary>
        /// 接收角色创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserService OnCreateCharacter: Name:{0} Class:{1}", request.Name, request.Class);//打印日志

            TCharacter character = new TCharacter()//new一个新的角色表
            {
                //将客户端传过来的数据赋值
                Name = request.Name,
                Class = (int)request.Class,
                TID = (int)request.Class,
                Level = 10,
                //进入游戏时要在什么地方
                MapID = 1,//初始地图
                //初始位置坐标
                MapPosX = 14250,
                MapPosY = 9500,
                MapPosZ = 1100,
                Gold = 100000,
                Equips = new byte[24],
            };
            //根据配置表实例化一个背包
            var bag = new TCharacterBag();
            bag.Owner = character;
            bag.Items = new byte[0];
            bag.Unlocked = 20;
            //将新建的背包数据添加到服务器，再赋值给角色
            character.Bag = DBService.Instance.Entities.CharacterBags.Add(bag);
            character = DBService.Instance.Entities.Characters.Add(character);//将创建的角色表Add到Entities

            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 1,
                ItemCount = 20,
            });
            character.Items.Add(new TCharacterItem()
            {
                Owner = character,
                ItemID = 2,
                ItemCount = 20,
            });

            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();//更新Entities

            sender.Session.Response.createChar = new UserCreateCharacterResponse();
            sender.Session.Response.createChar.Result = Result.Success;//结果值
            sender.Session.Response.createChar.Errormsg = "None";

            //把当前已经有的角色添加到列表中
            foreach (var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo info = new NCharacterInfo();
                info.Id = c.ID;
                info.Name = c.Name;
                info.Type = CharacterType.Player;
                info.Class = (CharacterClass)c.Class;
                info.ConfigId = c.ID;
                sender.Session.Response.createChar.Characters.Add(info);
            }

            sender.SendResPonse();
        }

        /// <summary>
        /// 接收游戏进入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserService OnGameEnter: CharacterID:{0}:{1} Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);//打印日志
            Character character = CharacterManager.Instance.AddCharacter(dbchar);//1.添加一个角色到角色管理器，并得到一个实体的Character
            SessionManager.Instance.AddSession(character.Id,sender);//每进入游戏，就将当前角色的会话对象添加到管理器中
            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;//结果值
            sender.Session.Response.gameEnter.Errormsg = "None";//错误信息

            sender.Session.Character = character;
            sender.Session.PostResponser = character;//进入游戏给后处理器赋值（里氏替换）

            sender.Session.Response.gameEnter.Character = character.Info;//进入成功 发送初始角色信息给客户端
            sender.SendResPonse();
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
            Log.InfoFormat("UserService OnGameLeave: CharacterID:{0}:{1} Map:{2}", character.Id, character.Info.Name, character.Info.mapId);//打印日志
            CharacterLeave(character);
            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;//得到结果
            sender.Session.Response.gameLeave.Errormsg = "None";//错误为空
            sender.SendResPonse();
        }

        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("UserService > CharacterLeave：CharacterID:{0}:{1}", character.Id, character.Info.Name);
            SessionManager.Instance.RemoveSession(character.Id);//角色离开删除会话对象
            CharacterManager.Instance.RemoveCharacter(character.entityId);//移除从客户端传送过来的角色
            character.Clear();//要先更改状态，再离开，并发送消息
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
