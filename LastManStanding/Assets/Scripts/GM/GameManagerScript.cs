using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviourPunCallbacks
{
    public InputController inputControl;

    public GameObject playerPrefab;
    public GameObject cameraArmPrefab;
    public Image blackBackground;

    public TextMeshProUGUI ReadyPlayersText;

    [SerializeField]
    GameObject ExitUI;

    public GameObject eventSystem;
    public GameObject uiEventSystem;

    public int readyPlayersCount = 0;

    private void Start()
    {
        //씬에 맞게 마우스 커서의 lockState와 visible설정
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        //인게임 시작 전 게임 로딩을 위해 1초정도 FadeOut진행
        if (SceneManager.GetActiveScene().name == "InGame")
        {
            StartCoroutine(FadeOut());
        }

        inputControl = GetComponent<InputController>();
        if (inputControl != null)
        {
            inputControl.playerInputControl.PlayerAction.Escape.started += VisibleExitUI;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("playerPrefab이 설정되지 않았습니다.");
        }
        else
        {
            if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null)
            {
                //카메라 암과 플레이어 생성
                Debug.Log("start에서 로컬플레이어를 생성합니다.");
                GameObject cameraArm = Instantiate(cameraArmPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

                GameObject player;
                //인게임에서는 플레이어를 랜덤한 위치에 생성시켜줌
                if (SceneManager.GetActiveScene().name == "InGame")
                {
                    player = PhotonNetwork.Instantiate(this.playerPrefab.name, SetRandomPlayerPositionOnNavMesh(), Quaternion.identity, 0);
                    //플레이어는 FadeOut이 끝나기 전까지 움직일 수 없음
                    player.GetComponent<PlayerAnimatorManager>().isMoveEnable = false;
                }
                else
                {
                    player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
                cameraArm.GetComponent<PlayerCameraWork>().targetObject = player;
            }
            else
            {
                Debug.Log("이미 로컬플레이어가 생성되어있어 새로 생성하지 않습니다.");
            }
        }
    }

    private void Update()
    {
        //방에 입장했을때만 갱신
        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        CheckReadyState();
    }

    //마스터 클라이언트의 맵 동기화
    void LoadArena()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("마스터 클라이언트가 아니면 LoadLevel을 진행할 수 없습니다.");
            return;
        }
        Debug.LogFormat("WaitingRoom 1을 로딩합니다.");
        PhotonNetwork.LoadLevel("WaitingRoom 1");
    }

    //현재 플레이어들의 준비 상태 확인
    void CheckReadyState()
    {
        if (ReadyPlayersText != null)
        {
            ReadyPlayersText.text = readyPlayersCount + "/" + PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }
    Vector3 SetRandomPlayerPositionOnNavMesh()
    {
        //AI캐릭터로부터 일정 범위의 랜덤한 위치를 생성합니다.
        Vector3 randomPosition = Random.insideUnitSphere * 100f;
        randomPosition.y = 1;
        //랜덤 위치가 NavMesh위에 있는지 확인해줍니다.
        NavMeshHit hit;

        while (!NavMesh.SamplePosition(randomPosition, out hit, 100f, NavMesh.AllAreas)) { }

        return hit.position;
    }

    //방에 입장하면 호출
    public override void OnJoinedRoom()
    {
        Debug.Log("Enter Room");
        Debug.LogFormat("플레이어의 닉네임은 {0} 입니다.", PhotonNetwork.NickName);
        Debug.LogFormat("현재 방에 있는 플레이어는 총 {0}명 입니다.", PhotonNetwork.CurrentRoom.PlayerCount);

        SceneChanger.Instance.MoveToWaitingRoomScene();

        //입장한 플레이어를 생성해줍니다.
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && PlayerManager.LocalPlayerInstance == null)
        {
            //카메라 암과 플레이어 생성
            Debug.Log("joinedRoom에서 로컬플레이어를 생성합니다.");
            GameObject cameraArm = Instantiate(cameraArmPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

            GameObject player;
            //인게임에서는 플레이어를 랜덤한 위치에 생성시켜줌
            if (SceneManager.GetActiveScene().name == "InGame")
            {
                player = PhotonNetwork.Instantiate(this.playerPrefab.name, SetRandomPlayerPositionOnNavMesh(), Quaternion.identity, 0);
            }
            else
            {
                player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            cameraArm.GetComponent<PlayerCameraWork>().targetObject = player;
        }
        else
        {
            Debug.Log("이미 로컬플레이어가 생성되어있어 새로 생성하지 않습니다.");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("{0}님이 참가하셨습니다.", newPlayer.NickName);
        Debug.LogFormat("현재 방에 있는 플레이어는 총 {0}명 입니다.", PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("{0}님이 퇴장하셨습니다.", otherPlayer.NickName);
        Debug.LogFormat("현재 방에 있는 플레이어는 총 {0}명 입니다.", PhotonNetwork.CurrentRoom.PlayerCount);
    }

    public void VisibleExitUI(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ExitUI.SetActive(true);
            eventSystem.SetActive(false);
            uiEventSystem.SetActive(true);
        }
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSecondsRealtime(1);
        Color fadecolor = blackBackground.color;

        while (fadecolor.a > 0f)
        {
            fadecolor.a -= Time.deltaTime;
            blackBackground.color = fadecolor;
            yield return null;
        }

        //FadeOut 끝난 후 플레이어 움직임 활성화
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            p.gameObject.GetComponent<PlayerAnimatorManager>().isMoveEnable = true;
        }
    }
}
