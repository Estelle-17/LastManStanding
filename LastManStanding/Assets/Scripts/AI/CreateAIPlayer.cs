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
        //AIĳ���ͷκ��� ���� ������ ������ ��ġ�� �����մϴ�.
        Vector3 randomPosition = Random.insideUnitSphere * 100f;
        randomPosition.y = 1;
        randomPosition += transform.position;
        //���� ��ġ�� NavMesh���� �ִ��� Ȯ�����ݴϴ�.
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
