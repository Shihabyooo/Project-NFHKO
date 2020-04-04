using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : Triggerable
{
    protected override void StartPlayerTrigger()
    {
        Player.player.SetHidingStatus(true);
    }

    public override void Cancel()
    {
        Player.player.SetHidingStatus(false);
        Player.player.ClearActiveTask();
    }

}
