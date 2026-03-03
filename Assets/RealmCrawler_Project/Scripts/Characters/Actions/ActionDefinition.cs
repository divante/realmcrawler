using UnityEngine;

[CreateAssetMenu(
    fileName = "New Action",
    menuName = "RealmCrawler/Characters/Actions/Action")]
public class ActionDefinition : ScriptableObject
{
    [SerializeField]
    [DefaultFileName]
    public string actionName;

    [SerializeReference]
    [SelectImplementation]
    public ActionRuntime actionRuntime = null;

    public ActionRuntime Runtime(GameObject owner)
    {
        var newRef = (ActionRuntime)actionRuntime?.Clone();

        if (newRef == null)
            newRef = new ActionRuntime();

        newRef.Initialize(this, owner);
        return newRef;
    }
}
