/**************************************************
Basic functionality for any entities that are hostile towards other entities.
This script is abstract so it must be inherited by another script to be used.
Every script inheriting from this must override ActivateAttack().

Patrol State (default): The entity will patrol an area (PatrolZone.cs) until an enemy (something on the enemyLayers) is within range.
Chase State: The entity will chase the enemy it detected until it's close enough to attack.
CloseAttack State: The entity will attack the enemy until it's no longer in range (either because it left or because it died)
Dead State: This script won't do anything if the entity dies.

Documentation updated 1/29/2024
**************************************************/
using UnityEngine;

public abstract class BaseEntityController : MonoBehaviour
{
    protected ObjectHealth objectHealth;

    public Transform attackPoint;
    public int attackDamage = 30;
    public float attackCooldown = 0.8f;
    protected float cooldownEndTime = 0f;

    public LayerMask enemyLayers;  // This LayerMask includes the Player's layer so the enemy is alerted
    public PatrolZone patrolZone;
    public EntityState EState { get; set; } = EntityState.Patrol;
    public float horizontalDirection = 0f;
    public float detectionRange = 6f;
    public float activateAttackRange = 3f;  // range which entity will activate attack
    protected bool hostileDetected = false;
    protected bool hostileInCloseRange = false;

    protected Rigidbody2D rb;
    public float moveVelocity = 6.0f;
    public float linearDrag = 1.0f;
    public float groundedRaycastLength = 1.8f;

    protected Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb.drag = linearDrag;
    }

    // Update is called once per frame
    void Update()
    {
        switch (EState)
        {
            case EntityState.Patrol:
                PatrolState();
                break;
            case EntityState.Chase:
                ChaseState();
                break;
            case EntityState.CloseAttack:
                CloseAttackState();
                break;
            case EntityState.Dead:
                break;
            default:
                PatrolState();
                break;
        }
    }

    // Triggered by an event in the attack animation
    // (or you can override TriggerAttack() if there is not attack animation)
    protected abstract void ActivateAttack();

    protected virtual void TriggerAttack()
    {
        animator.SetBool("IsAttacking", true);
    }

    protected virtual void PatrolState()
    {
        if (horizontalDirection == 0f)
            horizontalDirection = 1f;

        // If to the left of the left point, go right
        // If to the right of the right point, go left
        if (transform.position.x <= patrolZone.LeftPoint().x)
        {
            horizontalDirection = 1f;
        }
        else if (transform.position.x >= patrolZone.RightPoint().x)
        {
            horizontalDirection = -1f;
        }

        // Flip the direction of the enemy depending on direction
        // Facing right = -2 scale for x
        // Facing left = 2 scale for x
        if (horizontalDirection != 0f)
            transform.localScale = new Vector3(-2 * horizontalDirection,
                    transform.localScale.y,
                    transform.localScale.z);

        animator.SetBool("IsMoving", horizontalDirection != 0f);

        rb.velocity = new Vector2(moveVelocity * horizontalDirection, rb.velocity.y);

        // The offset (0, 1.3, 0) moves the circle up to the center of the sprite
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            detectionRange,
            enemyLayers);

        hostileDetected = hitObject != null;

        // Prioritize death
        if (objectHealth.IsDead)
        {
            EState = EntityState.Dead;
            horizontalDirection = 0f;
        }
        else if (hostileDetected)
            EState = EntityState.Chase;
    }

    protected virtual void ChaseState()
    {
        // The offset (0, 1.3, 0) moves the circle up to the center of the sprite
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            detectionRange,
            enemyLayers);

        hostileDetected = hitObject != null;
        hostileInCloseRange = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            activateAttackRange,
            enemyLayers) != null;

        // If detected enemy horizontal position is within the minimum amount (0.1f), don't switch directions
        // This prevents the enemy from rapidly switching directions
        // If detected enemy is to the left, go left
        // If detected enemy is to the right, go right
        if (hostileDetected && Mathf.Abs(hitObject.transform.position.x - transform.position.x) < 0.1f)
        {
            horizontalDirection = 0f;
        }
        else if (hostileDetected && hitObject.transform.position.x < transform.position.x)
        {
            horizontalDirection = -1f;
        }
        else if (hostileDetected && hitObject.transform.position.x > transform.position.x)
        {
            horizontalDirection = 1f;
        }
        else
        {
            horizontalDirection = 0f;
        }

        // Flip the direction of the enemy depending on direction
        // Facing right = -2 scale for x
        // Facing left = 2 scale for x
        if (horizontalDirection != 0f)
            transform.localScale = new Vector3(-2 * horizontalDirection,
                    transform.localScale.y,
                    transform.localScale.z);

        animator.SetBool("IsMoving", horizontalDirection != 0f);

        rb.velocity = new Vector2(moveVelocity * horizontalDirection, rb.velocity.y);

        // Prioritize death
        if (objectHealth.IsDead)
        {
            EState = EntityState.Dead;
            horizontalDirection = 0f;
        }
        else if (hostileInCloseRange)
        {
            EState = EntityState.CloseAttack;
        }
        else if (!hostileDetected)
            EState = EntityState.Patrol;
    }

    protected virtual void CloseAttackState()
    {
        Collider2D hitObject = Physics2D.OverlapCircle(transform.position + new Vector3(0f, 1.3f, 0f),
            activateAttackRange,
            enemyLayers);

        hostileInCloseRange = hitObject != null;

        // I use an offset of 1.4 to make it closer to upper body height
        if (hostileInCloseRange)
        {
            Debug.DrawRay(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.right * activateAttackRange,
                Color.cyan);

            Debug.DrawRay(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.left * activateAttackRange,
                Color.cyan);

            RaycastHit2D enemyFrontRay, enemyBackRay;

            enemyFrontRay = Physics2D.Raycast(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.right,
                activateAttackRange,
                enemyLayers);

            enemyBackRay = Physics2D.Raycast(transform.position + new Vector3(0f, 1.4f, 0f),
                Vector2.left,
                activateAttackRange,
                enemyLayers);

            if (enemyFrontRay.collider != null && Time.time >= cooldownEndTime)
            {
                horizontalDirection = 1f;
                cooldownEndTime = Time.time + attackCooldown;
                TriggerAttack();
            }
            else if (enemyBackRay.collider != null && Time.time >= cooldownEndTime)
            {
                horizontalDirection = -1f;
                cooldownEndTime = Time.time + attackCooldown;
                TriggerAttack();
            }
            else
            {
                horizontalDirection = 0f;
                animator.SetBool("IsAttacking", false);
            }

            if (horizontalDirection != 0f)
                transform.localScale = new Vector3(-2 * horizontalDirection,
                        transform.localScale.y,
                        transform.localScale.z);

        }
        else
        {
            animator.SetBool("IsAttacking", false);
        }

        animator.SetBool("IsMoving", false);

        if (objectHealth.IsDead)
        {
            EState = EntityState.Dead;
            horizontalDirection = 0f;
        }
        else if (!hostileInCloseRange && Time.time >= cooldownEndTime)
            EState = EntityState.Chase;
    }

    // display ranges in the editor
    // must be commented out to export the game
    private void OnDrawGizmosSelected()
    {
        // Purely for debugging purposes
        // This displays the radius of the detection circle
        // And the radius of the attack circle
        // The offset (0, 1.3, 0) moves the circle up to the center of the sprite
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, 1.3f, 0f), detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0f, 1.3f, 0f), activateAttackRange);
            
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        objectHealth = GetComponent<ObjectHealth>();
    }

}
