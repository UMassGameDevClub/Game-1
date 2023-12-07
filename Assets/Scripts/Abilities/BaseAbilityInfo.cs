using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AbilityForm { Offense, Defense, Utility, Passive };

public abstract class BaseAbilityInfo : ScriptableObject
{
    [Header("Basic Ability Info")]
    public int abilityID;
    public string abilityName = "DEFAULT_ABILITY";
    public Sprite overlapIcon;
    public List<Sprite> abilityIcons;

    [Header("Advanced Ability Info")]
    public AbilityForm currentForm;
    public int baseCost = 0;
    public float duration = 0f;
    public float tickRate = 0f;
    public float chargeUp = 0f;
    public float cooldown = 0f;
    public float endTime = 0f;
    public int damage = 0;

    // TODO: Implement ability effects
    [Header("Ability Effect Info")]
    public List<AbilityEffect> universalEffects;
    public List<AbilityEffect> offenseEffects;
    public List<AbilityEffect> defenseEffects;
    public List<AbilityEffect> utilityEffects;
    public List<AbilityEffect> passiveEffects;

    /*[Header("Advanced Ability Info")]*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected abstract void AbilityOffense(AbilityOwner abilityOwner);

    protected abstract void AbilityDefense(AbilityOwner abilityOwner);

    protected abstract void AbilityUtility(AbilityOwner abilityOwner);

    protected abstract void AbilityPassive(AbilityOwner abilityOwner);

    public virtual void AbilityUpdate(AbilityOwner abilityOwner) { }

    public virtual void AbilityActivate(AbilityOwner abilityOwner)
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
        // endTime = Time.time + duration;
    }
}
