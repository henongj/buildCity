using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestInputAction : MonoBehaviour
{
    public InputAction RightClick;
    public InputAction LeftClick;
    public InputAction MiddleClick;
    public InputAction Scroll;
    public InputAction CursorMove;

    private bool isLeftClicking = false;
    private bool isRightClicking = false;

    private void Awake()
    {
        // InputAction �̺�Ʈ�� ���� �� ���ε�
        RightClick = new InputAction("LeftClick", InputActionType.Button, "<Mouse>/rightButton"); //
        LeftClick = new InputAction("RightClick", InputActionType.Button, "<Mouse>/leftButton");
        MiddleClick = new InputAction("MiddleClick", InputActionType.Button, "<Mouse>/middleButton");
        Scroll = new InputAction("Scroll", InputActionType.Value, "<Mouse>/scroll/y");
        CursorMove = new InputAction("CursorMove", InputActionType.Value, "<Mouse>/position"); //

        // �̺�Ʈ�� ���� Ʈ���Ÿ� ����
        RightClick.performed += ctx => OnRightClick();
        RightClick.canceled += ctx => OnRightClickCanceled();

        LeftClick.performed += ctx => OnLeftClick();
        LeftClick.canceled += ctx => OnLeftClickCanceled();

        MiddleClick.performed += ctx => OnMiddleClick();
        MiddleClick.canceled += ctx => OnMiddleClickCanceled();

        Scroll.performed += ctx => OnScroll();
        CursorMove.performed += ctx => OnMove(ctx);

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
    private void OnRightClick()
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
    private void OnScroll()
    {
        float scrollValue = Scroll.ReadValue<float>();
        Debug.Log("Scroll: " + scrollValue);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (isLeftClicking)
        {
            OnLeftDrag();
        }

        if (isRightClicking)
        {
            OnRightDrag();
        }

        // console mouse position
        Debug.Log("Mouse Position: " + input);
    }

    private void OnLeftDrag()
    {
        Debug.Log("Left Drag");
    }
    private void OnRightDrag()
    {
        Debug.Log("Right Drag");
    }
}