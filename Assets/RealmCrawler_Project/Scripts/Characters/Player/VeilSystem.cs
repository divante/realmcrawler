using System;
using RealmCrawler.Spells;
using UnityEngine;

/// <summary>
/// Tracks the Veil's integrity. Casting spells erodes it.
/// As integrity drops: spell power rises, misfires become likely, mutations activate.
/// When fully broken, the world invasion begins.
/// </summary>
public class VeilSystem : MonoBehaviour
{
    [Header("Integrity")]
    [SerializeField] [Range(0f, 1f)] private float veilIntegrity = 1f;

    [Header("Power Scaling")]
    [Tooltip("Spell damage multiplier when Veil is fully broken.")]
    [SerializeField] private float maxPowerMultiplier = 3f;

    [Header("Misfire")]
    [Tooltip("Misfire chance (0-1) when Veil is fully broken.")]
    [SerializeField] private float maxMisfireChance = 0.4f;

    [Header("Mutation")]
    [Tooltip("Mutation activation chance (0-1) when Veil is fully broken.")]
    [SerializeField] private float maxMutationChance = 0.6f;

    [Header("Thresholds")]
    [SerializeField] private float[] warningThresholds = { 0.75f, 0.5f, 0.25f };

    public event Action<float> OnThresholdCrossed;
    public event Action OnVeilBroken;

    public float Integrity => veilIntegrity;

    /// <summary>Spell damage multiplier — increases as veil breaks.</summary>
    public float SpellPowerMultiplier => Mathf.Lerp(1f, maxPowerMultiplier, 1f - veilIntegrity);

    public void ConsumeVeil(float amount)
    {
        float previous = veilIntegrity;
        veilIntegrity = Mathf.Clamp01(veilIntegrity - amount);

        CheckThresholds(previous, veilIntegrity);

        if (veilIntegrity <= 0f && previous > 0f)
            OnVeilBroken?.Invoke();
    }

    /// <summary>Returns true if this cast misfires (fires in wrong direction).</summary>
    public bool RollMisfire()
    {
        float chance = Mathf.Lerp(0f, maxMisfireChance, 1f - veilIntegrity);
        return UnityEngine.Random.value < chance;
    }

    /// <summary>
    /// Returns a random mutation from the spell's list based on instability, or null.
    /// </summary>
    public SpellMutation SelectMutation(SpellData spell)
    {
        if (spell.PossibleMutations.Count == 0) return null;

        float chance = Mathf.Lerp(0f, maxMutationChance, 1f - veilIntegrity);
        if (UnityEngine.Random.value >= chance) return null;

        int index = UnityEngine.Random.Range(0, spell.PossibleMutations.Count);
        return spell.PossibleMutations[index];
    }

    private void CheckThresholds(float previous, float current)
    {
        foreach (float threshold in warningThresholds)
        {
            if (previous > threshold && current <= threshold)
                OnThresholdCrossed?.Invoke(threshold);
        }
    }
}
