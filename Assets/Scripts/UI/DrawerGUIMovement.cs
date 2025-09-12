using UnityEngine;
using System.Net.Http.Headers;
using System.Collections;



#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handles a smooth movement function for GUIs to push and pull onto the screen when needed
/// </summary>
public class DrawerGUIMovement : MonoBehaviour
{
    public enum DrawerType
    {
        X,
        Y
    }

    public enum Drawer_ScreenLocation
    {
        Start,
        Middle,
        End,
        Custom
    }

    [Header("Drawer Properties")]
    [Tooltip("The drawer's movement direction")]
    public DrawerType drawerType = DrawerType.X; // Always x or y
    public Drawer_ScreenLocation drawerLocation = Drawer_ScreenLocation.Custom;

    [SerializeField]
    [HideInInspector]
    [Tooltip("The delta position from the user's screen width to create the pulled/pushed position")]
    private float deltaXChange;

    [SerializeField]
    [HideInInspector]
    [Tooltip("The delta position from the user's screen height to create the pulled/pushed position")]
    private float deltaYChange;

    [SerializeField]
    [HideInInspector]
    [Tooltip("The offset of the y from the user's screen height to create the position ")]
    private int yOffset;

    [SerializeField]
    [HideInInspector]
    [Tooltip("The offset of the x from the user's screen width to create the position ")]
    private int xOffset;

    [SerializeField]
    [Tooltip("The time at which the drawer pulls and pushes out")]
    private float dampTime;

    [SerializeField]
    [Tooltip("Toggle for if the user wants the drawer to slowly recede")]
    private bool slowDampOnClose;

    private RectTransform drawerTrans;

    private float sWidth;
    private float sHeight;

    [SerializeField]
    [Tooltip("Initial speed at which the drawer starts its movement")]
    private float velocity = 10f;

    private float PulledOutState
    {
        get => drawerType is DrawerType.X ? WidthPosition - deltaXChange : HeightPosition + deltaYChange;
    }
    private float PushedInState
    {
        get => drawerType is DrawerType.X ? WidthPosition + deltaXChange : HeightPosition - deltaYChange;
    }
    private float HeightPosition
    {
        get
        {
            if (drawerType is DrawerType.Y)
            {
                switch (drawerLocation)
                {
                    case Drawer_ScreenLocation.Start:   // Starts at bottom of screen
                        return -sHeight;
                    case Drawer_ScreenLocation.Middle:  // Starts in the middle of the screen on the sides
                        return 0;
                    case Drawer_ScreenLocation.End:     // Starts at top of screen
                        return sHeight;
                    case Drawer_ScreenLocation.Custom:  // Unique location, simply break out
                        break;
                }
            }

            return sHeight - yOffset;
        }
    }
    private float WidthPosition
    {
        get
        {
            if (drawerType is DrawerType.X)
            {
                switch (drawerLocation)
                {
                    case Drawer_ScreenLocation.Start:   // Starts at left of screen
                        return 0;
                    case Drawer_ScreenLocation.Middle:  // Starts in the middle of the screen
                        return sWidth / 2;
                    case Drawer_ScreenLocation.End:     // Starts at right of screen
                        return sWidth;
                    case Drawer_ScreenLocation.Custom:  // Unique location, simply break out
                        break;
                }
            }

            return sWidth - xOffset;
        }
    }
    private float SmoothTime
    {
        get
        {
            if (!isPullingOut && slowDampOnClose)
                return dampTime * 4;
            else
                return dampTime;
        }
    }

    private bool isPullingOut    = false;
    private bool inProcess       = false;

    private void Awake()
    {
        drawerTrans = GetComponent<RectTransform>();

        sWidth  = Screen.width;
        sHeight = Screen.height;

        // Fix the offsets off the other parent transforms
        TrimOffsets();

        // Initialize the pushed in position of the drawer
        switch (drawerType)
        {
            case DrawerType.X:
                AdjustPosition(PushedInState, HeightPosition);
                break;
            case DrawerType.Y:
                AdjustPosition(WidthPosition, PushedInState);
                break;
        }
    }

    private void TrimOffsets()
    {
        RectTransform curTrans = drawerTrans;

        while (curTrans.parent != null)
        {
            curTrans = curTrans.parent.GetComponent<RectTransform>();

            sWidth   -= curTrans.localPosition.x;
            sHeight  -= curTrans.localPosition.y;
        }
    }

    private void Update()
    {
        if (inProcess)
            SmoothDrawerMovement();
    }

    private void SmoothDrawerMovement()
    {
        float curPos = drawerType is DrawerType.X ? drawerTrans.localPosition.x : drawerTrans.localPosition.y;
        float newPos = Mathf.SmoothDamp(curPos, isPullingOut ? PulledOutState : PushedInState, ref velocity, SmoothTime);
        float xPos   = drawerType is DrawerType.X ? newPos : WidthPosition;
        float yPos   = drawerType is DrawerType.Y ? newPos : HeightPosition;

        AdjustPosition(xPos, yPos);

        if (drawerType is DrawerType.X)
        {
            if (isPullingOut && curPos < PulledOutState - 1)
                AdjustPosition(PulledOutState, yPos);
            else if (!isPullingOut && curPos > PushedInState + 1)
                AdjustPosition(PushedInState, yPos);
        }
        else
        {
            if (isPullingOut && curPos > PulledOutState - 1)
                AdjustPosition(xPos, PulledOutState);
            else if (!isPullingOut && curPos < PushedInState + 1)
                AdjustPosition(xPos, PushedInState);
        }
    }

    private void AdjustPosition(float xPos, float yPos)
    {
        bool hasReachedXThresh = drawerType is DrawerType.X && (xPos == PulledOutState || xPos == PushedInState);
        bool hasReachedYThresh = drawerType is DrawerType.Y && (yPos == PulledOutState || yPos == PushedInState);

        bool hasReachedThreshold = hasReachedXThresh || hasReachedYThresh;
        
        if (hasReachedThreshold)
            inProcess = false;

        drawerTrans.localPosition = new Vector2(xPos, yPos);
    }

    public void ToggleDrawer()
    {
        inProcess       = true;
        isPullingOut    = !isPullingOut;
    }

    private Coroutine activeSlip;

    public void TriggerSlip()
    {
        // Reset the coroutine if money is updated again
        if (activeSlip != null)
            StopAllCoroutines();
        else
            ToggleDrawer();

        activeSlip = StartCoroutine(DrawerSlip());
    }

    private readonly float drawerLinger = 3.5f;

    private IEnumerator DrawerSlip()
    {
        yield return new WaitForSeconds(drawerLinger);

        ToggleDrawer();
        activeSlip = null;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DrawerGUIMovement))]
public class DrawerGUIEditor : Editor
{
    private DrawerGUIMovement container;

    private void OnEnable()
    {
        container = target as DrawerGUIMovement;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Drawer Properties", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("drawerType"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("drawerLocation"));
        EditorGUILayout.Space(15);

        switch (container.drawerType)
        {
            case DrawerGUIMovement.DrawerType.X:

                EditorGUILayout.PropertyField(serializedObject.FindProperty("deltaXChange"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("yOffset"));

                break;
            case DrawerGUIMovement.DrawerType.Y:

                EditorGUILayout.PropertyField(serializedObject.FindProperty("deltaYChange"));

                EditorGUILayout.PropertyField(serializedObject.FindProperty("xOffset"));

                break;
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("dampTime"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("slowDampOnClose"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("velocity"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
