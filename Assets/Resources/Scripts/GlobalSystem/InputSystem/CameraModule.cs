using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    Transform CameraRoot;

    private InputAction RightClick; // Camera Rotation
    private InputAction LeftClick; // For Selecting, Raycasting
    private InputAction MiddleClick;
    private InputAction Scroll; // For Zooming
    private InputAction CursorMove; // For Camera Movement and axis

    private InputAction KeyboardMove; // For Camera Movement and axis
    
    public float moveSpeed = 5f;
    public float rotationSpeed = 0.5f;
    public float scrollSpeed = 0.5f;

    private bool isLeftClicking = false;
    private bool isRightClicking = false;
    private Vector2 moveInput;
    private float scrollInput;

    private bool[] isKeyPressed = new bool[6]; // 0: Q, 1: W, 2: E, 3: A, 4: S, 5: D
    private float xSpeed = 0f;
    private float zSpeed = 0f;
    private float ySpeed = 0f;

    // max zoom (height) : 10, min zoom (height) : 1000
    private float maxHeight = 1000f;
    private float minHeight = 10f;

    private void Update()
    {
        // 키보드 입력이 계속해서 이동을 유지하도록 수정
        keyEventHandler();
    }

    private void Awake()
    {
        CameraRoot = transform.parent;

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

        // Keyboard Input W A S D Q E
        KeyboardMove = new InputAction("KeyboardMove", InputActionType.Value);
        
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


        KeyboardMove.AddCompositeBinding("3DVector")
            .With("Forward", "<Keyboard>/w")
            .With("Backward", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d")
            .With("Up", "<Keyboard>/q")
            .With("Down", "<Keyboard>/e");

        KeyboardMove.performed += ctx => onMoveKeyboard(ctx);
        KeyboardMove.canceled += ctx => onMoveKeyboardCanceled(ctx);

    }

    private void EventEnable()
    {
        // InputAction 이벤트를 활성화
        RightClick.Enable();
        LeftClick.Enable();
        MiddleClick.Enable();
        Scroll.Enable();
        CursorMove.Enable();
        KeyboardMove.Enable();
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

    public bool isRightCameraPosition(float cameraWorldSpaceYPosition)
    {
        if(cameraWorldSpaceYPosition < maxHeight && cameraWorldSpaceYPosition > minHeight)
        {
            return true;
        }

        Debug.Log("too high or log : " + cameraWorldSpaceYPosition);
        return false;
    }
    private void OnZoom(InputAction.CallbackContext context)
    {
        float scrollValue = context.ReadValue<float>();
        // 절댓값 사용
        float zoomAmount = (scrollValue * scrollSpeed);

        //Debug.Log("Scroll Input: " + scrollValue);

        // 적절한 위치가 아니면 줌을 하지 않음
        if(!isRightCameraPosition(transform.position.y + -1f*zoomAmount) || !isRightCameraPosition(transform.position.y - zoomAmount))
        {
            return;
        }
        Zoom(zoomAmount);
    }

    private void onMoveKeyboardCanceled(InputAction.CallbackContext context)
    {
        // console
        // Debug.Log("버튼 캔슬");

        Vector3 input = context.ReadValue<Vector3>();
        
        // q w e a s d
        if (isKeyPressed[0])
            isKeyPressed[0] = false;
        if (isKeyPressed[1])
            isKeyPressed[1] = false;
        if (isKeyPressed[2])
            isKeyPressed[2] = false;
        if (isKeyPressed[3])
            isKeyPressed[3] = false;
        if (isKeyPressed[4])
            isKeyPressed[4] = false;
        if (isKeyPressed[5])
            isKeyPressed[5] = false;

        xSpeed = 0f;
        ySpeed = 0f;
        zSpeed = 0f;
    }

    private void onMoveKeyboard(InputAction.CallbackContext context)
    {
        Vector3 input = context.ReadValue<Vector3>();

        // console
        Debug.Log("버튼 입력 : " + input);

        // key event handler is called at update function
        // keyEventHandler();

        // q w e a s d
        if (Keyboard.current.qKey.isPressed)
            isKeyPressed[0] = true;
        else
            isKeyPressed[0] = false;

        if(Keyboard.current.wKey.isPressed)
            isKeyPressed[1] = true;
        else
            isKeyPressed[1] = false;

        if(Keyboard.current.eKey.isPressed)
            isKeyPressed[2] = true;
        else
            isKeyPressed[2] = false;

        if(Keyboard.current.aKey.isPressed)
            isKeyPressed[3] = true;
        else
            isKeyPressed[3] = false;

        if(Keyboard.current.sKey.isPressed)
            isKeyPressed[4] = true;
        else
            isKeyPressed[4] = false;

        if(Keyboard.current.dKey.isPressed)
            isKeyPressed[5] = true;
        else
            isKeyPressed[5] = false;

        xSpeed = 0f;
        ySpeed = 0f;
        zSpeed = 0f;
    }
    private void keyEventHandler()
    {
        // log foreach isKeyPressed
        //Debug.Log("isKeyPressed : " + isKeyPressed[0] + " " + isKeyPressed[1] + " " + isKeyPressed[2] + " " + isKeyPressed[3] + " " + isKeyPressed[4] + " " + isKeyPressed[5]);

        // W S (Z 축)
        if (isKeyPressed[1])
            zSpeed = 1f;
        else if (isKeyPressed[4])
            zSpeed = -1f;

        // 좌우 (X 축)
        if (isKeyPressed[3])
            xSpeed = -1f;
        else if (isKeyPressed[5])
            xSpeed = 1f;

        // 하강/상승 (Y 축)
        if (isKeyPressed[0])
        {
            // 최소 높이를 넘게 될 경우 하강하지 않음
            // 루트의 월드좌표 + 카메라의 로컬좌표 + 이동량 = 카메라의 월드 좌표
            float cameraWorldSpaceYPosition = CameraRoot.position.y + transform.localPosition.y + -1f;
            // log all values
            //Debug.Log("values : " + transform.position.y + " " + CameraRoot.position.y + " " + cameraWorldSpaceYPosition);

            if (isRightCameraPosition(cameraWorldSpaceYPosition))
                ySpeed = -1f;
            else
            {
                xSpeed = 0f;
            }
        }
        if (isKeyPressed[2])
        {
            // 최대 높이를 넘게 될 경우 상승하지 않음
            // 루트 + 카메라 + 이동량 = 카메라의 월드 좌표
            float cameraWorldSpaceYPosition = transform.localPosition.y + CameraRoot.position.y;

            //Debug.Log("values : " + transform.position.y + " " + CameraRoot.position.y + " " + cameraWorldSpaceYPosition);

            if (isRightCameraPosition(cameraWorldSpaceYPosition))
                ySpeed = 1f;
            else
            {
                xSpeed = 0f;
            }

            ySpeed = 1f;
        }

        // 이동
        Vector3 moveDirection = new Vector3(xSpeed, ySpeed, zSpeed).normalized;
        Vector3 moveAmount = moveDirection * moveSpeed;

        //
        CameraRoot.Translate(moveAmount * Time.deltaTime, Space.Self);
    }

    private void Rotate()
    {
        if (isRightClicking)
        {
            float mouseX = Mouse.current.delta.x.ReadValue();
            float mouseY = Mouse.current.delta.y.ReadValue();
            float rotationAmountX = mouseX * rotationSpeed;
            float rotationAmountY = -mouseY * rotationSpeed * 0.5f;
            // console log values
            Debug.Log("Mouse X: " + mouseX + " Mouse Y: " + mouseY + " Rotation X: " + rotationAmountX + " Rotation Y: " + rotationAmountY);

            // 카메라의 로컬 좌표를 회전시키는 대신, 월드 좌표를 기준으로 회전
            CameraRoot.Rotate(Vector3.up, rotationAmountX, Space.World);
            transform.Rotate(Vector3.right, rotationAmountY, Space.Self);
        }
    }

    private void Zoom(float zoomAmount)
    {
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
       // Debug.Log("Mouse Position: " + input);
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