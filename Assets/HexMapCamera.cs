using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
    public float stickMinZoom, stickMaxZoom;            // 最小/最大缩放量
    public float swivelMinZoom, swivelMaxZoom;          // 最小/最大旋转量
    public float moveSpeedMinZoom, moveSpeedMaxZoom;    // 最小/最大移速
    public float rotationSpeed;     // 旋转速度
    public HexGrid grid;

    private Transform swivel, stick;

    private float zoom = 1f;
    private float rotationAngle;

    void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (!Mathf.Approximately(zoomDelta, 0f))
        {
            AdjustZoom(zoomDelta);
        }

        float rotationDelta = Input.GetAxis("Rotation");
        if (!Mathf.Approximately(rotationDelta, 0f))
        {
            AdjustRotation(rotationDelta);
        }

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (!Mathf.Approximately(xDelta, 0f) || !Mathf.Approximately(zDelta, 0f))
        {
            AdjustPosition(xDelta, zDelta);
        }
    }

    private void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    private void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f)
        {
            rotationAngle += 360f;
        }
        else if (rotationAngle >= 360f)
        {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }

    private void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    /// <summary>
    /// 限制移动区域
    /// </summary>
    private Vector3 ClampPosition(Vector3 position)
    {
        float xMax = (grid.chunkCountX * HexMetrics.chunkSizeX - 0.5f) *
                     (2f * HexMetrics.innerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = (grid.chunkCountZ * HexMetrics.chunkSizeZ - 1f) *
                     (1.5f * HexMetrics.outerRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }
}
