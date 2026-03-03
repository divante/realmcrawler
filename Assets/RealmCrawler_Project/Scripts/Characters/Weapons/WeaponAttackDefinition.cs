using UnityEngine;

[CreateAssetMenu(
	fileName = "New WeaponAttack",
	menuName = "RealmCrawler/Characters/Weapons/WeaponAttack")]
public class WeaponAttackDefinition : ActionDefinition
{
	[Tooltip("The weapon this attack uses")]
	[SerializeField] private WeaponDefinition weapon;

	[Tooltip("Type of attack (Fast or Heavy)")]
	[SerializeField] private AttackType attackType;

	[Tooltip("Attack pattern data (timing, damage, hitbox)")]
	[SerializeField] private AttackPattern attackPattern;

	[Tooltip("Can this attack chain into another attack?")]
	[SerializeField] private bool canChain = false;

	[Tooltip("Animation clip for this attack")]
	[SerializeField] private AnimationClip animationClip;

	// Getters
	public WeaponDefinition Weapon => weapon;
	public AttackType AttackType => attackType;
	public AttackPattern AttackPattern => attackPattern;
	public bool CanChain => canChain;
	public AnimationClip AnimationClip => animationClip;

	// Override to create WeaponAttackRuntime
	override public ActionRuntime Runtime(GameObject owner)
	{
		if (weapon == null)
		{
			Debug.LogError($"[WeaponAttackDefinition] No weapon assigned to {actionName}!");
			return null;
		}

		if (attackPattern == null)
		{
			Debug.LogError($"[WeaponAttackDefinition] No attack pattern assigned to {actionName}!");
			return null;
		}

		var runtime = new WeaponAttackRuntime();
		runtime.Initialize(this, owner);
		return runtime;
	}
}
