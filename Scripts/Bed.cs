using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : InteractableObject
{
    public override void Pickup()
    {
        string sleepText = "��Ҫ˯����";
        UIManager.Instance.TriggerYesNoPrompt(sleepText, GameStateManager.Instance.Sleep);
    }
}
