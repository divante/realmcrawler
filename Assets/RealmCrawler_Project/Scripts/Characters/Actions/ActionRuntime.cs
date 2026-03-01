using System;
using UnityEngine;

[Serializable]
public class ActionRuntime : ICloneable
{
	public event Action ActionCompleted;

	protected ActionDefinition _definition;
	protected GameObject _owner { get; private set; }

	public virtual void Initialize(ActionDefinition definition, GameObject owner)
	{
		_definition = definition;
		_owner = owner;

	}

	public virtual void Activate() { }
	public virtual void Deactivate() { }
	public virtual void Update() { }
	public virtual void FixedUpdate() { }

	public object Clone()
	{
		string json = JsonUtility.ToJson(this);
		return JsonUtility.FromJson(json, GetType());
	}
}

