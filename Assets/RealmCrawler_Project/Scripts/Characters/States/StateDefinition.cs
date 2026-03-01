using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "New State",
    menuName = "RealmCrawler/Characters/FSM/State")]
public class StateDefinition : ScriptableObject
{
  [DefaultFileName]
  [SerializeField]
  public string stateName;

  [SerializeReference]
  [SelectImplementation]
  public List<StateTransitionBase> transitions = new();

  [SerializeReference]
  public List<ActionDefinition> actions = new();
  
}