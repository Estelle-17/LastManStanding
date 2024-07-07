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
    }

    //READY ZONE�� �÷��̾���� üũ
    void CheckPlayers()
    {
        Collider[] cols = Physics.OverlapBox(transform.position, transform.localScale / 2);
        int cnt = 0;
        foreach (Collider col in cols)
        {
            Debug.Log(col.name);
            if(col.CompareTag("Player"))
            {
                cnt++;
            }
        }
        gameManagerScript.readyPlayersCount = cnt;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
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
