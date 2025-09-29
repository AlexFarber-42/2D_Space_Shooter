using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "UpgradeSO", menuName = "Scriptable Objects/UpgradeSO")]
public class UpgradeSO : ScriptableObject
{
    public enum UpgradeID
    {
        AFTERBURNERS = 0,
        PRE_IGNITION = 1,

    }

    [HideInInspector] public UpgradeID upgradeVal;
    [HideInInspector] public Sprite upgradeSprite;
    [HideInInspector] public int upgradeCost;
    [HideInInspector] [TextArea(3, 12)] public string upgradeDescription;
}

#if UNITY_EDITOR
[CustomEditor(typeof(UpgradeSO))]
public class UpgradeEditor: Editor
{
    private UpgradeSO container;

    private void OnEnable()
    {
        container = target as UpgradeSO;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("Upgrade Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(1);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("upgradeVal"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("upgradeSprite"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("upgradeCost"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("upgradeDescription"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
