using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

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

    //컬러값을 저장할 리스트 (이미지에서 불러온 컬러값 저장
    private List<Color> _colorList = new List<Color>();
    //백블렁셍 저장된 컬러값을 구별할 목적으로 생성
    private List<Block> _backBlockList = new List<Block>();

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
        PlayGame("Test", "Slime");

        
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

            foreach (var item in _Backboard)
            {
                //백보드에 큐브가 있으면 삭제한다
                if(item!=null)
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
                    obj.tag = "CubeBlock"; // 2023-02-11
                    Block block =obj.GetComponent<Block>();
                    block.OriginColor = color;
                    
                    block.col = i;
                    block.row = j;
                
                    obj.transform.position = new Vector3(j, i, 0.0f);

                    obj.GetComponent<MeshRenderer>().material.color = color;

                    //게임보드에 생성된 큐브의 참조값을 추가한다
                    _board[i, j] = obj;

                    //컬러 리스트를 만든다
                    _colorList.Add(color);

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

                    //위치의 컬러값을 기록한다
                    Block block = backObj.GetComponent<Block>();
                    block.OriginColor = color;
                    block.col = i;
                    block.row = j;


                    //컴포넌트 삭제
                    Rigidbody rigidbody= backObj.GetComponent<Rigidbody>();
                    Destroy(rigidbody);
                    Collider collider = backObj.GetComponent<Collider>();
                    Destroy(collider);

                    backObj.transform.SetParent(_sBackObject.transform);
                    backObj.transform.position = new Vector3(j,i,0.0f);

                    //블럭컴포넌트 하나하나 리스트에 넣는다(백보드상의 블럭컴포넌트 리스트에 저장
                    _backBlockList.Add(backObj.GetComponent<Block>());

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

        foreach (var item in _board)
        {
            if(item !=null)
            {
                item.GetComponent<Block>().OriginScale = item.transform.localScale;
            }
        }


        Vector3 mainPosition = _sMainObject.transform.position;
        _sMainObject.transform.position = new Vector3(mainPosition.x, mainPosition.y + 2f, 0f);
        Vector3 backPosition = _sBackObject.transform.position;
        _sBackObject.transform.position = new Vector3(backPosition.x, backPosition.y + 2f, 0f);

        //컬러리스트에 저장된 중복 컬러 값을 제거한다
        _colorList = _colorList.Distinct().ToList();


        foreach (var item in _backBlockList)
        {
            int index = _colorList.FindIndex(x => x == item.OriginColor);
            item.BlockNumebr = index;
            item.NumberText = index.ToString();

            _board[item.col, item.row].GetComponent<Block>().NumberText = index.ToString();
            _board[item.col, item.row].GetComponent<Block>().BlockNumebr = index;

            //백보드상의 블럭에 컬러번호 출력
            item.ShowNumberText(true);

        }

        //백보드상의 블럭의 위치값을 오리진 포지션에 저장한다
        foreach (var item in _Backboard)
        {
            if(item != null)
            {
                item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0f);
                item.GetComponent<Block>().OriginPos = item.transform.position;

            }
        }


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

               Invoke("ResetColliderSize", 0.1f);
            }
        }
    }

    private void ResetColliderSize()
    {
        foreach (var obj in _board)
        {
            if (obj != null)
            {
                var colli = obj.GetComponent<BoxCollider>();
                colli.size = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }

    private GameObject _target = null;
    private bool isMouseDrag = false;
    //클릭된 위치 조정값
    private const float CLICKYOFFSET = 1.3f;
    private const float CLICKZOFFSET = -0.5f;
    private Vector3 _ScreenPosition;
    private Vector3 _offset;
    private GameObject ReturnClickObject()
    {

        //스크린좌표계를 월드좌표로 바꾼 다음에 퍼즐의 블럭 사이즈에 포함되는지 아는 방식으로 하기 힘듬
        //레이캐스트로 화면을 클릭해서 광성과 겹쳐지는 면을 가지고 있는 오브젝트를 찾아낸다

        GameObject target = null;
        //화면을 클릭했을떄의 광선을 레이에 저장
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction * 10);
        foreach (var item in hits)
        {
            if(item.collider != null && item.transform.tag == "CubeBlock")
            {
                target = item.collider.gameObject;
                //Destroy(target);
                break;
            }
        }
            return target;
    }
    /// <summary>
    /// 드래그된 블럭의 컬러값에 새당하는 백보드상의 블럭을듸 텍스트칼라를 변경한다
    /// </summary>
    /// <param name="index"></param>
    /// <param name="color"></param>
    private void ChangeBlockColor (int index, Color color)
    {
        foreach (var item in _backBlockList)
        {
            if(item != null)
            {

                //if(index == -1)
                //{
                //    item.GetComponent<Block>().
                //}


                if(item.GetComponent<Block>().BlockNumebr == index)
                {
                    item.GetComponent<Block>().SetNumberTextColor(color);

                }
            }
        }
    }
    /// <summary>
    /// 백보드상의 블럭과 타겟 블럿의 위치값과 컬러값이 일치하는지 비교함
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private GameObject IsMatchPositionColorBlock(GameObject target)
    {
        foreach (var item in _backBlockList)
        {
            if(item.CurrentState == Block.STATE.FIXED &&item.CheckMatchPosition(target) && item.CheckMatchColor(target))
            {
                //위치와 컬러가 일치된 블럭이 들어온다 
                return item.gameObject;
            }
        }
        return null;
    }

    void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            _target = ReturnClickObject();

            if (_target != null)
            {
                //클릭된 큐브블럭이 있으면?
                isMouseDrag = true;

                //큐브를 선택 시 백보드상에 클릭된 큐브의 컬러값과 같은 백보드상의 블럭의 텍스트를 빨간색으로 변경한다

                ChangeBlockColor(_target.GetComponent<Block>().BlockNumebr, Color.red);

                Vector3 clickObjectPosition = _target.transform.position;
                _target.transform.position = new Vector3(clickObjectPosition.x, clickObjectPosition.y, 0f);

                //큐브블럭이 중력의(물리엔진의) 영향을 받지 않도록 한다.
                Destroy(_target.GetComponent<Rigidbody>());

                //선택된 블럭의 회전값을 초기화하여 전면이 보이도록 처리

                _target.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                //선택된 블럭이 클린된 위치보다 위쪽에 표시되도록 처리
                Vector3 pos = _target.transform.position;
                _target.transform.position = new Vector3(pos.x, pos.y + CLICKYOFFSET, pos.z + CLICKZOFFSET);

                //선택된 블럭의 스케일을 조절한다
                _target.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

                //큐브의 스크린상의 좌표값을 구한다
                _ScreenPosition = Camera.main.WorldToScreenPoint(_target.transform.position);

                _offset = _target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _ScreenPosition.z));



            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDrag = false;
            //_target = null;

            if(_target != null)
            {
                ChangeBlockColor(_target.GetComponent<Block>().BlockNumebr, Color.black);
                //마우스 클릭시 제거했던 리지드바디를 추가해서 물리 영향을 준다
                _target.AddComponent<Rigidbody>();
                _target.transform.localScale = _target.GetComponent<Block>().OriginScale;


            }
            _target = null;

        }
        //마우스가 클릭된 상태에서 마우스의 이동 시 처리
        if(isMouseDrag)
        {
            Vector3 currentScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _ScreenPosition.z);
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(currentScreenPos) + _offset;
            if (_target != null)
            {
                _target.transform.position = currentPos;

                //드래그된 블럭이 이동할때마다 일치하는 맥보드상의 블럭이 있는지 체크
                GameObject matchObj = IsMatchPositionColorBlock(_target);

                if(matchObj != null)
                {
                    //일치된 큐브블럭을 백보드상에 위치시킨다
                    _target.transform.position = matchObj.GetComponent<Block>().OriginPos;
                    _target.transform.localScale = _target.GetComponent<Block>().OriginScale;
                    matchObj.SetActive(false);

                    //해당 위치에 매칭 되었다고 처리
                    _target.GetComponent<Block>().CurrentState = Block.STATE.FIXED;
                    matchObj.GetComponent<Block>().CurrentState = Block.STATE.FIXED;

                    //콜라이드 제거
                    Destroy(_target.GetComponent<Collider>());

                    ChangeBlockColor(_target.GetComponent<Block>().BlockNumebr, Color.black);

                    _target.GetComponent<Block>().MatchBlockAnimationStart();
                    _target = null;
                    isMouseDrag = false;
                }
            }
        }



    }
}
