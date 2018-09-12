using System;
using UnityEngine;

[CreateAssetMenu]
public class moveSet : ScriptableObject {
    [SerializeField] attackSet[] attackSetList;
    
    public int comboLength
    {
        get
        {
            return attackSetList.Length;
        }
    }
    
    public attackData getAttack(combatSystem.attackType type, int attacksDone)
    {
        
        attackSet set = attackSetList[attacksDone];

        switch (type)
        {
            case combatSystem.attackType.nuetral:
                return set.neutral;
            case combatSystem.attackType.side:
                return set.side;
            case combatSystem.attackType.up:
                return set.up;
            case combatSystem.attackType.down:
                return set.down;
            case combatSystem.attackType.special:
                return set.special;
            default:
                throw new Exception("Attempted to get data of invalid attack type");
        }
        
    }
    
}

[System.Serializable]
public class attackSet
{
    public attackData neutral;
    public attackData side;
    public attackData up;
    public attackData down;
    public attackData special;
}

[System.Serializable]
public struct attackData
{
    public bool enabled;
    public Vector2 attackForce;
}