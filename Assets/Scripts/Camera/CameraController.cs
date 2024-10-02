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
    [SerializeField] private GameObject backPrefab;

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
        SpawnBack();
    }

    private void SpawnBack()
    {
        var backDragObject = Instantiate(backPrefab, nodesParent);
        backDragObject.transform.position = transform.position;
        backDragObject.transform.SetSiblingIndex(0);
        backDragObject.transform.localScale = dragAreaSize;
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

    public void OnCameraDrag()
    {
        var cameraTransform = CameraHolder.Instance.transform;
        var secondPosition = Input.mousePosition;
        var newPosition = (startDragPosition - secondPosition) * distanceForPixel + startCameraPosition;
        cameraTransform.position = new Vector3(newPosition.x, newPosition.y, cameraTransform.position.z);
    }
}
