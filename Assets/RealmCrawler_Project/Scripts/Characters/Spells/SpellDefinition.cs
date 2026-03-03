using UnityEngine;

[CreateAssetMenu(
	fileName = "New Spell",
	menuName = "RealmCrawler/Characters/Spells/Spell")]
public class SpellDefinition : ScriptableObject
{
	[DefaultFileName]
	[SerializeField]
	public string spellName;

	[Tooltip("Type of spell effect")]
	public SpellType spellType;

	[Tooltip("Cooldown time in seconds")]
	[SerializeField]
	public float cooldown = 2f;

	[Tooltip("Mana/energy cost")]
	[SerializeField]
	public float cost = 10f;

	[Tooltip("Time to cast the spell")]
	[SerializeField]
	public float castTime = 0.5f;

	[Tooltip("Spell effect parameters")]
	[SerializeField]
	public SpellEffect effect;

	[Tooltip("Animation clip for casting")]
	[SerializeField]
	public AnimationClip animationClip;

	[Tooltip("Spell slot index (0-3)")]
	[SerializeField]
	public int spellSlotIndex = 0;
}

public enum SpellType
{
	Projectile,
	AoE,
	Buff,
	Debuff,
	Instant
}

[Serializable]
public class SpellEffect
{
	[Tooltip("Damage amount")]
	public float damage = 10f;

	[Tooltip("Projectile speed (for Projectile type)")]
	public float projectileSpeed = 20f;

	[Tooltip("Projectile lifetime in seconds")]
	public float projectileLifetime = 3f;

	[Tooltip("Area radius (for AoE type)")]
	public float areaRadius = 5f;

	[Tooltip("Effect duration (for Buff/Debuff)")]
	public float effectDuration = 5f;

	[Tooltip("VFX prefab to spawn")]
	public GameObject vfxPrefab;

	[Tooltip("Audio clip to play")]
	public AudioClip audioClip;
}
