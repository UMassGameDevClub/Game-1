/**************************************************
This is an example ability with placeholder functionality.
Useful for understanding the syntax for the ability system.
This is a scriptable object, meaning you can make and instance of it in the editor.

Documentation updated 1/29/2024
**************************************************/
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "New ExampleAbilityInfo", menuName = "Abilities/Create New ExampleAbilityInfo")]
public class ExampleAbilityInfo : BaseAbilityInfo
{
    [Header("Custom Ability Info")]
    public Transform projectilePrefab;
    public Transform effectPrefab;
    GameObject tempAbilitySpawn;

    private void PlaceholderEffect(AbilityOwner abilityOwner)
    {
        Instantiate(effectPrefab, abilityOwner.OwnerTransform.position, Quaternion.identity);
    }

    protected override void AbilityOffense(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Offense");
        Transform ownerTransform = abilityOwner.OwnerTransform;

        // Placeholder Effect below
        if (tempAbilitySpawn != null)
            Destroy(tempAbilitySpawn);
        tempAbilitySpawn = Instantiate(projectilePrefab,
            ownerTransform.position + new Vector3(0f, 1f, 0f),
            Quaternion.identity).gameObject;
        tempAbilitySpawn.transform.Find("TestText").GetComponent<TextMeshPro>().text = "Offense";
    }

    protected override void AbilityDefense(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Defense");
        PlayerHealth playerHealth = abilityOwner.OwnerTransform.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.HealInstant(damage);
        }

        Transform ownerTransform = abilityOwner.OwnerTransform;

        // Placeholder Effect below
        if (tempAbilitySpawn != null)
            Destroy(tempAbilitySpawn);
        tempAbilitySpawn = Instantiate(projectilePrefab,
            ownerTransform.position + new Vector3(0f, 1f, 0f),
            Quaternion.identity).gameObject;
        tempAbilitySpawn.transform.Find("TestText").GetComponent<TextMeshPro>().text = "Defense";
    }

    protected override void AbilityUtility(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Utility");

        Transform ownerTransform = abilityOwner.OwnerTransform;

        // Placeholder Effect below
        if (tempAbilitySpawn != null)
            Destroy(tempAbilitySpawn);
        tempAbilitySpawn = Instantiate(projectilePrefab,
            ownerTransform.position + new Vector3(0f, 1f, 0f),
            Quaternion.identity).gameObject;
        tempAbilitySpawn.transform.Find("TestText").GetComponent<TextMeshPro>().text = "Utility";
    }

    protected override void AbilityPassive(AbilityOwner abilityOwner)
    {
        Debug.Log("Example Passive");
    }

    public override void AbilityUpdate(AbilityOwner abilityOwner) {
        Debug.Log("Update : " + Time.time);
        PlayerHealth playerHealth = abilityOwner.OwnerTransform.GetComponent<PlayerHealth>();
        Debug.Log("Health : " + playerHealth.GetHealth());
        if (playerHealth != null)
        {
            playerHealth.HealInstant(damage);
        }
    }

    public override void AbilityDisable(AbilityOwner abilityOwner, AbilityEffectType effectType)
    {
        base.AbilityDisable(abilityOwner, effectType);
    }

    // Covered by the virtual method in BaseAbilityInfo
    // Can override the function here for more specific functionality
    /*public override void AbilityActivate(AbilityOwner abilityOwner)
    {
        switch (currentForm)
        {
            case AbilityForm.Offense:
                AbilityOffense(abilityOwner);
                break;
            case AbilityForm.Defense:
                AbilityDefense(abilityOwner);
                break;
            case AbilityForm.Utility:
                AbilityUtility(abilityOwner);
                break;
            case AbilityForm.Passive:
                AbilityPassive(abilityOwner);
                break;
            default:
                break;
        }
    }*/
}
