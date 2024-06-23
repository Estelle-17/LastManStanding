using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    private static GameManagerScript inst;

    int hostOrJoin; //0 : host, 1 : join
    string roomName;
    bool isRoomVisible;

    void Awake()
    {
        if (null == inst)
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

    public static GameManagerScript Instance
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

    public void SaveValue(int i, string r, bool b)
    {
        hostOrJoin = i;
        roomName = r;
        isRoomVisible = b;
    }
}
