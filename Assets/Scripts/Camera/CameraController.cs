using Camera;
using Connection;
using Player.ActionHandlers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool isShowGizmos = true;
    [SerializeField] private Vector2 dragAreaSize = new Vector2(20, 20);
    [Space]
    [SerializeField] private Transform nodesParent;

    private bool isDrag = false;
    private Vector3 startDragPosition;
    private Vector3 startCameraPosition;
    private ClickHandler clickHandler;
    private ColorNode[] nodes;
    private float distanceForPixel;

    private void Start()
    {
        distanceForPixel = Vector3.Distance(CameraHolder.Instance.MainCamera.ScreenToWorldPoint(Vector3.zero), CameraHolder.Instance.MainCamera.ScreenToWorldPoint(new Vector3(1, 0, 0)));
        clickHandler = ClickHandler.Instance;
        clickHandler.DragStartEvent += OnStartDrag;
        clickHandler.DragEndEvent += OnStopDrag;
        nodes = nodesParent.GetComponentsInChildren<ColorNode>();
        var cameraPos = CameraHolder.Instance.transform.position;
        cameraPos.x = 0;
        cameraPos.y = 0;
        CameraHolder.Instance.transform.position = cameraPos;
    }

    private void OnDrawGizmos()
    {
        if (!isShowGizmos) return;
        Gizmos.DrawCube(transform.position, new Vector3(dragAreaSize.x, dragAreaSize.y, 1));
    }

    public bool IsCanStartMove(Vector2 position)
    {
        foreach (var colorNode in nodes)
        {
            if (colorNode.IsInBounds(position))
                return false;
        }
        return true;
    }

    private void OnStopDrag(Vector3 finishDragPos)
    {
        isDrag = false;
    }

    private void OnStartDrag(Vector3 startDragPos)
    {
        if (!IsCanStartMove(startDragPos)) return;
        isDrag = true;
        startDragPosition = Input.mousePosition;
        startCameraPosition = CameraHolder.Instance.transform.position;
    }

    private void LateUpdate()
    {
        if (!isDrag) return;
        OnCameraDrag();
    }

    private void OnCameraDrag()
    {
        var cameraTransform = CameraHolder.Instance.transform;
        var secondPosition = Input.mousePosition;
        var newPosition = (startDragPosition - secondPosition) * distanceForPixel + startCameraPosition;
        cameraTransform.position = new Vector3(newPosition.x, newPosition.y, cameraTransform.position.z);
        ClampCameraPosition();
    }

    private void ClampCameraPosition()
    {
        var leftTopCornerPos = CameraHolder.Instance.MainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height));
        var rightBottomCornerPos = CameraHolder.Instance.MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0));
        var areaTopLeftCorner = (Vector2)transform.position - new Vector2(dragAreaSize.x / 2, -dragAreaSize.y / 2);
        var areaBottomRightCorner = (Vector2)transform.position + new Vector2(dragAreaSize.x / 2, -dragAreaSize.y / 2);
        var moveCameraOffset = Vector3.zero;

        if (leftTopCornerPos.x < areaTopLeftCorner.x) moveCameraOffset.x -= leftTopCornerPos.x - areaTopLeftCorner.x;
        if (rightBottomCornerPos.x > areaBottomRightCorner.x) moveCameraOffset.x -= rightBottomCornerPos.x - areaBottomRightCorner.x;
        if (rightBottomCornerPos.y < areaBottomRightCorner.y) moveCameraOffset.y -= rightBottomCornerPos.y - areaBottomRightCorner.y;
        if (leftTopCornerPos.y > areaTopLeftCorner.y) moveCameraOffset.y -= leftTopCornerPos.y - areaTopLeftCorner.y;

        CameraHolder.Instance.transform.position += moveCameraOffset;
    }
}
