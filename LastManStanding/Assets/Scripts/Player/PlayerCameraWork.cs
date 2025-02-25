using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerCameraWork : MonoBehaviourPun
{

    [SerializeField]
    private float distance = -7.0f;
    [SerializeField]
    private float height = 3.0f;
    [SerializeField]
    private float smoothSpeed = 10f;
    [SerializeField]
    private float verticalLookSensitivity = 6f;
    [SerializeField]
    private float horizontalLookSensitivity = 3f;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    GameObject mainCamera;

    public GameObject targetObject;
    public InputController inputControl;
    float xRotate;

    void Start()
    {
        mainCamera = Camera.main.gameObject;
        if(mainCamera != null)
        {
            mainCamera.transform.parent = gameObject.transform;
        }
        else
        {
            Debug.Log("메인 카메라가 발견되지 않았습니다.");
        }

        inputControl = GetComponent<InputController>();
        if(inputControl != null)
        {
#if UNITY_EDITOR //유니티 에디터로 실행 시
            inputControl.playerInputControl.PlayerAction.Mouse.started += OnMouseMove;
#elif UNITY_ANDROID //안드로이드에서 실행 시
            inputControl.playerInputControl.PlayerAction.Mouse.started += OnMouseMove;
#endif
        }
        else
        {
            Debug.Log("InputController가 발견되지 않았습니다.");
        }

        xRotate = 0;
    }
    void Update()
    {
        //따라다니는 대상이 존재할 경우 따라다님
        if(targetObject != null)
        {
            transform.position = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y + 1.5f, targetObject.transform.position.z);
        }
        CameraRaycast();
    }
    void LateUpdate()
    {
        mainCamera.transform.LookAt(transform.position);
    }
    private void CameraRaycast()
    {
        float camera_dist = Mathf.Sqrt(distance * distance + height * height);
        //레이저를 쏘는 벡터값
        Vector3 ray_target = transform.up * height + transform.forward * distance;

        RaycastHit hit;
        Physics.Raycast(transform.position, ray_target, out hit, camera_dist, layerMask);

        Vector3 hitPosition;
        
        if (hit.point != Vector3.zero)
        {
            //Raycast가 닿은 위치로 옮긴다
            hitPosition = hit.point - (targetObject.transform.position - transform.position).normalized;
        }
        else
        {
            //카메라위치까지의 방향벡터 * 카메라 최대거리로 옮긴다
            hitPosition = transform.position + ray_target.normalized * camera_dist;
        }
        mainCamera.transform.position = new Vector3(Mathf.Lerp(mainCamera.transform.position.x, hitPosition.x, smoothSpeed * Time.deltaTime),
                                                    Mathf.Lerp(mainCamera.transform.position.y, hitPosition.y, smoothSpeed * Time.deltaTime),
                                                    Mathf.Lerp(mainCamera.transform.position.z, hitPosition.z, smoothSpeed * Time.deltaTime));
    }

    //마우스를 통해 플레이어의 카메라를 회전
    public void OnMouseMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        float angleX = input.x * Time.deltaTime * verticalLookSensitivity;
        float angleY = -input.y * Time.deltaTime * horizontalLookSensitivity;

        //x값은 eulerAngle로는 각도 제한을 주기 어렵기 때문에 따로 계산
        xRotate = Mathf.Clamp(xRotate + angleY, -45, 50);
        transform.eulerAngles = new Vector3(xRotate, transform.eulerAngles.y + angleX, 0);
    }
}