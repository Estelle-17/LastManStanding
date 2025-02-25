using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class PlayerAnimatorManager : MonoBehaviourPun
{
    public InputController inputControl;
    public Transform attackColliderPosition;

    private Animator animator;

    [SerializeField]
    private GameObject JoystickObject;
    private Joystick JoystickController;
    

    [SerializeField]
    float moveSpeed = 2.5f;
    [SerializeField]
    float runSpeed = 5f;
    [SerializeField]
    float turnSpeed = 5;

    Vector2 input;
    public bool isMoveEnable;
    public bool isCheckAttackCollider;
    [SerializeField]
    public bool isActiveAttack;

    Rigidbody rig;

    Vector3 gizmoPos;

    void Start()
    {
        animator = GetComponent<Animator>();
        if(!animator)
        {
            Debug.LogError("�÷��̾ Animator�� �������� �ʽ��ϴ�.");
        }
        inputControl = GetComponent<InputController>();
        if (inputControl != null)
        {
            inputControl.playerInputControl.PlayerAction.Chat.started += SettingPlayerInput;
        }
//#if UNITY_ANDROID //�ȵ���̵忡�� ���� ��
//            JoystickController = Instantiate<GameObject>(JoystickObject).GetComponent<Joystick>();
//#endif
        //JoystickController = Instantiate<GameObject>(JoystickObject, GameObject.Find("MainCanvas").transform).GetComponent<Joystick>();

        //�ΰ��ӿ����� ���� ���
        if (SceneManager.GetActiveScene().name == "InGame")
        {
            isActiveAttack = true;
        }
        else
        {
            isActiveAttack = false;
        }

        input = Vector2.zero;

        isMoveEnable = true;
        isCheckAttackCollider = false;

        rig = GetComponent<Rigidbody>();
    }

    void Update()
    {
        #region ��� Ŭ���̾�Ʈ�� �÷��̾ ����Ǿ�� �ϴ� �ڵ�

        //���� �������� �ִϸ�����
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //���� �� �ݶ����� üũ
        if (stateInfo.IsName("Base Layer.Attack") && stateInfo.normalizedTime >= 0.55f && !isCheckAttackCollider)
        {
            isCheckAttackCollider = true;
            CheckAttackCollider();
        }

        if (!stateInfo.IsName("Base Layer.Attack"))
        {
            isCheckAttackCollider = false;
        }

        #endregion

        //�ν��Ͻ��� Ŭ���̾�Ʈ���� �� ����ǰ� �ִ��� Ȯ��
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true || !animator)
        {
            return;
        }

        if(!isMoveEnable)
        {
            input = Vector2.zero;
        }

        if (input.magnitude != 0)
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
        else
        {
            isCheckAttackCollider = false;
        }
    }
    //�÷��̾��� �������� FixedUpdate���� ����
    private void FixedUpdate()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true || !animator)
        {
            return;
        }

        //���� �������� �ִϸ�����
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //���� ĳ���Ͱ� ������ �� ���� ��
        if(!isMoveEnable)
        {
            return;
        }
        //�����߿��� �̵� �Ұ���
        if (!stateInfo.IsName("Base Layer.Attack") && !animator.GetBool("IsAttack"))
        {
            MoveAndRotate();
        }
    }

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

            /*if (animator.GetBool("IsRun"))
                rig.position += moveDir * Time.deltaTime * runSpeed;
            else
                rig.position += moveDir * Time.deltaTime * moveSpeed;*/
            if (animator.GetBool("IsRun"))
                transform.position += moveDir * Time.deltaTime * runSpeed;
            else
                transform.position += moveDir * Time.deltaTime * moveSpeed;
        }
    }
    //���� �� ���� �÷��̾� �� �峭�� üũ
    private void CheckAttackCollider()
    {
        gizmoPos = new Vector3(attackColliderPosition.position.x, attackColliderPosition.position.y, attackColliderPosition.position.z);
        Collider[] cols = Physics.OverlapSphere(gizmoPos, 1);
        foreach (Collider col in cols)
        {
            if (col.CompareTag("Player") && isActiveAttack && col.gameObject != gameObject && !col.GetComponent<PlayerManager>().isDead)
            {
                col.GetComponent<PlayerManager>().CharacterDead();
            }
            else if(col.CompareTag("AIPlayer") && isActiveAttack && !col.GetComponent<AIMove>().isDead)
            {
                col.GetComponent<AIMove>().CharacterDead();
            }
            else if(col.CompareTag("Toy"))
            {
                col.GetComponent<Rigidbody>().AddForce(transform.forward / 50, ForceMode.Impulse);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireSphere(gizmoPos, 1);
    }

    #region InputSystem Callback

    public void OnMove(InputAction.CallbackContext context)
    {
        input = context.ReadValue<Vector2>();
    }
    //�޸��� ����
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
    //���� ����
    public void OnAttack(InputAction.CallbackContext context)
    {
        //Ű �Է��� ���۵� ���
        if (context.started)
        {
            if(isMoveEnable)
                animator.SetBool("IsAttack", true);

            isMoveEnable = true;
        }
    }
    //InputSystem�� ChatŰ�� �Է� �� �÷��̾��� �������� ����
    public void SettingPlayerInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (isMoveEnable)
                isMoveEnable = false;
            else
                isMoveEnable = true;
        }
    }

    #endregion
}
