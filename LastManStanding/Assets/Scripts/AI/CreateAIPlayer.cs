using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreateAIPlayer : MonoBehaviourPunCallbacks
{
    public GameObject aiPlayerPrefab;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 50; i++)
            {
                if (SpawnAIPlayerOnNavMesh())
                {
                    i--;
                }
            }
        }
    }

    bool SpawnAIPlayerOnNavMesh()
    {
        //AI캐릭터로부터 일정 범위의 랜덤한 위치를 생성합니다.
        Vector3 randomPosition = Random.insideUnitSphere * 100f;
        randomPosition.y = 1;
        randomPosition += transform.position;
        //랜덤 위치가 NavMesh위에 있는지 확인해줍니다.
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPosition, out hit, 100f, NavMesh.AllAreas))
        {
            PhotonNetwork.Instantiate(this.aiPlayerPrefab.name, hit.position, Quaternion.identity, 0);
            return false;
        }
        else
        {
            return true;
        }
    }
}
