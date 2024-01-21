using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRepairable
{
    public void RepairSystem(inventoryItem requiredTool, int useCost = 1, float repairMult = 1);
}