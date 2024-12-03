using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils 
{
    private static Utils util;
    public static Utils GetInstance()
    {
        if(util == null)
        {
            util = new Utils();
        }
        return util;
    }

    public bool CheckPositionXZ(Tile tile, Tile tile2)
    {
        return tile.tileGameObject.transform.position.x == tile2.tileGameObject.transform.position.x &&
            tile.tileGameObject.transform.position.z == tile2.tileGameObject.transform.position.z;
    }


    public int CalculateDamage(float attack, float defense, int floor)
    {
        int defenseScalingFactor = Mathf.Max(20, 100 - floor);
        float defenseFactor = 1 - (defense / (defense + defenseScalingFactor));

        float damageOutput = attack * defenseFactor;

        System.Random rand = new System.Random();
        float randomizationFactor = (float)(rand.NextDouble() * (1.05 - 0.95) + 0.95); // Random factor between 0.95 and 1.05
        damageOutput *= randomizationFactor;

        return (int)damageOutput;
    }
}
