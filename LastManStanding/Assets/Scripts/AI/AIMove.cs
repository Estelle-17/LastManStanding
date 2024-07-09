using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour
{
    public enum AIState
    {
        IDLE,   //이동이 없는 기본 상태
        MOVE,    //타겟 위치로 이동 상태
        DEAD   //죽은 상태
    }
    [SerializeField]
    AIState currentState;

    NavMeshAgent agent;
    Animator animator;

    public Vector3 randomPos;

    [SerializeField]
    float remainTime = 0;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        remainTime = 0;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //정해진 시간 동안 주어진 State실행
            remainTime -= Time.deltaTime;

            //지정된 위치까지 이동
            if (currentState == AIState.MOVE)
            {
                animator.SetBool("IsMove", true);
                //SetDestination이 완료된 후 이동하여 지정된 위치까지 도착하면 멈춤
                if (!agent.pathPending && agent.remainingDistance <= 0.01f)
                {
                    currentState = AIState.IDLE;
                }
            }

            //현재 State의 시간이 지나고 이동이 끝나면 새롭게 State를 설정
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
        //AI캐릭터로부터 일정 범위의 랜덤한 위치를 생성
        Vector3 randomPosition = Random.insideUnitSphere * 30f;
        randomPosition.y = 0;
        randomPosition += transform.position;

        //랜덤 위치가 NavMesh위에 있는지 확인
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
}
