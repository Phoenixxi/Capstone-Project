using UnityEngine;
 
public class BlobShadowManager : MonoBehaviour
{
    public GameObject player;
    public GameObject blobShadow;
 
    void LateUpdate()
    {
        if (player == null || blobShadow == null) return;
 
        blobShadow.SetActive(player.activeInHierarchy);
    }
}