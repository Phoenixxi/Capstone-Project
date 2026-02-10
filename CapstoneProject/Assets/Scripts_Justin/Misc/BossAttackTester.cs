using UnityEngine;

public class BossAttackTester : MonoBehaviour
{
    public GameObject attack;
    public Vector3 spawnPosition;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Instantiate(attack, spawnPosition, Quaternion.identity);
        }
    }
}
