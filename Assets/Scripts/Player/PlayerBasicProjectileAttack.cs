using UnityEngine;

/** \deprecated
This was the original script that handled the player's basic projectile attack. Now, this is handled by PlayerAttackManager.
Please use PlayerAttackManager instead of this script. It has been disconnected from the player object and will not work.

Documentation updated 8/30/2024
\author Stephen Nuttall
*/
public class PlayerBasicProjectileAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform attackPoint;
    public float attackCooldown = 1f;
    float cooldownTimer = 0f;

    public PlayerAttackManager playerAttackManager;

    void Update()
    {
        cooldownTimer += Time.deltaTime; // update attack cooldown timer

        // if right click is pressed and the cooldown timer has expired...
        if (Input.GetKeyDown(KeyCode.Mouse1) && cooldownTimer >= attackCooldown)
        {
            playerAttackManager.ShootProjectile(projectilePrefab);

            // reset cooldown timer
            cooldownTimer = 0;
        }
    }
}
