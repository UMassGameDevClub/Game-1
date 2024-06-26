/**************************************************
Governs the ability hotbar in the corner of the screen.
Updates ActiveAbilityData when the icons are moved.

Documentation updated 4/11/2024
**************************************************/
using UnityEngine;
using UnityEngine.UI;

public class TotalAbilityUI : MonoBehaviour
{
    // Interact with ActiveAbilityData purely for the purposes of display
    // Edit: Currently interacts with ActiveAbilityData to change abilities at slots
    // That means it modifies ActiveAbilityData rather than simply displaying it
    public ActiveAbilityData equippedAbilities;
    public AbilitySlotUI abilitySlotUIPrefab;
    public AbilityImageUI abilityImageUIPrefab;
    AbilitySlotUI[] abilitySlotsUI;
    int MAX_SLOTS = 4;

    // Re-initalize slots when the ability inventory UI closes
    void OnEnable()
    {
        AbilityInventoryUI.abilityInventoryClosed += InitializeAllSlots;
    }

    void OnDisable()
    {
        AbilityInventoryUI.abilityInventoryClosed -= InitializeAllSlots;
    }

    // Subscribes to the Drop event of each AbilitySlotUI
    // When an ability is dropped into that slot,
    // Refreshes then displays the ability
    public void SlotDropListener(int slotNumber)
    {
        RefreshAbilityAtSlot(slotNumber);
        DisplayAbilityAtSlot(slotNumber);
    }

    // Refreshes the ability at the currently stored slot
    void RefreshAbilityAtSlot(int slotNumber)
    {
        AbilitySlotUI currentSlot = abilitySlotsUI[slotNumber];
        if (currentSlot.CurAbilityImageUI == null)
            equippedAbilities.SetAbilityAt(slotNumber, null);
        else
            equippedAbilities.SetAbilityAt(slotNumber, currentSlot.CurAbilityImageUI.CurAbilityInfo);
    }

    // 0 - offense slot
    // 1 - defense slot
    // 2 - utility slot
    // 3 - passive slot
    void DisplayAbilityAtSlot(int slotNumber)
    {
        if (equippedAbilities.AbilityAt(slotNumber) == null)
            return;
        Sprite overlapSprite = equippedAbilities.AbilityAt(slotNumber).overlapIcon;
        Sprite currentSprite = equippedAbilities.AbilityAt(slotNumber).abilityIcons[slotNumber];
        AbilitySlotUI currentSlot = abilitySlotsUI[slotNumber];
        if (currentSlot.CurAbilityImageUI == null)
        {
            currentSlot.CurAbilityImageUI = Instantiate(abilityImageUIPrefab, currentSlot.transform, false);
            currentSlot.CurAbilityImageUI.CurImage.sprite = overlapSprite;
            currentSlot.CurAbilityImageUI.transform.Find("AbilitySubIconUI").GetComponent<Image>().sprite = currentSprite;
            currentSlot.CurAbilityImageUI.CurAbilityInfo = equippedAbilities.AbilityAt(slotNumber);
        }
        else
        {
            currentSlot.CurAbilityImageUI.CurImage.sprite = overlapSprite;
            currentSlot.CurAbilityImageUI.transform.Find("AbilitySubIconUI").GetComponent<Image>().sprite = currentSprite;
        }
    }

    // Fills all slots in the UI
    // Sets the slotIDs of each slot
    // Subscribes to each AbilitySlotUI
    void InitializeAllSlots()
    {
        if (abilitySlotsUI != null)
        {
            foreach(AbilitySlotUI abilitySlot in abilitySlotsUI)
            {
                abilitySlot.UnsubscribeFromDrop(this);
                Destroy(abilitySlot.gameObject);
            }
        }
        abilitySlotsUI = new AbilitySlotUI[MAX_SLOTS];
        for (int i = 0; i < MAX_SLOTS; i++)
        {
            abilitySlotsUI[i] = Instantiate(abilitySlotUIPrefab, transform, false);
            abilitySlotsUI[i].slotID = i;
            abilitySlotsUI[i].SubscribeToDrop(this);
            if (equippedAbilities.AbilityAt(i) != null)
                DisplayAbilityAtSlot(i);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeAllSlots();
    }
}
