using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public Transform Cube;
    [SerializeField] private float CameraSpeed = 1;
    private int m_FieldSize;
    private Camera m_Camera;

    private const float MaxZoom = 100f;
    private const float MinZoom = 0.35f;
    
    public void SetInitialCameraPosition(int fieldSize)
    {
        m_FieldSize = fieldSize;
        m_Camera = GetComponent<Camera>();
        transform.position = new Vector3(fieldSize / 2f, fieldSize / 2f + 1.5f, -1.5f);
        m_Camera.orthographicSize = 0.35f;
    }

    // Update is called once per frame
    void Update()
    {
        var horizontalMovement = Input.GetAxis("Horizontal");
        var verticalMovement = Input.GetAxis("Vertical");

        var mouseScrollDelta = -Input.mouseScrollDelta.y;
        if (mouseScrollDelta != 0)
        {
            if (mouseScrollDelta > 0)
            {
                //Проверяем выход границ камеры за границы поля
                m_Camera.orthographicSize += mouseScrollDelta * Time.unscaledDeltaTime * CameraSpeed;
                var plan = GeometryUtility.CalculateFrustumPlanes(m_Camera);
                for (var i = 0; i < 4; i++)
                {
                    //Если хотя бы одна из границ камеры вышла за границы поля, отменяем зум
                    var flag = false;
                    switch (i)
                    {
                        case 0:
                            flag = IsPlaneOutsideBounds(plan[i], new Vector3(-.5f, 0, 0),
                                new Vector3(m_FieldSize - 0.5f, 0, 0));
                            break;
                        case 1:
                            flag = IsPlaneOutsideBounds(plan[i], new Vector3(-.5f, 0, 0),
                                new Vector3(m_FieldSize - 0.5f, 0, 0));
                            break;
                        case 2:
                            flag = IsPlaneOutsideBounds(plan[i], new Vector3(0, 0, -.5f),
                                new Vector3(0, 0, m_FieldSize));
                            break;
                        case 3:
                            flag = IsPlaneOutsideBounds(plan[i], new Vector3(0, 0, -.5f),
                                new Vector3(0, 0, m_FieldSize));
                            break;
                    }

                    if (!flag) continue;
                    m_Camera.orthographicSize -= mouseScrollDelta * Time.unscaledDeltaTime * CameraSpeed;
                    break;
                }
            }
            else
            {
                m_Camera.orthographicSize += mouseScrollDelta * Time.unscaledDeltaTime * CameraSpeed;
                if (m_Camera.orthographicSize < MinZoom)
                    m_Camera.orthographicSize -= mouseScrollDelta * Time.unscaledDeltaTime * CameraSpeed;
            }
        }

        if (horizontalMovement != 0)
        {
            transform.position += Vector3.right * (horizontalMovement * Time.unscaledDeltaTime * CameraSpeed);
            
            //Проверяем нахождение границ камеры за границами поля
            var leftPoint = new Vector3(-.5f, 0, 0);
            var rightPoint = new Vector3(m_FieldSize-0.5f, 0, 0);
            
            var planes = GeometryUtility.CalculateFrustumPlanes(m_Camera);
            //Левая плоскость
            var leftPlaneCheck = IsPlaneOutsideBounds(planes[0], leftPoint, rightPoint);
            //Правая плоскость
             var rightPlaneCheck = IsPlaneOutsideBounds(planes[1], leftPoint, rightPoint);
            
            if (leftPlaneCheck || rightPlaneCheck)
            {
                transform.position -= Vector3.right * (horizontalMovement * Time.unscaledDeltaTime * CameraSpeed);
            }
        }
        if (verticalMovement != 0)
        {
            transform.position += Vector3.up * (verticalMovement * Time.unscaledDeltaTime * CameraSpeed);
            
            //Проверяем нахождение границ камеры за границами поля
            var downPoint = new Vector3(0, 0, -.5f);
            var upPoint = new Vector3(0, 0, m_FieldSize);
            
            var planes = GeometryUtility.CalculateFrustumPlanes(m_Camera);
            
            //Нижняя плоскость
            var downPlaneCheck = IsPlaneOutsideBounds(planes[2], downPoint, Vector3.up);
            //Верхняя плоскость
            var upPlaneCheck = IsPlaneOutsideBounds(planes[3], downPoint, Vector3.up);
            
            if (downPlaneCheck || upPlaneCheck)
            {
                transform.position -= Vector3.up * (verticalMovement * Time.unscaledDeltaTime * CameraSpeed);
            }
        }
    }

    private bool IsPlaneOutsideBounds(Plane plane, Vector3 point1, Vector3 point2)
    {
        var pointCheck = plane.GetSide(point1);
        return pointCheck == plane.GetSide(point2) && pointCheck;
    }
}
