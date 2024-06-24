using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimatorManager : MonoBehaviourPun
{
    #region MonoBehaviour Callbacks

    private Animator animator;

    [SerializeField]
    float moveSpeed = 5;
    [SerializeField]
    float turnSpeed = 25;
    Vector2 input;

    void Start()
    {
        animator = GetComponent<Animator>();
        if(!animator)
        {
            Debug.LogError("플레이어에 Animator가 존재하지 않습니다.");
        }
        input = Vector2.zero;
    }

    void Update()
    {
        //인스턴스가 클라이언트한테 잘 제어되고 있는지 확인
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (!animator)
        {
            return;
        }

        MoveAndRotate();
    }

    #endregion

    private void MoveAndRotate()
    {
        if (input != null)
        {
            //메인 카메라 transform
            Transform camera = Camera.main.transform;
            Vector3 forwardDir = new Vector3(camera.forward.x, 0f, camera.forward.z).normalized;
            Vector3 rightDir = new Vector3(camera.right.x, 0f, camera.right.z).normalized;
            Vector3 moveDir = forwardDir * input.y + rightDir * input.x;

            Quaternion viewRot = Quaternion.LookRotation(moveDir.normalized);

            transform.rotation = Quaternion.Lerp(transform.rotation, viewRot, turnSpeed * Time.deltaTime);

            transform.position += moveDir * Time.deltaTime * moveSpeed;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }
}
