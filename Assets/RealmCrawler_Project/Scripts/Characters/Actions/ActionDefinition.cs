using UnityEngine;

[CreateAssetMenu(
		fileName = "New State",
		menuName = "RealmCrawler/Characters/Actions/Basic")]
public class ActionDefinition : ScriptableObject
{
	[DefaultFileName]
	[SerializeField]
	public string actionName;

	[SerializeReference]
	[SelectImplementation]
	public ActionRuntime actionRuntime = null;

	public ActionRuntime Runtime(GameObject owner)
	{
		var newRef = (ActionRuntime)actionRuntime?.Clone();

		if (newRef == null) newRef = new ActionRuntime();

		newRef.Initialize(this, owner);
		return (ActionRuntime)newRef;
	}
}
