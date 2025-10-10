using UnityEngine;

[CreateAssetMenu(fileName = "ModuleSO", menuName = "Scriptable Objects/ModuleSO")]
public class ModuleSO : ScriptableObject
{
    public enum ModuleID
    {
        OMNI_MOVEMENT      = 0, // Module to allow the player to rotate completely around
        AUTO_DRONE         = 1, // Module to allow the player to have a drone that fire at enemies
        SHIELD             = 2, // Module to allow the player to have a shield that absorbs damage
    }

    public string moduleName;
    public Sprite moduleSprite;
    public ModuleID moduleID;
}
