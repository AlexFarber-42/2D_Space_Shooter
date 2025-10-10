using System.Collections.Generic;
using UnityEngine;

public class ModuleManager : MonoBehaviour
{
    [SerializeField] private Transform moduleContentHolder;

    private ModuleSlot[] moduleSlots;
    private ModuleSO[] availableModules;

    private List<ModuleSO> activatedModules;


}
