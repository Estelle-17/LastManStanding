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
            Debug.Log("���� ī�޶� �߰ߵ��� �ʾҽ��ϴ�.");
        }

        inputControl = GetComponent<InputController>();
        if(inputControl != null)
        {
#if UNITY_EDITOR //����Ƽ �����ͷ� ���� ��
            inputControl.playerInputControl.PlayerAction.Mouse.started += OnMouseMove;
#elif UNITY_ANDROID //�ȵ���̵忡�� ���� ��
            inputControl.playerInputControl.PlayerAction.Mouse.started += OnMouseMove;
#endif
        }
        else
        {
            Debug.Log("InputController�� �߰ߵ��� �ʾҽ��ϴ�.");
        }

        xRotate = 0;
    }
    void Update()
    {
        //����ٴϴ� ����� ������ ��� ����ٴ�
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
        //�������� ��� ���Ͱ�
        Vector3 ray_target = transform.up * height + transform.forward * distance;

        RaycastHit hit;
        Physics.Raycast(transform.position, ray_target, out hit, camera_dist, layerMask);

        Vector3 hitPosition;
        
        if (hit.point != Vector3.zero)
        {
            //Raycast�� ���� ��ġ�� �ű��
            hitPosition = hit.point - (targetObject.transform.position - transform.position).normalized;
        }
        else
        {
            //ī�޶���ġ������ ���⺤�� * ī�޶� �ִ�Ÿ��� �ű��
            hitPosition = transform.position + ray_target.normalized * camera_dist;
        }
        mainCamera.transform.position = new Vector3(Mathf.Lerp(mainCamera.transform.position.x, hitPosition.x, smoothSpeed * Time.deltaTime),
                                                    Mathf.Lerp(mainCamera.transform.position.y, hitPosition.y, smoothSpeed * Time.deltaTime),
                                                    Mathf.Lerp(mainCamera.transform.position.z, hitPosition.z, smoothSpeed * Time.deltaTime));
    }

    //���콺�� ���� �÷��̾��� ī�޶� ȸ��
    public void OnMouseMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        float angleX = input.x * Time.deltaTime * verticalLookSensitivity;
        float angleY = -input.y * Time.deltaTime * horizontalLookSensitivity;

        //x���� eulerAngle�δ� ���� ������ �ֱ� ��Ʊ� ������ ���� ���
        xRotate = Mathf.Clamp(xRotate + angleY, -45, 50);
        transform.eulerAngles = new Vector3(xRotate, transform.eulerAngles.y + angleX, 0);
    }
}