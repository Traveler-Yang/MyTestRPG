using Managers;
using UnityEngine;

public class MapContorller : MonoBehaviour
{
    public Collider miniMapBoundingBox;

    void Start()
    {
        MiniMapManager.Instance.UpdateMiniMap(miniMapBoundingBox);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
