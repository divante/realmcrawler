using UnityEngine;

[CreateAssetMenu(
	fileName = "New Weapon",
	menuName = "RealmCrawler/Characters/Weapons/Weapon")]
public class WeaponDefinition : ScriptableObject
{
	[Header("Weapon Info")]
	[DefaultFileName]
	[SerializeField]
	private string _weaponName;
	public string WeaponName => _weaponName;

	[Header("Attack Patterns")]
	[SerializeField]
	private AttackPattern _fastAttack;
	public AttackPattern FastAttack => _fastAttack;

	[SerializeField]
	private AttackPattern _heavyAttack;
	public AttackPattern HeavyAttack => _heavyAttack;

	[Header("Animation")]
	[SerializeField]
	private AnimationClip _fastAttackClip;
	public AnimationClip FastAttackClip => _fastAttackClip;

	[SerializeField]
	private AnimationClip _heavyAttackClip;
	public AnimationClip HeavyAttackClip => _heavyAttackClip;

	[Header("Properties")]
	[SerializeField]
	private float _switchTime = 0.5f;
	public float SwitchTime => _switchTime;

	[SerializeField]
	private LayerMask _enemyLayer;
	public LayerMask EnemyLayer => _enemyLayer;
}
