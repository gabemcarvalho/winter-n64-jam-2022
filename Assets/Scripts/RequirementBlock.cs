using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class RequirementBlock 
{
    public enum Type {
        GREATER_THAN,
        LESS_THAN,
        EQUAL_TO
    }

    public DecorationInfo decorationInfo;
    public int amount;

    public Type requirement;
}
