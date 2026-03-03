using RealmCrawler.Equipment;
using UnityEngine;

/// <summary>
/// Tracks live combo state so SpellCastAction can detect a mid-combo cast and apply a bonus.
/// Written by ComboAttackAction, read by SpellCastAction.
/// </summary>
public class CombatContext : MonoBehaviour
{
    public bool IsInCombo { get; private set; }
    public int ComboHitsLanded { get; private set; }
    public ComboNode CurrentNode { get; private set; }

    public void BeginCombo(ComboNode rootNode)
    {
        IsInCombo = true;
        ComboHitsLanded = 0;
        CurrentNode = rootNode;
    }

    public void AdvanceCombo(ComboNode nextNode)
    {
        ComboHitsLanded++;
        CurrentNode = nextNode;
    }

    public void EndCombo()
    {
        IsInCombo = false;
        ComboHitsLanded = 0;
        CurrentNode = null;
    }
}
