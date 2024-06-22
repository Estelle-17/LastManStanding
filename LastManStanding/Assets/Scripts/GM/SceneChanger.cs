/*
* Writer : ���ؿ�
*
* �� �ҽ��ڵ�� ���� ���� �̵��� �� �ִ� �Լ����� �ڵ��Ǿ� �ֽ��ϴ�.
*
* Last Update : 2024 / 06 / 19
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
   private static SceneChanger inst;

    void Awake()
    {
        if(null == inst)
        {
            inst = this;

            //�� ��ȯ�� �Ǵ��� �ı����� �ʵ��� ��
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            //�̹� ������� �ν��Ͻ��� ����ϱ� ���� ���ο� ���� ���� SceneChanger�� ������ ��� ��������
            Destroy(this.gameObject);
        }
    }
    //�ٸ� Ŭ���������� ���� ȣ���� ������
    public static SceneChanger Instance
    { 
        get 
        {
            if (inst == null)
            {
                return null;
            }

            return inst; 
        } 
    }
    //����ȭ������ �� ��ȯ �Լ�
    public void MoveToLobbyScene() 
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    //�ΰ���ȭ������ �� ��ȯ �Լ�
    public void MoveToRoomScene()
    {
        SceneManager.LoadScene("GameRoom", LoadSceneMode.Single);
    }
    //�ΰ���ȭ������ �� ��ȯ �Լ�
    public void MoveToInGameScene()
    {
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }
}
