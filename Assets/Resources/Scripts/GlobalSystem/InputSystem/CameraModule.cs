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
        // Ű���� �Է��� ����ؼ� �̵��� �����ϵ��� ����
        keyEventHandler();
    }

    private void Awake()
    {
        CameraRoot = transform.parent;

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
        // InputAction �̺�Ʈ�� Ȱ��ȭ
        RightClick.Enable();
        LeftClick.Enable();
        MiddleClick.Enable();
        Scroll.Enable();
        CursorMove.Enable();
        KeyboardMove.Enable();
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
        // ���� ���
        float zoomAmount = (scrollValue * scrollSpeed);

        //Debug.Log("Scroll Input: " + scrollValue);

        // ������ ��ġ�� �ƴϸ� ���� ���� ����
        if(!isRightCameraPosition(transform.position.y + -1f*zoomAmount) || !isRightCameraPosition(transform.position.y - zoomAmount))
        {
            return;
        }
        Zoom(zoomAmount);
    }

    private void onMoveKeyboardCanceled(InputAction.CallbackContext context)
    {
        // console
        // Debug.Log("��ư ĵ��");

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
        Debug.Log("��ư �Է� : " + input);

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

        // W S (Z ��)
        if (isKeyPressed[1])
            zSpeed = 1f;
        else if (isKeyPressed[4])
            zSpeed = -1f;

        // �¿� (X ��)
        if (isKeyPressed[3])
            xSpeed = -1f;
        else if (isKeyPressed[5])
            xSpeed = 1f;

        // �ϰ�/��� (Y ��)
        if (isKeyPressed[0])
        {
            // �ּ� ���̸� �Ѱ� �� ��� �ϰ����� ����
            // ��Ʈ�� ������ǥ + ī�޶��� ������ǥ + �̵��� = ī�޶��� ���� ��ǥ
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
            // �ִ� ���̸� �Ѱ� �� ��� ������� ����
            // ��Ʈ + ī�޶� + �̵��� = ī�޶��� ���� ��ǥ
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

        // �̵�
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

            // ī�޶��� ���� ��ǥ�� ȸ����Ű�� ���, ���� ��ǥ�� �������� ȸ��
            CameraRoot.Rotate(Vector3.up, rotationAmountX, Space.World);
            transform.Rotate(Vector3.right, rotationAmountY, Space.Self);
        }
    }

    private void Zoom(float zoomAmount)
    {
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