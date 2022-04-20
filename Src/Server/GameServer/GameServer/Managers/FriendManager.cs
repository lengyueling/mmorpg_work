using Common;
using Common.Data;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class FriendManager
    {
        private Character Owner;

        List<NFriendInfo> friends = new List<NFriendInfo>();

        /// <summary>
        /// 标记好友状态改变
        /// </summary>
        bool friendChanged = false;

        public FriendManager(Character owner)
        {
            this.Owner = owner;
            this.InitFriends();
        }

        public void GetFriendInfos(List<NFriendInfo> list)
        {
            foreach (var f in friends)
            {
                list.Add(f);
            }
        }

        /// <summary>
        /// 初始化好友列表
        /// </summary>
        private void InitFriends()
        {
            this.friends.Clear();
            foreach (var friend in Owner.Data.Friends)
            {
                this.friends.Add(GetFriendInfo(friend));
            }
        }

        public void AddFriend(Character friend)
        {
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendID = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level,
            };
            this.Owner.Data.Friends.Add(tf);
            friendChanged = true;
        }

        /// <summary>
        /// 按照FriendId移除好友
        /// </summary>
        /// <param name="friendid"></param>
        /// <returns></returns>
        public bool RemoveFriendByFriendId(int friendid)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.FriendID == friendid);
            if (removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        /// <summary>
        /// 按照Id移除好友
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool RemoveFriendByID(int id)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.Id == id);
            if (removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        /// <summary>
        /// 获取好友信息
        /// </summary>
        /// <param name="friend"></param>
        /// <returns></returns>
        public NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo friendInfo = new NFriendInfo();
            var character = CharacterManager.Instance.GetCharacter(friend.FriendID);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;
            //如果角色离线了
            if (character == null)
            {
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;
            }
            else
            {
                friendInfo.friendInfo = GetBasicInfo(character.Info);
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;
                character.FriendManager.UpdateFriendInfo(this.Owner.Info, 1);
                friendInfo.Status = 1;
            }
            return friendInfo;
        }

        NCharacterInfo GetBasicInfo(NCharacterInfo info)
        {
            return new NCharacterInfo()
            {
                Id = info.Id,
                Name = info.Name,
                Class = info.Class,
                Level = info.Level,
            };
        }

        public NFriendInfo GetFriendInfo(int FriendID)
        {
            foreach (var f in this.friends)
            {
                if (f.friendInfo.Id == FriendID)
                {
                    return f;
                }
            }
            return null;
        }

        /// <summary>
        /// 更新好友状态
        /// </summary>
        /// <param name="friendInfo"></param>
        /// <param name="state"></param>
        public void UpdateFriendInfo(NCharacterInfo friendInfo, int status)
        {
            foreach (var f in friends)
            {
                if (f.friendInfo.Id == friendInfo.Id)
                {
                    f.Status = status;
                    break;
                }
            }
            this.friendChanged = true;
        }

        /// <summary>
        /// 好友后处理
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            if (friendChanged)
            {
                this.InitFriends();
                if (message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.friends);
                }
                friendChanged = false;
            }
        }
    }
}
