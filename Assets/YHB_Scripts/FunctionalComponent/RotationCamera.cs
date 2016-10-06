/*
作者名称:YHB

脚本作用:用来做相机绕整体的一个场景旋转，或用作产品展示时围绕产品旋转

建立时间:2016.9.27.15.17
*/

using UnityEngine;

/// <summary>
/// 脚本挂载在相机主摄像机上，TargetLookAt放整个场景或产品
/// </summary>
public class RotationCamera : MonoBehaviour
{
    #region +
    public Transform TargetLookAt; //绕这个目标点旋转
    public float Distance = 10.0f;//相机距离目标点的初始距离
    public float DistanceMin = 5.0f;//鼠标滚轮缩放拉近推远相机视角的最小距离
    public float DistanceMax = 15.0f;//最大的距离
    public float X_MouseSensitivity = 5.0f;//鼠标X轴的敏感度
    public float Y_MouseSensitivity = 5.0f;//鼠标Y轴的敏感度
    public float MouseWheelSensitivity = 5.0f;//鼠标滚轮的敏感度
    public float X_Smooth = 0.05f;//X轴移动的平滑插值
    public float Y_Smooth = 0.1f;//Y轴移动的平滑插值
    public float Y_MinLimit = 15.0f;//看上看下Y轴最小的位置
    public float Y_MaxLimit = 70.0f;//看上看下Y轴最大的位置
    public float DistanceSmooth = 0.025f;//距离的平滑插值
    #endregion

    #region -
    private float startingDistance = 0.0f;//开始目标点里脚本挂载的物体的距离距离
    private float desiredDistance = 0.0f;
    private float mouseX = 0.0f;
    private float mouseY = 0.0f;
    private float velocityDistance = 0.0f;
    private Vector3 desiredPosition = Vector3.zero;
    private float velX = 0.0f;
    private float velY = 0.0f;
    private float velZ = 0.0f;
    private Vector3 position = Vector3.zero;
    #endregion

    #region Unity内置函数
    void Start()
    {
        Distance = Vector3.Distance(TargetLookAt.transform.position, gameObject.transform.position);

        if (Distance > DistanceMax)//如果初始距离超出了限制的最大距离
        {
            DistanceMax = Distance;//那就以初始距离为最大距离
        }

        startingDistance = Distance;
        Reset();
    }
    void LateUpdate()
    {
        if (TargetLookAt == null)
        {
            return;
        }

        HandlePlayerInput();
        CalculateDesiredPosition();
        UpdatePosition();
    }
    #endregion

    #region -HandlePlayerInput玩家输入功能
    private void HandlePlayerInput()
    {
        float deadZone = 0.01f;//滚轮死区

        if (Input.GetMouseButton(0))
        {
            mouseX += Input.GetAxis("Mouse X") * X_MouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * Y_MouseSensitivity;
        }

        mouseY = ClampAngle(mouseY, Y_MinLimit, Y_MaxLimit);   //限制鼠标Y轴

        //获取鼠标滚轮输入  -0.01到0.01之间是不会进行相机的拉近推远的
        if (Input.GetAxis("Mouse ScrollWheel") < -deadZone || Input.GetAxis("Mouse ScrollWheel") > deadZone)
        {
            desiredDistance = Mathf.Clamp(Distance - (Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity), DistanceMin, DistanceMax);
        }
    }
    #endregion

    #region -计算方法

    #region   -CalculateDesiredPosition计算渴望得到的距离
    private void CalculateDesiredPosition()
    {
        Distance = Mathf.SmoothDamp(Distance, desiredDistance, ref velocityDistance, DistanceSmooth);

        //鼠标输入反向将对齐轴
        desiredPosition = CalculatePosition(mouseY, mouseX, Distance);
    }
    #endregion

    #region -CalculatePosition计算位置
    private Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        return TargetLookAt.position + (rotation * direction);
    }
    #endregion

    #endregion

    #region  -功能方法

    #region -UpdatePosition更新相机的位置
    private void UpdatePosition()
    {
        float posX = Mathf.SmoothDamp(position.x, desiredPosition.x, ref velX, X_Smooth);
        float posY = Mathf.SmoothDamp(position.y, desiredPosition.y, ref velY, Y_Smooth);
        float posZ = Mathf.SmoothDamp(position.z, desiredPosition.z, ref velZ, X_Smooth);
        position = new Vector3(posX, posY, posZ);
        transform.position = position;
        transform.LookAt(TargetLookAt);
    }
    #endregion

    #region -Reset重置鼠标变量
    private void Reset()
    {
        mouseX = 0;
        mouseY = 0;
        Distance = startingDistance;
        desiredDistance = Distance;
    }
    #endregion

    #region  -ClampAngle最小浮点数和最大浮值之间的夹角
    private float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360.0f || angle > 360.0f)
        {
            if (angle < -360.0f)
            {
                angle += 360.0f;
            }

            if (angle > 360.0f)
            {
                angle -= 360.0f;
            }
        }

        return Mathf.Clamp(angle, min, max);
    }
    #endregion

    #endregion

}
