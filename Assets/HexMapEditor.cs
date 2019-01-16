using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour
{
    public Color[] colors;
    public HexGrid hexGrid;

    private Color activeColor;
    private int activeElevation;
    private bool applyColor;                // 是否应用颜色
    private bool applyElevation = true;     // 是否应用高度
    private int brushSize;                  // 笔刷大小

    void Awake()
    {
        SelectColor(0);
    }

    void Update()
    {
        if (Input.GetMouseButton(0) &&
            !EventSystem.current.IsPointerOverGameObject())
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            EditCells(hexGrid.GetCell(hit.point));
        }
    }

    void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - r; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    void EditCell(HexCell cell)
    {
        if (cell == null)
        {
            return;
        }
        if (applyColor)
        {
            cell.Color = activeColor;
        }
        if (applyElevation)
        {
            cell.Elevation = activeElevation;
        }
    }

    // 是否应用颜色
    public void SelectColor(int index)
    {
        applyColor = index >= 0;
        if (applyColor)
        {
            activeColor = colors[index];
        }
    }

    // 是否应用高度
    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    // 设置高度
    public void SetElevation(float elevation)
    {
        activeElevation = (int) elevation;
    }

    // 设置笔刷大小
    public void SetBrushSize(float size)
    {
        brushSize = (int) size;
    }

    // 是否显示UI
    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }
}
