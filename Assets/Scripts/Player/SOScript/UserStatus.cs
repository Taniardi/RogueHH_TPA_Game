using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "UserStatusScriptableObject", menuName = "ScriptableObjects/UserStatusManager")]
public class UserStatus : ScriptableObject
{
    [SerializeField] public int CurrentHealth;
    [SerializeField] public int Health;
    [SerializeField] public int Attack;
    [SerializeField] public int Defense;
    [SerializeField] public int CriticalRate;
    [SerializeField] public int CriticalDamage;
    [SerializeField] public int Zen;
    [SerializeField] public int MaxFloor; 
    [SerializeField] public int CurrentFloor; 
    [SerializeField] public int CurrentExp;
    [SerializeField] public int Level;
    [SerializeField] public bool isSaved;
}
