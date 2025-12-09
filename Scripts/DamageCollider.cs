using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Is this the players own damage VFX")]
    public bool isPlayers;

    [Header("List of who got hit already")]
    public List<GameObject> whoWasHit;

    [Header("Player Damage")]
    public float playerDamage = 0; // when spawning in vfx we get this script and set dmg 

    [Header("Enemy Damage")]
    public float enemyDamage = 0; // when spawning in vfx we get this script and set dmg 

    public void OnTriggerEnter(Collider other)
    {
        if (isPlayers)
        {
            // DONT DAMAGE PLAYER ONLY ENEMIES
            if (other.tag == "Enemy")
            {
                if (!whoWasHit.Contains(other.gameObject))
                {
                    // the collider hit an enemey deal damage
                    other.GetComponent<EnemyManager>().TakeDamage(playerDamage);
                }
            }
        }
        else
        {
            // NOT THE PLAYERS SO DAMAGE ONLY PLAYERS
            if (other.tag == "Player")
            {
                if (!whoWasHit.Contains(other.gameObject))
                {
                    // the collider hit the player deal damage
                    other.GetComponent<PlayerManager>().TakeDamage(enemyDamage);
                }
               
            }
        }
    }
}
