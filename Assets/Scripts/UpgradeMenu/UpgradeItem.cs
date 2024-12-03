using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeItemScriptableObject", menuName = "ScriptableObjects/UpgradeItemManager")]
public class UpgradeItem : ScriptableObject
{
    [SerializeField] public Sprite ItemImage;
    [SerializeField] public string Name;
    [SerializeField] public string Description;
    [SerializeField] public int Price;
    [SerializeField] public int UpgradeValue;
    [SerializeField] public int ItemLevel;
}
