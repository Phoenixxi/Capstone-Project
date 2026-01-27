using UnityEngine;

public class VFXDestroy : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            Destroy(gameObject);
        
    }
}
