using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Convert3D : MonoBehaviour
{
    //큐브 프리팹
    [SerializeField]
    private GameObject _CubePrefab;
    //큐브의 부모 오브젝트
    [SerializeField]
    private GameObject _MainObject;
    //보드 배치 2차원 배열 생성 (게임보드)
    private GameObject[,] _Board = null;
    void Start()
    {
        CreateCube(10,10);
    }

    void Update()
    {
        
    }
    /// <summary>
    /// 3D 큐브 보드를 생성하는 함수
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    private void CreateCube(int height, int width)
    {
        #region 보드 생성 전 큐브 삭제하고 초기화하기
        //해당 보드가 널이 아니면 ( 보드에 큐브가 있음 )
        if (_Board!=null)
        {
            //게임보드 초기화
            foreach (var item in _Board)
            {
                //게임보드에 큐브가 있으면 삭제한다
                if(item!=null)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        //널로 만들어준다
        _Board = null;
        #endregion
        //게임 보드 다시 생성한다
        _Board = new GameObject[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //프리팹을 동적으로 만든다.
                GameObject obj = Instantiate<GameObject>(_CubePrefab);
                //큐브 이름 정해주기
                obj.name = $"Cube{i},{j}";
                //부모 정해주기
                obj.transform.SetParent(_MainObject.transform);
                //위치 잡아주기
                obj.transform.position = new Vector3(j, i, 0f);
            }
        }
    }

}
