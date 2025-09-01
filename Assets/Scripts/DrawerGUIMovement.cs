using UnityEngine;

/// <summary>
/// Handles a smooth movement function for GUIs to push and pull onto the screen when needed
/// </summary>
public class DrawerGUIMovement : MonoBehaviour
{
    [Header("Drawer Properties")]
    [SerializeField]
    [Tooltip("The delta position from the user's screen width to create the pulled/pushed position")]
    private float deltaXChange;

    [SerializeField]
    [Tooltip("The offset of the y from the user's screen height to create the position ")]
    private float yOffset;

    [SerializeField]
    [Tooltip("The time at which the drawer pulls and pushes out")]
    private float dampTime;

    [SerializeField]
    [Tooltip("Toggle for if the user wants the drawer to slowly recede")]
    private bool slowDampOnClose;

    private RectTransform drawerTrans;

    private float sWidth;
    private float sHeight;

    private float vel = 10f;

    private float PulledOutState
    {
        get => sWidth - deltaXChange;
    }
    private float PushedInState
    {
        get => sWidth + deltaXChange;
    }
    private float HeightPosition
    {
        get => sHeight - yOffset;
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
        drawerTrans = this.GetComponent<RectTransform>();

        sWidth = Screen.width;
        sHeight = Screen.height;

        // Fix the offsets off the other parent transforms
        TrimOffsets();

        // Initialize the pushed in position of the drawer
        AdjustPosition(PushedInState);
    }

    private void TrimOffsets()
    {
        RectTransform curTrans = drawerTrans;

        while (curTrans.parent != null)
        {
            curTrans = curTrans.parent.GetComponent<RectTransform>();

            sWidth  -= curTrans.localPosition.x;
            sHeight -= curTrans.localPosition.y;
        }
    }

    private void Update()
    {
        if (inProcess)
            SmoothDrawerMovement();
    }

    private void SmoothDrawerMovement()
    {
        float curXPos = drawerTrans.localPosition.x;

        float newPos = Mathf.SmoothDamp(curXPos, isPullingOut ? PulledOutState : PushedInState, ref vel, SmoothTime);
        AdjustPosition(newPos);

        // Flags if the drawer is fully pulled out or pushed back in to then mark
        // that SmoothDrawerMovement() no longer needs to be called
        if (isPullingOut && curXPos < PulledOutState - 1)
            AdjustPosition(PulledOutState);
        else if (!isPullingOut && curXPos > PushedInState + 1)
            AdjustPosition(PushedInState);
    }

    private void AdjustPosition(float xPos)
    {
        if (xPos == PulledOutState || xPos == PushedInState)
            inProcess = false;

        drawerTrans.localPosition = new Vector2(xPos, HeightPosition);
    }

    public void ToggleDrawer()
    {
        inProcess       = true;
        isPullingOut    = !isPullingOut;
    }
}
