using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Convert_3D : MonoBehaviour
{
    //큐브 프리팹
    [SerializeField] private GameObject _sCubePrefab;
    //큐브의 부모 오브젝트
    [SerializeField] private GameObject _sMainObject;

    //하우징
    [SerializeField] private GameObject _sRightFence;
    [SerializeField] private GameObject _sLeftFence;

    [SerializeField] private GameObject _sBackObject;   //백큐브의 부모 오브젝트

    //게임보드
    private GameObject[,] _board = null;
    private GameObject[,] _Backboard = null;
    void Start()
    {
        //스크린 좌표계의 좌중단 좌표값의 월드좌표의 값을 구한다
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(0f,Screen.height/2f, 10f));
        Debug.Log(p1);

        Vector3 RightFencePosition = _sRightFence.transform.position;
        Vector3 LeftFencePosition = _sLeftFence.transform.position;

        //왼쪽 벽과 오른쪽 벽의 위치값을 화면 사이즈에 맞춰서 위치시킨다
        _sRightFence.transform.position = new Vector3(p1.x, RightFencePosition.y, 0f);
        _sLeftFence.transform.position = new Vector3( -p1.x, LeftFencePosition.y, 0f);

        
        
        // Create3Cube(,10, 10);
        PlayGame("foods", "DragonFruit");

        
    }

    /// <summary>
    /// 3D CUBE 보드를 배치한다
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    private void Create3Cube(List<Color32> colors, int height, int width)
    {
        //게임보드 초기화
        if(_board != null)
        {
            foreach (var item in _board)
            {
                //게임보드에 큐브가 있으면 삭제한다
                if(item != null)
                {
                    Destroy(item);
                }
            }
        }

        _board = null;

        _Backboard= null;

        //게임보드 생성
        _board = new GameObject[height, width];

        _Backboard= new GameObject[height, width];

        //화면에 실제 출력되는 이미지 사이즈를 계산하기 위한 값을 저장
        //이미지 파일에서 픽셀의 색깔값이 투명인 픽셀을 제외하고 출력되는 이미지의 가로열 갯수와 세로열 갯수를 계산하기 위한 변수
        //세로의 최소값 ( 최상단 컬러를 가지고 있는 큐브의 세로 위치)
        int columMin = height;
        //세로의 최대값
        int columnMax = 0;
        int roMin = width;
        int roMax = 0;


        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color32 color = colors[i * width + j];

                //플레이큐브 보드 생성
                //투명하지 않은 블럭들만 출력한다
                if(color.a != 0f)
                {
                    GameObject obj = Instantiate(_sCubePrefab);
                    //큐브에 이름을 부여한다
                    obj.name = $"Cube[{i},{j}]";
                    //동적으로 만든 큐브에 부모 오브젝트를 설정한다
                    obj.transform.SetParent(_sMainObject.transform);

                
                    obj.transform.position = new Vector3(j, i, 0.0f);

                    obj.GetComponent<MeshRenderer>().material.color = color;

                    //게임보드에 생성된 큐브의 참조값을 추가한다
                    _board[i, j] = obj;

                    //가로 세로 블럭의 갯수를 계산
                    if(columMin>i)  //최상단
                    {
                        columMin = i;
                    }

                    if(columnMax <i)    //최하단
                    {
                        columnMax = i;
                    }

                    if(roMin > j)   //최좌단
                    {
                        roMin = j;
                    }

                    if(roMax < j)   //최우단
                    {
                        roMax = j;
                    }

                }
                //백보드 블럭을 생성
                if(color.a != 0f)
                {
                    GameObject backObj = Instantiate(_sCubePrefab);
                    backObj.name = $"CubeBack[{i},{j}]";
                    //컴포넌트 삭제
                    Rigidbody rigidbody= backObj.GetComponent<Rigidbody>();
                    Destroy(rigidbody);
                    Collider collider = backObj.GetComponent<Collider>();
                    Destroy(collider);

                    backObj.transform.SetParent(_sBackObject.transform);
                    backObj.transform.position = new Vector3(j,i,0.0f);
                    _Backboard[i,j] = backObj;

                }
            }
        }


        float midXvalue = width / 2f;
        float midYValue = height / 2f;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (_board[i,j] != null)
                {
                    Vector3 calPosition = _board[i, j].transform.position;

                    _board[i, j].transform.position = 
                        new Vector3(calPosition.x - midXvalue + 0.5f, calPosition.y - midYValue + 0.5f, 0f);

                    _Backboard[i, j].transform.position =
                        new Vector3(calPosition.x - midXvalue + 0.5f, calPosition.y - midXvalue + 0.5f, 0f);

                }
            }
        }

        //화면에 출력 비율을 계산한다
        int calColumnCount = columnMax - columMin + 1;
        int calRowCount = roMax - roMin + 1;

        //스케일값 계산
        float Xscale = 0f;
        //가로가 더 큼
        //세로값보다 가로값이 큰 경우 가로값을 기준으로 비율을 설정
        if(calColumnCount < calRowCount)
        {
            Xscale = XBaseWidth / 100.0f;
            Xscale = Xscale / calRowCount;
        }
        else
        {
            Xscale = YBaseHeight / 100.0f;
            Xscale = Xscale / calColumnCount;
        }

        if(Xscale > 0.22f)
        {
            Xscale = 0.22f;
        }

        //계산된 스케일 값ㅇ,로 큐브의 부모 오브젝트를 설정한다.
        _sMainObject.transform.localScale = new Vector3(Xscale, Xscale, Xscale);
        _sBackObject.transform.localScale = new Vector3(Xscale, Xscale, Xscale);
        Vector3 mainPosition = _sMainObject.transform.position;
        _sMainObject.transform.position = new Vector3(mainPosition.x, mainPosition.y + 2f, 0f);
        Vector3 backPosition = _sBackObject.transform.position;
        _sBackObject.transform.position = new Vector3(backPosition.x, backPosition.y + 2f, 0f);

        Invoke("Crash", 1f);

    }
    /// <summary>
    /// 이미지 파일을 리소스 폴더에서 읽어온다
    /// </summary>
    /// <param name="categoryName"></param>
    /// <param name="imageName"></param>
    public void PlayGame(string categoryName, string imageName)
    {
        Texture2D texture = null;

        //이미지 경로명을 생성
        string PATH = "StageImages/" + categoryName + "/" + imageName;

        texture = Resources.Load(PATH,typeof(Texture2D)) as Texture2D;

        BuildConvert3D(texture);
    }


    private int _CurrentColumn = 0;
    private int _CurrentRow = 0;
    /// <summary>
    /// 읽어온 이미지 정보를 분석해서 3D 보드를 생성한다
    /// </summary>
    /// <param name="texture"></param>
    private void BuildConvert3D(Texture2D texture)
    {
        texture.filterMode = FilterMode.Point;

        var textureFormat = texture.format;

        if( textureFormat != TextureFormat.RGBA32)
        {
            Debug.Log("32비트 컬러를 사용");
        }

        //세로축 픽셀 갯수
        int height = texture.height;
        //가로축 픽셀 갯수
        int width = texture.width;

        //이미지의 가로 세로 값 기록
        _CurrentColumn = height;
        _CurrentRow = width;

        Color32[] colorBuffer = texture.GetPixels32();

        //픽셀정보를 읽어온다
        var TextureColors = GeneratedColors(colorBuffer, height, width);

        Create3Cube(TextureColors, height, width);
    }

    private List<Color32> GeneratedColors(Color32[] colorBuffer, int height, int width)
    {
        List<Color32> vertexColors = new List<Color32>(height * width);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color32 c = colorBuffer[j + i * width];

                vertexColors.Add(c);
            }
        }

        return vertexColors;
    }
    //이미지가 모드 정방향이라고 간주하고 출력 사이즈를 설정
    private float XBaseWidth = 500;
    private float YBaseHeight = 500;

    private void Crash()
    {
        foreach(GameObject obj in _board)
        {
            if(obj != null)
            {
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -0.001f);
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                obj.GetComponent<Rigidbody>().useGravity = true;
                
            }
        }
    }


    void Update()
    {
        
    }
}
