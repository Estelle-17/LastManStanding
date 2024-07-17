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
        //���� �°� ���콺 Ŀ���� lockState�� visible����
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

        //�ΰ��� ���� �� ���� �ε��� ���� 1������ FadeOut����
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
            Debug.LogError("playerPrefab�� �������� �ʾҽ��ϴ�.");
        }
        else
        {
            if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null)
            {
                //ī�޶� �ϰ� �÷��̾� ����
                Debug.Log("start���� �����÷��̾ �����մϴ�.");
                GameObject cameraArm = Instantiate(cameraArmPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

                GameObject player;
                //�ΰ��ӿ����� �÷��̾ ������ ��ġ�� ����������
                if (SceneManager.GetActiveScene().name == "InGame")
                {
                    player = PhotonNetwork.Instantiate(this.playerPrefab.name, SetRandomPlayerPositionOnNavMesh(), Quaternion.identity, 0);
                    //�÷��̾�� FadeOut�� ������ ������ ������ �� ����
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
                Debug.Log("�̹� �����÷��̾ �����Ǿ��־� ���� �������� �ʽ��ϴ�.");
            }
        }
    }

    private void Update()
    {
        //�濡 ������������ ����
        if (!PhotonNetwork.InRoom)
        {
            return;
        }

        CheckReadyState();
    }

    //������ Ŭ���̾�Ʈ�� �� ����ȭ
    void LoadArena()
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("������ Ŭ���̾�Ʈ�� �ƴϸ� LoadLevel�� ������ �� �����ϴ�.");
            return;
        }
        Debug.LogFormat("WaitingRoom 1�� �ε��մϴ�.");
        PhotonNetwork.LoadLevel("WaitingRoom 1");
    }

    //���� �÷��̾���� �غ� ���� Ȯ��
    void CheckReadyState()
    {
        if (ReadyPlayersText != null)
        {
            ReadyPlayersText.text = readyPlayersCount + "/" + PhotonNetwork.CurrentRoom.PlayerCount;
        }
    }
    Vector3 SetRandomPlayerPositionOnNavMesh()
    {
        //AIĳ���ͷκ��� ���� ������ ������ ��ġ�� �����մϴ�.
        Vector3 randomPosition = Random.insideUnitSphere * 100f;
        randomPosition.y = 1;
        //���� ��ġ�� NavMesh���� �ִ��� Ȯ�����ݴϴ�.
        NavMeshHit hit;

        while (!NavMesh.SamplePosition(randomPosition, out hit, 100f, NavMesh.AllAreas)) { }

        return hit.position;
    }

    //�濡 �����ϸ� ȣ��
    public override void OnJoinedRoom()
    {
        Debug.Log("Enter Room");
        Debug.LogFormat("�÷��̾��� �г����� {0} �Դϴ�.", PhotonNetwork.NickName);
        Debug.LogFormat("���� �濡 �ִ� �÷��̾�� �� {0}�� �Դϴ�.", PhotonNetwork.CurrentRoom.PlayerCount);

        SceneChanger.Instance.MoveToWaitingRoomScene();

        //������ �÷��̾ �������ݴϴ�.
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && PlayerManager.LocalPlayerInstance == null)
        {
            //ī�޶� �ϰ� �÷��̾� ����
            Debug.Log("joinedRoom���� �����÷��̾ �����մϴ�.");
            GameObject cameraArm = Instantiate(cameraArmPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity);

            GameObject player;
            //�ΰ��ӿ����� �÷��̾ ������ ��ġ�� ����������
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
            Debug.Log("�̹� �����÷��̾ �����Ǿ��־� ���� �������� �ʽ��ϴ�.");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("{0}���� �����ϼ̽��ϴ�.", newPlayer.NickName);
        Debug.LogFormat("���� �濡 �ִ� �÷��̾�� �� {0}�� �Դϴ�.", PhotonNetwork.CurrentRoom.PlayerCount);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("{0}���� �����ϼ̽��ϴ�.", otherPlayer.NickName);
        Debug.LogFormat("���� �濡 �ִ� �÷��̾�� �� {0}�� �Դϴ�.", PhotonNetwork.CurrentRoom.PlayerCount);
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

        //FadeOut ���� �� �÷��̾� ������ Ȱ��ȭ
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            p.gameObject.GetComponent<PlayerAnimatorManager>().isMoveEnable = true;
        }
    }
}
