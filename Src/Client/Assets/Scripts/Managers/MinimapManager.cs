using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MinimapManager : Singleton<MinimapManager>
    {
        public UIMinimap minimap;
        private Collider minimapBoundingBox;
        public Collider MinimapBoundingBox
        {
            get
            {
                return minimapBoundingBox;
            }
        }

        /// <summary>
        /// 设置当前角色坐标
        /// </summary>
        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharacterObject == null)
                {
                    return null;
                }
                return User.Instance.CurrentCharacterObject.transform;
            } 
        }
        /// <summary>
        /// 加载当前小地图的资源图片
        /// </summary>
        /// <returns></returns>
        public Sprite LoadCurrentMinimap()
        {
            return Resloader.Load<Sprite>("UI/Minimap/" + User.Instance.CurrentMapData.Minimap);
        }

        /// <summary>
        /// 更新包围盒,并更新地图
        /// </summary>
        /// <param name="minimapBoudingBox"></param>
        public void UpdateMinimap(Collider minimapBoundingBox)
        {
            this.minimapBoundingBox = minimapBoundingBox;
            if (this.minimap != null)
            {
                this.minimap.UpdateMap();
            }
        }
    }
}

