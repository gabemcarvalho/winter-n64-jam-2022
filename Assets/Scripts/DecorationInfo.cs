using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDecoration", menuName = "Decoration")]
public class DecorationInfo : ScriptableObject
{
    public string displayName;
    public Sprite uiIcon;
    public string projectileResource = "Decorations/NewDecorationProjectile";
}
