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
        // InputAction 이벤트를 생성 및 바인딩
        RightClick = new InputAction("LeftClick", InputActionType.Button, "<Mouse>/rightButton"); //
        LeftClick = new InputAction("RightClick", InputActionType.Button, "<Mouse>/leftButton");
        MiddleClick = new InputAction("MiddleClick", InputActionType.Button, "<Mouse>/middleButton");
        Scroll = new InputAction("Scroll", InputActionType.Value, "<Mouse>/scroll/y");
        CursorMove = new InputAction("CursorMove", InputActionType.Value, "<Mouse>/position"); //

        // 이벤트에 대한 트리거를 설정
        RightClick.performed += ctx => OnRightClick();
        RightClick.canceled += ctx => OnRightClickCanceled();

        LeftClick.performed += ctx => OnLeftClick();
        LeftClick.canceled += ctx => OnLeftClickCanceled();

        MiddleClick.performed += ctx => OnMiddleClick();
        MiddleClick.canceled += ctx => OnMiddleClickCanceled();

        Scroll.performed += ctx => OnScroll();
        CursorMove.performed += ctx => OnMove(ctx);

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
    private void OnRightClick()
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