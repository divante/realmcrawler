using UnityEngine;

namespace RealmCrawler.Spells
{
    /// <summary>
    /// Base class for a designer-authored spell mutation applied at high Veil instability.
    /// Create concrete subclasses (e.g. SplitMutation, ReverseMutation) and assign them
    /// to SpellData.possibleMutations in the inspector.
    /// </summary>
    public abstract class SpellMutation : ScriptableObject
    {
        [SerializeField] [TextArea(1, 3)] private string mutationDescription;
        public string MutationDescription => mutationDescription;

        /// <summary>
        /// Called by SpellCastAction after the spell projectile is spawned.
        /// Implement mutation behaviour here (split, redirect, detonate, etc.)
        /// </summary>
        public abstract void Apply(GameObject spellInstance, Vector3 fireOrigin, Quaternion fireRotation);
    }
}
