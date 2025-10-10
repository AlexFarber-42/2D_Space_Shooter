using UnityEngine;
using UnityEngine.UI;

public class ModuleSlot : MonoBehaviour
{
    [SerializeField] private Image moduleSprite;

    private ModuleSO containedModule;

    public void InsertNewModule(ModuleSO newModule)
    {
        containedModule = newModule;
        moduleSprite.sprite = containedModule.moduleSprite;
    }

    public void ActivateModule()
    {
        switch (containedModule.moduleID)
        {
            case ModuleSO.ModuleID.OMNI_MOVEMENT:
                break;
            case ModuleSO.ModuleID.AUTO_DRONE:
                break;
            case ModuleSO.ModuleID.SHIELD:
                break;
            default:
                Debug.Log($"Unimplemented Module detected {containedModule.name}");
                break;
        }
    }
}
