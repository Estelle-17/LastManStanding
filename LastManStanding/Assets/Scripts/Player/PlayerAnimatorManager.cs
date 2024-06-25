using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerAnimatorManager : MonoBehaviourPun
{
    #region MonoBehaviour Callbacks

    private Animator animator;

    [SerializeField]
    float moveSpeed = 2.5f;
    [SerializeField]
    float runSpeed = 5f;
    [SerializeField]
    float turnSpeed = 25;

    Vector2 input;

    void Start()
    {
        animator = GetComponent<Animator>();
        if(!animator)
        {
            Debug.LogError("�÷��̾ Animator�� �������� �ʽ��ϴ�.");
        }
        input = Vector2.zero;
    }

    void Update()
    {
        //�ν��Ͻ��� Ŭ���̾�Ʈ���� �� ����ǰ� �ִ��� Ȯ��
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (!animator)
        {
            return;
        }

        //���� �������� �ִϸ�����
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(input.magnitude != 0)
        {
            animator.SetBool("IsMove", true);
        }
        else
        {
            animator.SetBool("IsMove", false);
        }

        //������ IsAttack�� �Ķ���͸� false�� ����
        if (stateInfo.IsName("Base Layer.Attack"))
        {
            animator.SetBool("IsAttack", false);
        }
    }
    //�÷��̾��� �������� FixedUpdate���� ����
    private void FixedUpdate()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (!animator)
        {
            return;
        }

        //���� �������� �ִϸ�����
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //���� �� ��� ������ �̵�
        if (animator.GetBool("IsAttack"))
        {
            AttackMove();
        }

        //�����߿��� �̵� �Ұ���
        if (!stateInfo.IsName("Base Layer.Attack") && !animator.GetBool("IsAttack"))
        {
            MoveAndRotate();
        }
    }

    #endregion

    private void MoveAndRotate()
    {
        if (input != null && input.magnitude != 0)
        {
            //���� ī�޶� transform
            Transform camera = Camera.main.transform;
            Vector3 forwardDir = new Vector3(camera.forward.x, 0f, camera.forward.z).normalized;
            Vector3 rightDir = new Vector3(camera.right.x, 0f, camera.right.z).normalized;
            Vector3 moveDir = forwardDir * input.y + rightDir * input.x;

            Quaternion viewRot = Quaternion.LookRotation(moveDir.normalized);

            transform.rotation = Quaternion.Lerp(transform.rotation, viewRot, turnSpeed * Time.deltaTime);

            if(animator.GetBool("IsRun"))
                transform.position += moveDir * Time.deltaTime * runSpeed;
            else
                transform.position += moveDir * Time.deltaTime * moveSpeed;
        }
    }
    private void AttackMove()
    {
        transform.position += transform.forward * Time.deltaTime * runSpeed;
    }
    #region InputSystem Callback
    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        //Ű �Է��� ���۵� ���
        if (context.started)
        {
            animator.SetBool("IsRun", true);
        }

        //Ű �Է��� ���� ���
        if(context.canceled)
        {
            animator.SetBool("IsRun", false);
        }
    }
    #endregion

    public void OnAttack(InputAction.CallbackContext context)
    {
        //Ű �Է��� ���۵� ���
        if (context.started)
        {
            animator.SetBool("IsAttack", true);
        }
    }
}
