using Models;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniMap : MonoBehaviour {

	public Collider miniMapBoundingBox;//地图包围盒（主城）
	public Image miniMap;//地图图片
	public Image arrow;//角色箭头
	public Text mapName;//地图名字
	public Camera minimapCamera;//小地图的相机

	private Transform playerTransform;//当前角色的位置

	void Start () 
	{
        MiniMapManager.Instance.miniMap = this;//将当前的迷你地图UI赋值给MiniMapManager，告诉管理器当前的迷你地图UI是哪个
        UpdateMap();
    }
	
	public void UpdateMap()
	{
        this.mapName.text = User.Instance.CurrentMapData.Name;//将从配置表中读取的数值赋值给当前地图
		//this.miniMap.overrideSprite = MiniMapManager.Instance.LoadCurrentMiniMap();

		//this.miniMap.SetNativeSize();
		//this.miniMap.transform.localPosition = Vector3.zero;
		//this.miniMapBoundingBox = MiniMapManager.Instance.MiniMapBoundingBox;//获取当前地图的包围盒
		this.playerTransform = null;//清空当前角色的位置
    }
	
	void Update () {

		if (playerTransform == null)
			playerTransform = MiniMapManager.Instance.PlayerTransform;

        if (playerTransform == null) return;

		//float realWidth = miniMapBoundingBox.bounds.size.x;//地图包围盒的宽度
		//float realHeight = miniMapBoundingBox.bounds.size.y;//地图包围盒的高度

		//float relaX = playerTransform.position.x - miniMapBoundingBox.bounds.min.x;//包围盒的左下角坐标
		//float relaY = playerTransform.position.z - miniMapBoundingBox.bounds.min.z;

		//float pivotX = relaX / realWidth;
		//float pivotY = relaY / realHeight;

		//this.miniMap.rectTransform.pivot = new Vector2(pivotX, pivotY);
		//this.miniMap.rectTransform.localPosition = Vector3.zero;
		this.minimapCamera.transform.position = new Vector3(playerTransform.position.x, minimapCamera.transform.position.y, playerTransform.position.z);
        this.arrow.transform.eulerAngles = new Vector3(0f, 0f, -playerTransform.eulerAngles.y);
	}
}
