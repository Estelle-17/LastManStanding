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
    //지속적으로 로딩바 상황 업데이트
    private void Update()
    {
        //방의 모든 플레이어가 READY ZONE에 들어와있다면 Start원형 바가 채워짐
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

    //READY ZONE의 플레이어들을 체크
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

    //게임 시작 시 모든 클라이언트 씬 변경
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

    //플레이어가 입장/퇴장 시 bool값 변경
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
