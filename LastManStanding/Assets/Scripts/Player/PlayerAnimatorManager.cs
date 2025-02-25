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
            Debug.LogError("플레이어에 Animator가 존재하지 않습니다.");
        }
        inputControl = GetComponent<InputController>();
        if (inputControl != null)
        {
            inputControl.playerInputControl.PlayerAction.Chat.started += SettingPlayerInput;
        }
//#if UNITY_ANDROID //안드로이드에서 실행 시
//            JoystickController = Instantiate<GameObject>(JoystickObject).GetComponent<Joystick>();
//#endif
        //JoystickController = Instantiate<GameObject>(JoystickObject, GameObject.Find("MainCanvas").transform).GetComponent<Joystick>();

        //인게임에서만 공격 허용
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
        #region 모든 클라이언트의 플레이어가 실행되어야 하는 코드

        //현재 적용중인 애니메이터
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //공격 시 콜라이터 체크
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

        //인스턴스가 클라이언트한테 잘 제어되고 있는지 확인
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

        //공격후 IsAttack의 파라미터를 false로 변경
        if (stateInfo.IsName("Base Layer.Attack"))
        {
            animator.SetBool("IsAttack", false);
        }
        else
        {
            isCheckAttackCollider = false;
        }
    }
    //플레이어의 움직임은 FixedUpdate에서 진행
    private void FixedUpdate()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true || !animator)
        {
            return;
        }

        //현재 적용중인 애니메이터
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        //만약 캐릭터가 움직일 수 없을 때
        if(!isMoveEnable)
        {
            return;
        }
        //공격중에는 이동 불가능
        if (!stateInfo.IsName("Base Layer.Attack") && !animator.GetBool("IsAttack"))
        {
            MoveAndRotate();
        }
    }

    private void MoveAndRotate()
    {
        if (input != null && input.magnitude != 0)
        {
            //메인 카메라 transform
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
    //공격 시 맞은 플레이어 및 장난감 체크
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
    //달리기 설정
    public void OnRun(InputAction.CallbackContext context)
    {
        //키 입력이 시작된 경우
        if (context.started)
        {
            animator.SetBool("IsRun", true);
        }

        //키 입력이 끝날 경우
        if(context.canceled)
        {
            animator.SetBool("IsRun", false);
        }
    }
    //공격 설정
    public void OnAttack(InputAction.CallbackContext context)
    {
        //키 입력이 시작된 경우
        if (context.started)
        {
            if(isMoveEnable)
                animator.SetBool("IsAttack", true);

            isMoveEnable = true;
        }
    }
    //InputSystem의 Chat키를 입력 시 플레이어의 움직임을 멈춤
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
