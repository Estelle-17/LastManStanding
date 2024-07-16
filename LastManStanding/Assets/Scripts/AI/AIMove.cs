using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour, IPunObservable
{
    public enum AIState
    {
        IDLE,   //�̵��� ���� �⺻ ����
        MOVE,    //Ÿ�� ��ġ�� �̵� ����
        DEAD   //���� ����
    }
    [SerializeField]
    AIState currentState;

    NavMeshAgent agent;
    Animator animator;

    public Vector3 randomPos;

    [SerializeField]
    float remainTime = 0;
    [SerializeField]
    public bool isDead;
    private bool isDeadSettingFinish;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        remainTime = 0;
        isDead = false;
        isDeadSettingFinish = false;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && isDead && !isDeadSettingFinish)
        {
            CharacterDead();
            isDeadSettingFinish = true;
        }
        if (PhotonNetwork.IsMasterClient && !isDead)
        {
            //������ �ð� ���� �־��� State����
            remainTime -= Time.deltaTime;

            //������ ��ġ���� �̵�
            if (currentState == AIState.MOVE)
            {
                animator.SetBool("IsMove", true);
                //SetDestination�� �Ϸ�� �� �̵��Ͽ� ������ ��ġ���� �����ϸ� ����
                if (!agent.pathPending && agent.remainingDistance <= 0.01f)
                {
                    currentState = AIState.IDLE;
                }
            }

            //���� State�� �ð��� ������ �̵��� ������ ���Ӱ� State�� ����
            if (currentState == AIState.IDLE)
            {
                animator.SetBool("IsMove", false);
                if (remainTime <= 0)
                {
                    SetAIState();
                }
            }
        }
    }

    void SetAIState()
    {
        currentState = (AIState)Random.Range(0, 2);
        remainTime = Random.Range(1.0f, 5.0f);

        if (currentState == AIState.MOVE)
        {
            randomPos = GetRandomPositionOnNavMesh();
            agent.SetDestination(randomPos);
        }
    }

    Vector3 GetRandomPositionOnNavMesh()
    {
        //AIĳ���ͷκ��� ���� ������ ������ ��ġ�� ����
        Vector3 randomPosition = Random.insideUnitSphere * 30f;
        randomPosition.y = 0;
        randomPosition += transform.position;

        //���� ��ġ�� NavMesh���� �ִ��� Ȯ��
        NavMeshHit hit;

        for(int i = 0; i < 30; i++)
        {
            NavMesh.SamplePosition(randomPosition, out hit, 16f, NavMesh.AllAreas);

            if (Vector3.Distance(hit.position, randomPos) > 5f)
            {
                return hit.position;
            }
        }

        return transform.position;
    }

    public void CharacterDead()
    {
        agent.isStopped = true;
        animator.SetBool("IsDead", true);
        isDead = true;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<CapsuleCollider>().enabled = false;
    }

    #region IPunObservable Implementation

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //Ŭ���̾�Ʈ���� ���ϴ� �� ����ȭ�� ���⿡ �ڵ� �Է�
            stream.SendNext(isDead);
        }
        else
        {
            this.isDead = (bool)stream.ReceiveNext();
        }
    }

    #endregion
}
