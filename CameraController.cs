using DG.Tweening;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    public Transform target;
    public float xSpeed = 200, ySpeed = 200, mSpeed = 10;  //分别定义相机横向、纵向的旋转速度，视野缩放速度
    public float yMinLimit = 5, yMaxLimit = 50; //限制纵向的最小最大旋转角
    public float distance = 50, minDistance = 2, maxDistance = 100; //设置初始状态相机的视野范围，以及相机所能缩放的最小最大范围
    public bool needDamping = true; //是否需要相机阻尼效果
    float damping = 5f; //相机阻尼系数
    public float x = 0f; //初始状态照相机横向旋转角度
    public float y = 0f; //初始状态照相机纵向旋转角度
    public bool isS;

    private Vector3 m_mouseMovePos;
    private Camera camera;

    public static CameraController _instance;
    public Transform LowestT;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        camera = GetComponent<Camera>();
    }


    // 定义相机灵敏度
    public float sensitivity = 2f;
    // 定义相机移动速度
    public float moveSpeed = 3f; 
    // Update is called once per frame
    void LateUpdate()
    {

        if (target)
        {
            //移动鼠标
            //if (isS && Input.GetMouseButton(2))
            //{
            //    float mouseX = Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
            //    float mouseY = Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;
            //    Vector3 move = new Vector3(0, -mouseY, mouseX);

            //    // === 新增/修改的逻辑：限制最低点 ===
            //    // 1. 计算应用移动后target的预期新位置
            //    Vector3 projectedPosition = target.position + move;
            //    // 2. 如果新位置的Y轴（假设地面在XZ平面，高度为Y轴）低于最低点，则限制其Y坐标
            //    if (projectedPosition.y < LowestT.position.y)
            //    {
            //        // 只修正Y坐标，保持X和Z的移动
            //        projectedPosition.y = LowestT.position.y;
            //        // 重新计算所需的移动向量（从当前位置到限制后的新位置）
            //        move = projectedPosition - target.position;
            //    }
            //    // === 逻辑结束 ===

            //    target.Translate(move, Space.Self);
            //}

            // ✅ 永远朝向相机（水平方向，无上下倾斜）
            Vector3 toCamera = this.transform.position - target.position;
            toCamera.y = 0;
            target.rotation = Quaternion.LookRotation(toCamera);

            //if (isS && Input.GetMouseButton(2))
            //{
            //    float mouseX = Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
            //    float mouseY = Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;

            //    // 在世界空间中计算移动方向
            //    Vector3 localMove = new Vector3(mouseX, -mouseY, 0);
            //    Vector3 worldMove = target.TransformDirection(localMove);

            //    // 计算预期新位置
            //    Vector3 newPosition = target.position + worldMove;

            //    // 限制中心点不低于最低点
            //    if (newPosition.y < LowestT.position.y)
            //    {
            //        newPosition.y = LowestT.position.y;
            //        // 重新计算修正后的世界空间移动量
            //        worldMove = newPosition - target.position;
            //    }

            //    target.position += worldMove;
            //}


            if (isS && Input.GetMouseButton(2))
            {             
                //grab the rotation of the camera so we can move in a psuedo local XY space
                target.rotation = transform.rotation;
                target.Translate(Vector3.right * -Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime);
                target.Translate(transform.up * -Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime, Space.World);
                // 限制中心点不低于最低点
                if (target.position.y < LowestT.position.y)
                {
                    target.position = new Vector3(target.position.x, LowestT.position.y, target.position.z);
                }
            }


            //    if (isS && Input.GetMouseButton(2))
            //{
            //    float mouseX = Input.GetAxis("Mouse X") * moveSpeed * Time.deltaTime;
            //    float mouseY = Input.GetAxis("Mouse Y") * moveSpeed * Time.deltaTime;

            //    // ✅ 正确映射：鼠标左右 = 世界X轴，鼠标上下 = 世界Z轴（负号让上移=后退）
            //    Vector3 worldMove = new Vector3(0, -mouseY, mouseX);

            //    // ✅ 贴地限制：防止穿入地面
            //    Vector3 projectedPos = target.position + worldMove;
            //    if (LowestT != null && projectedPos.y < LowestT.position.y)
            //    {
            //        projectedPos.y = LowestT.position.y;
            //        worldMove = projectedPos - target.position;
            //    }

            //    // ✅ 强制使用世界坐标系，不依赖任何旋转
            //    target.Translate(worldMove, Space.World);
            //}


            if (Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }
            distance -= Input.GetAxis("Mouse ScrollWheel") * mSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            Quaternion rotation = Quaternion.Euler(y, x, 0.0f);  //
            Vector3 disVector = new Vector3(0f, 0f, -distance);
            Vector3 position = rotation * disVector + target.position;

            if (needDamping)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * damping);
                transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * damping);
            }
            else
            {
                transform.rotation = rotation;
                transform.position = position;
            }

        }
    }
    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void FlyAni(float dis,float time)
    {
        DOTween.To(() => distance, x => distance = x, dis, time);
    }

}
