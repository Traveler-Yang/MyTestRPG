using Models;
using UnityEngine;
using Cinemachine;

public class MainPlayerCamera : MonoSingleton<MainPlayerCamera>
{
    [Header("主相机")]
    public Camera camera;

    [Header("Cinemachine FreeLook 相机")]
    public CinemachineFreeLook vcam; // 在 Inspector 拖进去

    public GameObject player;

    /// <summary>
    /// 设置相机跟随目标（只跟随本地玩家）
    /// </summary>
    public void SetTarget(Transform target)
    {
        if (vcam != null && target != null)
        {
            // 这里假设你的角色 prefab 下有 CameraRoot 和 CameraTarget 两个节点
            Transform follow = target.Find("CameraRoot");
            Transform lookAt = target.Find("CameraTarget");

            if (follow != null) vcam.Follow = follow;
            if (lookAt != null) vcam.LookAt = lookAt;
        }
    }

    private void LateUpdate()
    {
        // 只在本地玩家创建好后绑定一次相机
        if (player == null && User.Instance.CurrentCharacterObject != null)
        {
            player = User.Instance.CurrentCharacterObject.gameObject;

            // 设置相机跟随本地玩家
            SetTarget(player.transform);
        }
    }
}
