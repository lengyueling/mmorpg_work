using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using Network;
using UnityEngine;
using UnityEngine.Events;

using Entities;
using SkillBridge.Message;

namespace Managers
{
    class CharacterManager : Singleton<CharacterManager>, IDisposable
    {
        /// <summary>
        /// 储存角色id和实体的字典
        /// </summary>
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();


        public UnityAction<Character> OnCharacterEnter;
        public UnityAction<Character> OnCharacterLeave;

        public CharacterManager()
        {

        }

        public void Dispose()
        {
        }

        public void Init()
        {

        }
        /// <summary>
        /// 自己离开游戏
        /// 移除屏幕上的所有人
        /// </summary>
        public void Clear()
        {
            int[] keys = this.Characters.Keys.ToArray();
            foreach (var key in keys)
            {
                this.RemoveCharacter(key);
            }
            this.Characters.Clear();
        }

        /// <summary>
        /// 一个角色进入了地图
        /// </summary>
        /// <param name="cha"></param>
        public void AddCharacter(SkillBridge.Message.NCharacterInfo cha)
        {
            Debug.LogFormat("AddCharacter:{0}:{1} Map:{2} Entity:{3}", cha.Id, cha.Name, cha.mapId, cha.Entity.String());
            Character character = new Character(cha);
            this.Characters[cha.EntityId] = character;
            EntityManager.Instance.AddEntity(character);
            if(OnCharacterEnter!=null)
            {
                OnCharacterEnter(character);
            }
        }

        /// <summary>
        /// 一个角色离开了地图
        /// </summary>
        /// <param name="entityId"></param>
        public void RemoveCharacter(int entityId)
        {
            Debug.LogFormat("RemoveCharacter:{0}", entityId);
            EntityManager.Instance.RemoveEntity(this.Characters[entityId].Info.Entity);
            if (this.Characters.ContainsKey(entityId))
            {
                OnCharacterLeave(this.Characters[entityId]);

            }
            this.Characters.Remove(entityId);

        }
    }
}
