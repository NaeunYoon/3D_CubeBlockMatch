using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Convert3D : MonoBehaviour
{
    //ť�� ������
    [SerializeField]
    private GameObject _CubePrefab;
    //ť���� �θ� ������Ʈ
    [SerializeField]
    private GameObject _MainObject;
    //���� ��ġ 2���� �迭 ���� (���Ӻ���)
    private GameObject[,] _Board = null;
    void Start()
    {
        CreateCube(10,10);
    }

    void Update()
    {
        
    }
    /// <summary>
    /// 3D ť�� ���带 �����ϴ� �Լ�
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    private void CreateCube(int height, int width)
    {
        #region ���� ���� �� ť�� �����ϰ� �ʱ�ȭ�ϱ�
        //�ش� ���尡 ���� �ƴϸ� ( ���忡 ť�갡 ���� )
        if (_Board!=null)
        {
            //���Ӻ��� �ʱ�ȭ
            foreach (var item in _Board)
            {
                //���Ӻ��忡 ť�갡 ������ �����Ѵ�
                if(item!=null)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        //�η� ������ش�
        _Board = null;
        #endregion
        //���� ���� �ٽ� �����Ѵ�
        _Board = new GameObject[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //�������� �������� �����.
                GameObject obj = Instantiate<GameObject>(_CubePrefab);
                //ť�� �̸� �����ֱ�
                obj.name = $"Cube{i},{j}";
                //�θ� �����ֱ�
                obj.transform.SetParent(_MainObject.transform);
                //��ġ ����ֱ�
                obj.transform.position = new Vector3(j, i, 0f);
            }
        }
    }

}
