using System;
using UnityEngine;

/// <summary>
/// Manages 4 spell slots with cooldown tracking
/// </summary>
public class SpellManager : MonoBehaviour
{
    [Serializable]
    public class SpellSlot
    {
        [Tooltip("Spell assigned to this slot")]
        public SpellDefinition spell;
        
        [Tooltip("Slot index (0-3)")]
        public int slotIndex;
        
        [HideInInspector]
        public float cooldownTimer;
        
        [HideInInspector]
        public bool isOnCooldown;
        
        public bool CanCast()
        {
            return spell != null && !isOnCooldown;
        }
        
        public void StartCooldown()
        {
            if (spell == null) return;
            
            isOnCooldown = true;
            cooldownTimer = spell.cooldown;
            OnCooldownStart?.Invoke(slotIndex);
        }
        
        public void UpdateCooldown(float deltaTime)
        {
            if (!isOnCooldown) return;
            
            cooldownTimer -= deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                cooldownTimer = 0f;
                OnCooldownComplete?.Invoke(slotIndex);
            }
            
            OnCooldownUpdate?.Invoke(slotIndex, cooldownTimer, spell.cooldown);
        }
    }
    
    public event Action<int> OnCooldownStart;
    public event Action<int> OnCooldownComplete;
    public event Action<int, float, float> OnCooldownUpdate;
    
    [SerializeField]
    private SpellSlot[] spellSlots = new SpellSlot[4];
    
    private SpellRuntime[] activeSpellRuntimes;
    
    private void Awake()
    {
        activeSpellRuntimes = new SpellRuntime[4];
        
        for (int i = 0; i < spellSlots.Length; i++)
        {
            spellSlots[i].slotIndex = i;
        }
    }
    
    public SpellSlot GetSpellSlot(int index)
    {
        if (index >= 0 && index < spellSlots.Length)
            return spellSlots[index];
        return null;
    }
    
    public bool TryCastSpell(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= spellSlots.Length)
            return false;
        
        SpellSlot slot = spellSlots[slotIndex];
        if (!slot.CanCast())
            return false;
        
        // Create and activate spell runtime
        SpellRuntime spellRuntime = slot.spell.CreateRuntime(this.gameObject);
        spellRuntime.SpellSlotIndex = slotIndex;
        spellRuntime.OnSpellComplete += () => slot.StartCooldown();
        
        activeSpellRuntimes[slotIndex] = spellRuntime;
        spellRuntime.Activate();
        
        return true;
    }
    
    public void Update()
    {
        // Update cooldowns
        for (int i = 0; i < spellSlots.Length; i++)
        {
            spellSlots[i].UpdateCooldown(Time.deltaTime);
        }
        
        // Update active spell runtimes
        for (int i = 0; i < activeSpellRuntimes.Length; i++)
        {
            if (activeSpellRuntimes[i] != null)
            {
                activeSpellRuntimes[i].Update();
            }
        }
    }
    
    public void ResetCooldowns()
    {
        for (int i = 0; i < spellSlots.Length; i++)
        {
            spellSlots[i].cooldownTimer = 0f;
            spellSlots[i].isOnCooldown = false;
        }
    }
}
