using UnityEngine;

public class BossAttackTester : MonoBehaviour
{
    public GameObject attack;
    public Vector3 spawnPosition;
    public bool isRepeating = false;
    protected GameObject spawnedAttack;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        if(!isRepeating || spawnedAttack == null)
        {
            spawnedAttack = Instantiate(attack, spawnPosition, Quaternion.identity);
        } else
        {
            spawnedAttack.GetComponent<BossAttack>().Attack();
        }
    }
}
