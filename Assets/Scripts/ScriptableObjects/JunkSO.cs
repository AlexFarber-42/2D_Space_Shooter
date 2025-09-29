using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "JunkSO", menuName = "Scriptable Objects/JunkSO")]
public class JunkSO : ScriptableObject
{
    public enum JunkEffect
    {
        Metal = 0,
        Core = 1,
        Piece = 2,
        Module = 3
    }

    [HideInInspector] public string junkName;
    [HideInInspector] public Sprite junkSprite;
    [HideInInspector] public int junkPrice;
    [HideInInspector] public JunkEffect junkEffect;
    [HideInInspector] public GameObject junkPieceObject;
    // [HideInInspector] public Module junkModule;
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
        EditorGUILayout.PropertyField(serializedObject.FindProperty("junkPrice"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("junkEffect"));

        switch (container.junkEffect)
        {
            case JunkSO.JunkEffect.Piece:
                EditorGUI.indentLevel = 1;
                EditorGUILayout.Space(3);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("junkPieceObject"));
                break;
         // case JunkSO.JunkEffect.Module:
              //EditorGUI.indentLevel = 1;
              //EditorGUILayout.Space(3);
              //EditorGUILayout.PropertyField(serializedObject.FindProperty("junkModule"));
              //break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
