using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyTrigger : MonoBehaviourPunCallbacks
{
    public GameManagerScript gameManagerScript;
    public Image loadingBar;

    [SerializeField]
    float startCount;

    void Start()
    {
        startCount = 0;
    }
    //���������� �ε��� ��Ȳ ������Ʈ
    private void Update()
    {
        //���� ��� �÷��̾ READY ZONE�� �����ִٸ� Start���� �ٰ� ä����
        if (gameManagerScript.readyPlayersCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            startCount += Time.deltaTime;
        }
        else
        {
            startCount = 0;
        }
        loadingBar.fillAmount = startCount / 3;

        if(startCount / 3 >= 1)
        {
            LoadArena();
            startCount = 0;
        }
    }

    //READY ZONE�� �÷��̾���� üũ
    void CheckPlayers()
    {
        Collider[] cols = Physics.OverlapBox(transform.position, transform.localScale / 2);
        int cnt = 0;
        foreach (Collider col in cols)
        {
            if(col.CompareTag("Player"))
            {
                cnt++;
            }
        }
        gameManagerScript.readyPlayersCount = cnt;
    }

    //���� ���� �� ��� Ŭ���̾�Ʈ �� ����
    void LoadArena()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject p in players)
            {
                if (p.GetComponent<PhotonView>().IsMine == true)
                {
                    PhotonNetwork.Destroy(gameObject);
                }
            }

            SceneChanger.Instance.MoveToInGameScene();
        }
    }

    //�÷��̾ ����/���� �� bool�� ����
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerManager>().isRoomReady = true;
            CheckPlayers();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerManager>().isRoomReady = false;
            startCount = 0;
            CheckPlayers();
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CheckPlayers();
    }
}
