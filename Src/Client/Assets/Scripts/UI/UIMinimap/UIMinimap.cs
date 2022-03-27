using Models;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour {

    public Image minimap;
    public Image arrow;
    public Text mapName;
    public Collider minimapBoundingBox;
    private Transform playerTransform;

	void Start () {
        //不写这个会在单例中被destory
        if (MinimapManager.Instance.minimap == null)
        {
            MinimapManager.Instance.minimap = this;

        }
        this.UpdateMap();
	}
	
    /// <summary>
    /// 更新地图
    /// </summary>
    public void UpdateMap()
    {
        //设置小地图名字
        this.mapName.text = User.Instance.CurrentMapData.Name;
        //设置小地图资源
        this.minimap.overrideSprite = MinimapManager.Instance.LoadCurrentMinimap();
        this.minimap.SetNativeSize();
        this.minimap.transform.localPosition = Vector3.zero;
        //设置小地图的包围盒
        this.minimapBoundingBox = MinimapManager.Instance.MinimapBoundingBox;
        //重置小地图的玩家位置
        this.playerTransform = null;
    }

	void Update () {
        if (playerTransform == null)
        {
            playerTransform = MinimapManager.Instance.PlayerTransform;
        }
        if (minimapBoundingBox == null || playerTransform == null)
        {
            return;
        }
        //角色在地图的绝对位置
        float realWidth = minimapBoundingBox.bounds.size.x;
        float realHeight = minimapBoundingBox.bounds.size.z;
        //角色在小地图的相对位置
        float relaX = playerTransform.position.x - minimapBoundingBox.bounds.min.x;
        float relaY = playerTransform.position.z - minimapBoundingBox.bounds.min.z;
        //实时更新小地图的中心点
        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;
        this.minimap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.minimap.rectTransform.localPosition = Vector2.zero;
        //让小地图中心的箭头实时跟着角色旋转
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
    }
}
