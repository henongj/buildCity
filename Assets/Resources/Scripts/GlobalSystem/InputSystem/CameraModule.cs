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
        // InputAction 이벤트를 생성 및 바인딩
        GenerateInputAction();

        // 이벤트 트리거를 설정
        BindEvent();

        // InputAction 이벤트를 활성화
        EventEnable();
    }

    private void GenerateInputAction()
    {
        // InputAction 이벤트를 생성 및 바인딩
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
        // InputAction 이벤트를 활성화
        RightClick.Enable();
        LeftClick.Enable();
        MiddleClick.Enable();
        Scroll.Enable();
        CursorMove.Enable();
    }

    private void OnDisable()
    {
        // InputAction 이벤트를 비활성화
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

        // 앞뒤
        if (Keyboard.current.wKey.isPressed)
            vertical = 1f;
        else if (Keyboard.current.sKey.isPressed)
            vertical = -1f;

        // 좌우
        if (Keyboard.current.aKey.isPressed)
            horizontal = -1f;
        else if (Keyboard.current.dKey.isPressed)
            horizontal = 1f;

        // 하강
        if (Keyboard.current.qKey.isPressed)
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime, Space.Self);
        }
        // 상승
        else if (Keyboard.current.eKey.isPressed)
        {
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime, Space.Self);
        }

        // 이동
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


    // 좌클릭 이벤트 핸들러
    private void OnLeftClick()
    {
        isLeftClicking = true;
        Debug.Log("Left Click");
    }

    // 좌클릭 캔슬 이벤트 핸들러
    private void OnLeftClickCanceled()
    {
        isLeftClicking = false;
        Debug.Log("Left Click Canceled");
    }

    // 우클릭 이벤트 핸들러
    private void OnRightClick(InputAction.CallbackContext context)
    {
        isRightClicking = true;
        Debug.Log("Right Click");

    }

    // 우클릭 캔슬 이벤트 핸들러
    private void OnRightClickCanceled()
    {
        isRightClicking = false;
        Debug.Log("Right Click Canceled");
    }

    // 휠클릭 이벤트 핸들러
    private void OnMiddleClick()
    {
        Debug.Log("Middle Click");
    }

    // 휠클릭 캔슬 이벤트 핸들러
    private void OnMiddleClickCanceled()
    {
        Debug.Log("Middle Click Canceled");
    }

    // 스크롤 이벤트 핸들러
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