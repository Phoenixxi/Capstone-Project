using UnityEngine;

public class VFXExitDestroy : MonoBehaviour
{
    public void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            Destroy(gameObject);
        
    }
}
