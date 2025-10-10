using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "JunkSO", menuName = "Scriptable Objects/JunkSO")]
public class JunkSO : ScriptableObject
{
    public enum JunkEffect
    {
        Metal       = 0,
        Core        = 1,
        Piece       = 2,
        Module      = 3
    }

    public enum JunkRarity
    {
        Common     = 0,     // Magnifies price by 1.00x
        Uncommon   = 1,     // Magnifies price by 1.25x
        Rare       = 2,     // Magnifies price by 1.50x
        Epic       = 3,     // Magnifies price by 2.00x
        Exotic     = 4      // Magnifies price by 2.50x
    }

    [HideInInspector] public string junkName;
    [HideInInspector] public Sprite junkSprite;
    [HideInInspector] public JunkRarity junkRarity = JunkRarity.Common;
    [HideInInspector] public int junkPrice;
    [HideInInspector][TextArea(2,8)] public string[] vendorStrings;
    [HideInInspector] public JunkEffect junkEffect;
    [HideInInspector] public int junkMetalAmount;
    [HideInInspector] public int junkCoreAmount;
    [HideInInspector] public GameObject junkPieceObject;
    [HideInInspector] public ModuleSO junkModule;

    [HideInInspector] public int JunkPrice 
    {
        get
        {
            int value = 0;
                
            switch (junkRarity)
            {
                case JunkRarity.Common:
                    value = junkPrice;
                    break;
                case JunkRarity.Uncommon:
                    value = (int)(junkPrice * 1.25f);
                    break;
                case JunkRarity.Rare:
                    value = (int)(junkPrice * 1.5f);
                    break;
                case JunkRarity.Epic:
                    value = (int)(junkPrice * 2f);
                    break;
                case JunkRarity.Exotic:
                    value = (int)(junkPrice * 2.5f);
                    break;
            }

            return value;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(JunkSO))]
public class JunkEditor: Editor
{
    private JunkSO container;

    private void OnEnable()
    {
        container = target as JunkSO;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("Junk Parameters", EditorStyles.boldLabel);
        EditorGUILayout.Space(2);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("junkName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("junkSprite"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("junkRarity"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("junkPrice"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("vendorStrings"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("junkEffect"));

        EditorGUI.indentLevel = 1;
        EditorGUILayout.Space(3);

        switch (container.junkEffect)
        {
            case JunkSO.JunkEffect.Metal:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("junkMetalAmount"));
                break;
            case JunkSO.JunkEffect.Core:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("junkCoreAmount"));
                break;
            case JunkSO.JunkEffect.Piece:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("junkPieceObject"));
                break;
            case JunkSO.JunkEffect.Module:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("junkModule"));
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
