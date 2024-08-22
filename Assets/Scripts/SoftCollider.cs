using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used in the soft collider prefab, which is put as a child onto other objects.
/// Allows for other objects to pass through this object, but push them out if they stay inside this object.
/// 
/// For example, a player can pass through a pot or an enemy, but will be pushed away if they try to stand inside it.
/// Think of how animals in Minecraft push each other around when there's too many in a small area.
/// 
/// Documentation updated 8/12/2024
/// </summary>
/// \author Roy Pascual
/// \note Only applies to objects on the "Push" layer.
public class SoftCollider : MonoBehaviour
{
    /// <summary> Reference to the rigidbody of the parent object. </summary>
    /// <value></value>
    public Rigidbody2D ParentBody { get; set; }
    /// <summary> List of all objects colliding with the parent object. </summary>
    List<SoftCollider> opposingColliders;
    /// <summary> How much the opposing colliders should be pushed away. </summary>
    float pushMagnitude = 3f;

    /// <summary>
    /// Runs when an object enters the soft collider.
    /// Adds any new opposing colliders to the opposingColliders list.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Push"))
        {
            SoftCollider opposingCollider = collision.GetComponent<SoftCollider>();
            if (!opposingColliders.Contains(opposingCollider))
                opposingColliders.Add(opposingCollider);
        }
    }

    /// <summary>
    /// Runs every frame an object is in the soft collider.
    /// Calculate and apply momentum to each object in opposingColliders that pushes them away from this object.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (opposingColliders.Count > 0)
        {
            foreach (SoftCollider opposingCollider in opposingColliders)
            {
                Vector2 difference = opposingCollider.transform.position - transform.position;
                float pushDirection = 0f;
                if (difference.x > 0f)
                {
                    pushDirection = 1f;
                }
                else
                {
                    pushDirection = -1f;
                }
                float absXDiff = Mathf.Abs(difference.x);

                // Calculate push relative to distance between transforms
                float pushDistRatio = (100f + absXDiff) / (100f + 800f * Mathf.Pow(absXDiff, 3));
                
                // Calculate push relative to current horizontal velocity
                float pushFactor = (30f + Mathf.Abs(opposingCollider.ParentBody.velocity.x))
                    / (30f + 10f * Mathf.Pow(Mathf.Abs(opposingCollider.ParentBody.velocity.x),2));
                
                // Apply push
                opposingCollider.ParentBody.velocity =
                    new Vector2(opposingCollider.ParentBody.velocity.x + (pushDirection * pushMagnitude * pushDistRatio * pushFactor),
                    opposingCollider.ParentBody.velocity.y);
            }
        }
    }

    /// <summary>
    /// Runs when an object exits the soft collider.
    /// Remove any leaving opposing colliders from the opposingColliders list.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Push"))
        {
            SoftCollider opposingCollider = collision.GetComponent<SoftCollider>();
            if (opposingColliders.Contains(opposingCollider))
                opposingColliders.Remove(opposingCollider);
        }
    }

    /// <summary>
    /// Initialize ParentBody and opposingColliders.
    /// </summary>
    void Awake()
    {
        ParentBody = transform.parent.GetComponent<Rigidbody2D>();
        opposingColliders = new List<SoftCollider>();
    }
}
