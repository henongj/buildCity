using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private InputAction RightClick; // Camera Rotation
    private InputAction LeftClick; // For Selecting, Raycasting
    private InputAction MiddleClick;
    private InputAction Scroll; // For Zooming
    private InputAction CursorMove; // For Camera Movement and axis

    private InputAction KeyboardMove; // For Camera Movement and axis

    public float moveSpeed = 5f;
    public float rotationSpeed = 2f;
    public float scrollSpeed = 5f;

    private bool isLeftClicking = false;
    private bool isRightClicking = false;
    private Vector2 moveInput;
    private float scrollInput;

    private void Awake()
    {
        // InputAction �̺�Ʈ�� ���� �� ���ε�
        GenerateInputAction();

        // �̺�Ʈ Ʈ���Ÿ� ����
        BindEvent();

        // InputAction �̺�Ʈ�� Ȱ��ȭ
        EventEnable();
    }

    private void GenerateInputAction()
    {
        // InputAction �̺�Ʈ�� ���� �� ���ε�
        RightClick = new InputAction("LeftClick", InputActionType.Button, "<Mouse>/rightButton"); //
        LeftClick = new InputAction("RightClick", InputActionType.Button, "<Mouse>/leftButton");
        MiddleClick = new InputAction("MiddleClick", InputActionType.Button, "<Mouse>/middleButton");
        Scroll = new InputAction("Scroll", InputActionType.Value, "<Mouse>/scroll/y");
        CursorMove = new InputAction("CursorMove", InputActionType.Value, "<Mouse>/position"); //
        KeyboardMove = new InputAction("KeyboardMove", InputActionType.Value, "<Keyboard>/position"); //
    }

    private void BindEvent()
    {
        RightClick.performed += ctx => OnRightClick(ctx);
        RightClick.canceled += ctx => OnRightClickCanceled();

        LeftClick.performed += ctx => OnLeftClick();
        LeftClick.canceled += ctx => OnLeftClickCanceled();

        MiddleClick.performed += ctx => OnMiddleClick();
        MiddleClick.canceled += ctx => OnMiddleClickCanceled();

        Scroll.performed += ctx => OnScroll(ctx);
        CursorMove.performed += ctx => OnCursorMove(ctx);

        KeyboardMove.performed += ctx => onMoveKeyboard(ctx);
    }

    private void EventEnable()
    {
        // InputAction �̺�Ʈ�� Ȱ��ȭ
        RightClick.Enable();
        LeftClick.Enable();
        MiddleClick.Enable();
        Scroll.Enable();
        CursorMove.Enable();
    }

    private void OnDisable()
    {
        // InputAction �̺�Ʈ�� ��Ȱ��ȭ
        RightClick.Disable();
        LeftClick.Disable();
        MiddleClick.Disable();
        Scroll.Disable();
        CursorMove.Disable();
    }


    private void OnRotate(InputAction.CallbackContext context)
    {
        Rotate();
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>();
        Debug.Log("Scroll Input: " + scrollValue);
        Zoom(scrollValue);
    }

    private void onMoveKeyboard(InputAction.CallbackContext context)
    {
        float horizontal = 0f;
        float vertical = 0f;

        // �յ�
        if (Keyboard.current.wKey.isPressed)
            vertical = 1f;
        else if (Keyboard.current.sKey.isPressed)
            vertical = -1f;

        // �¿�
        if (Keyboard.current.aKey.isPressed)
            horizontal = -1f;
        else if (Keyboard.current.dKey.isPressed)
            horizontal = 1f;

        // �ϰ�
        if (Keyboard.current.qKey.isPressed)
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.Self);
        }
        // ���
        else if (Keyboard.current.eKey.isPressed)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);
        }

        // �̵�
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveAmount = moveDirection * moveSpeed * Time.deltaTime;
        transform.Translate(moveAmount, Space.Self);
    }

    private void Rotate()
    {
        if (isRightClicking)
        {
            float mouseX = moveInput.x;
            float mouseY = moveInput.y;

            float rotationAmountX = -mouseY * rotationSpeed;
            float rotationAmountY = mouseX * rotationSpeed;

            transform.Rotate(rotationAmountX, rotationAmountY, 0f, Space.Self);
        }
    }

    private void Zoom(float scrollAmount)
    {
        float zoomAmount = scrollAmount * scrollSpeed;
        Vector3 zoom = new Vector3(0f, 0f, zoomAmount);

        transform.Translate(zoom, Space.Self);
    }


    // ��Ŭ�� �̺�Ʈ �ڵ鷯
    private void OnLeftClick()
    {
        isLeftClicking = true;
        Debug.Log("Left Click");
    }

    // ��Ŭ�� ĵ�� �̺�Ʈ �ڵ鷯
    private void OnLeftClickCanceled()
    {
        isLeftClicking = false;
        Debug.Log("Left Click Canceled");
    }

    // ��Ŭ�� �̺�Ʈ �ڵ鷯
    private void OnRightClick(InputAction.CallbackContext context)
    {
        isRightClicking = true;
        Debug.Log("Right Click");

    }

    // ��Ŭ�� ĵ�� �̺�Ʈ �ڵ鷯
    private void OnRightClickCanceled()
    {
        isRightClicking = false;
        Debug.Log("Right Click Canceled");
    }

    // ��Ŭ�� �̺�Ʈ �ڵ鷯
    private void OnMiddleClick()
    {
        Debug.Log("Middle Click");
    }

    // ��Ŭ�� ĵ�� �̺�Ʈ �ڵ鷯
    private void OnMiddleClickCanceled()
    {
        Debug.Log("Middle Click Canceled");
    }

    // ��ũ�� �̺�Ʈ �ڵ鷯
    private void OnScroll(InputAction.CallbackContext context)
    {
        OnZoom(context);
    }

    private void OnCursorMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (isLeftClicking)
        {
            OnLeftDrag();
        }

        if (isRightClicking)
        {
            OnRightDrag(context);
        }

        // console mouse position
        Debug.Log("Mouse Position: " + input);
    }

    private void OnLeftDrag()
    {
        Debug.Log("Left Drag");
    }
    private void OnRightDrag(InputAction.CallbackContext context)
    {
        Debug.Log("Right Drag");
        OnRotate(context);
    }
}