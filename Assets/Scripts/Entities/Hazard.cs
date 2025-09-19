using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class Hazard : MonoBehaviour
{
    [Header("Hazard Metrics")]
    [SerializeField] private int spriteFrameCount;
    [SerializeField] private float scaleSet;
    [SerializeField] private int damageValue = 1;
    [HideInInspector] public bool containsLight = false;

    [Header("Light Settings")]
    [SerializeField] private LightStepData[] lightSteps = new LightStepData[0];

    private CircleCollider2D cirCol;
    private Animator anim;
    private AnimationClip clipInfo;
    private Light2D lightComp;

    private List<GameObject> affectedEntities = new List<GameObject>();

    public int Damage
    {
        get => damageValue;
    }

    private void Awake()
    {
        cirCol   = GetComponent<CircleCollider2D>();
        anim     = GetComponent<Animator>();
        clipInfo = anim.runtimeAnimatorController.animationClips.FirstOrDefault();

        if (containsLight)
        {
            lightComp = GetComponent<Light2D>();

            SetLightValues(0);
        }
    }

    public void CreateHazard()
    {
        StartCoroutine(ProcessHazard());
    }

    public bool MarkEntity(GameObject entity)
    {
        if (affectedEntities.Contains(entity))
            return false;
        else
        {
            affectedEntities.Add(entity);
            return true;
        }
    }

    private IEnumerator ProcessHazard()
    {
        cirCol.radius = scaleSet;

        float clipTime = clipInfo.length;

        // Create steps
        float delta = clipTime / spriteFrameCount;
        float step = scaleSet / spriteFrameCount;

        int index = 0;

        if (containsLight && (lightSteps.Length != spriteFrameCount))
            Debug.LogWarning($"Mismatch in {this.name} for spriteFrameCount ({spriteFrameCount}) and lightSteps length ({lightSteps.Length})");

        while (index < spriteFrameCount)
        {
            cirCol.radius += step;

            if (containsLight)
                SetLightValues(index);

            ++index;
            yield return new WaitForSeconds(delta);
        }

        Pools.Instance.RemoveObject(gameObject);
    }

    private void SetLightValues(int index)
    {
        // TODO ---> Possible rework this to include other light types via switch case
        lightComp.intensity             = lightSteps[index].intensity;
        lightComp.pointLightInnerRadius = lightSteps[index].innerRadius;
        lightComp.falloffIntensity      = lightSteps[index].falloffVal;
        lightComp.volumeIntensity       = lightSteps[index].volIntensity;
        lightComp.color                 = lightSteps[index].lightColor;
    }
}

[System.Serializable]
public struct LightStepData
{
    public float intensity;
    public float volIntensity;
    public float innerRadius;
    public float falloffVal;
    public Color lightColor;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Hazard))]
public class HazardEditor : Editor
{
    private Hazard container;

    private void OnEnable()
    {
        container = target as Hazard;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.indentLevel = 0;
        EditorGUILayout.LabelField("Hazard Properties", EditorStyles.boldLabel);
        EditorGUILayout.Space(3);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("spriteFrameCount"), new GUIContent("Sprite Frame Count", "The total number of frames in the hazard's animation sprite sheet."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scaleSet"), new GUIContent("Scale Set", "The maximum scale size the hazard will reach during its animation (for the collider)."));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("damageValue"), new GUIContent("Damage Value", "The amount of damage this hazard will inflict on entities that come into contact with it."), GUILayout.ExpandWidth(true));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("containsLight"), new GUIContent("Contains Light", "Determines if the hazard has an associated light effect."));

        if (container.containsLight)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("lightSteps"), new GUIContent("Light Steps", "Defines the light effect properties at various steps during the hazard's animation."));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
