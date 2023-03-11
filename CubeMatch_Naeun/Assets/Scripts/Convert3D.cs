using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using Unity.VisualScripting;
using System;
using UnityEngine.SocialPlatforms;
using Unity.Collections.LowLevel.Unsafe;

public class Convert3D : MonoBehaviour
{
    //큐브 프리팹
    [SerializeField]
    private GameObject _CubePrefab;
    //큐브의 부모 오브젝트
    [SerializeField]
    private GameObject _MainObject;
    //큐브가 맞춰지면 파티클 실핼
    [SerializeField]
    private ParticleSystem _MainParticle;
    //보드 배치 2차원 배열 생성 (게임보드)
    private GameObject[,] _Board = null;


    //화면 사이즈에 맞춰서 좌측벽과 우측벽이 조정되어야 함
    [SerializeField]
    private GameObject _RightFence;
    [SerializeField]
    private GameObject _LeftFence;

    //백큐브의 부모 오브젝트 생성
    [SerializeField]
    private GameObject _Backobj;
    private GameObject[,] _backBoard = null;


    /// </summary>

    void Start()
    {
        //만들어진 리스트에 스테이지 데이터를 담는다
        CSV_FileLoad.OnLoadCSV("StageDatas", stageData);
        //출력해보기
        Debug.Log("StageData.Count " + stageData.Count);


        //스크린의 세로 중간값을 구한다(Screen.height 실제 스크림의 높이 값을 가져와서 2로 나눈다)
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2f, 10f));
        //p1은 중간값이 반환된다
        Vector3 RightFensPos = _RightFence.transform.position;
        Vector3 LeftFencePos = _LeftFence.transform.position;

        //왼쪽 벽과 오른쪽 벽의 위치값을 화면 사이즈에 맞춘다 ( 화면 해상도가 계속 변할 수 있으므로)
        _RightFence.transform.position = new Vector3(p1.x, RightFensPos.y, 0f);
        _LeftFence.transform.position = new Vector3(-p1.x, LeftFencePos.y, 0f);

        //리스트를 임시로 만들어주고



        //PlayGame("Test", "TopSpin");
        PlayNextStage();
    }


    //이미지가 정방형이라고 가정하고 출력 사이즈를 설정한다
    private float XbaseWidth = 500f;
    private float YbaseHeight = 500f;

    /// <summary>
    /// 3D 큐브 보드를 생성하는 함수
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    private void CreateCube(List<Color32> colors, int height, int width)
    {
        #region 보드 생성 전 큐브 삭제하고 초기화하기
        //해당 보드가 널이 아니면 ( 보드에 큐브가 있음 )
        if (_Board != null)
        {
            //게임보드 초기화
            foreach (var item in _Board)
            {
                //게임보드에 큐브가 있으면 삭제한다
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            //백보드 초기화 
            foreach (var item in _backBoard)
            {
                //백보드에 큐브가 있으면 삭제한다
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        //널로 만들어준다
        _Board = null;

        //백보드 추가
        _backBoard = null;
        #endregion

        //게임 보드 다시 생성한다
        _Board = new GameObject[height, width];

        //백보드 생성
        _backBoard = new GameObject[height, width];

        //컬러리스트도 초기화
        _colorList.Clear();

        //백블럭 리스트 초기화
        _backBlockList.Clear();

        //백 오브젝트와 메인오브젝트의 크기와 위치 초기화--------------------------------------
        _MainObject.transform.localScale = Vector3.one;
        _MainObject.transform.position = Vector3.zero;

        _Backobj.transform.position = Vector3.zero;
        _Backobj.transform.localScale = Vector3.one;

        //토글 초기화
        isAllMadeToggle = false;

        //화면에 실제 출력되는 이미지 사이즈를 계산하기 위한 값을 저장하는 변수
        //이미지파일에서 픽셀의 색깔값이 투명인 것을 제외하고 출력되는 이미지의 가로 세로 갯수를 계산하기 위함

        //가장 작은 값을 찾을거라 현재 나올 수 있는 가장 큰 값을 셋팅
        //최 상단 컬러를 가지고 있는 큐브의 세로 위치 ( 세로의 최소값)
        int columnMin = height;
        //작은값으로 초기화 ( 세로의 최대값 )
        int columnMax = 0;
        //최 좌측값을 가지고 와서 큰 값을 설정 (가로의 최소값)
        int rowMin = width;
        //가로의 최대값
        int rowMax = 0;


        //프리팹 생성
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //인자로 받은 컬러ㅣ 리스트에서 해당 위치의 컬러값을 가져온다
                Color32 color = colors[i * width + j];

                //////////////////////////////////////////////////////////////////

                if (color.a != 0f)    //컬러의 알파값이 0이 아닌것들만 생성한다
                {
                    //프리팹을 동적으로 만든다.
                    GameObject obj = Instantiate<GameObject>(_CubePrefab);
                    //큐브 이름 정해주기
                    obj.name = $"Cube{i},{j}";
                    //부모 정해주기
                    obj.transform.SetParent(_MainObject.transform);
                    //태그 지정================================================>
                    obj.tag = "Cube";


                    //블럭 매니저에 가져온 컬러, 컬럼, 로우값을 저장해준다 ================>
                    BlockManager block = obj.GetComponent<BlockManager>();
                    block.OriginColor = color;
                    block.col = i;
                    block.row = j;

                    //위치 잡아주기
                    obj.transform.position = new Vector3(j, i, 0f);

                    obj.GetComponent<MeshRenderer>().material.color = color;

                    //게임보드에 생성된 큐브의 참조값을 추가한다
                    _Board[i, j] = obj;

                    //읽어온 컬러값을 리스트에 저장한다 ==========================>백보드 매칭 시 사용
                    _colorList.Add(color);

                    //컬러값이 있어야 들어온다 (가로/ 세로 블럭의 갯수를 계산)

                    //최상단
                    if (columnMin > i) //i 는 height 값
                    {
                        columnMin = i;
                    }
                    //최하단
                    if (columnMax < i) //i 는 height 값 색깔이 있는 것 중에 큰값
                    {
                        columnMax = i;
                    }
                    //최좌단
                    if (rowMin > j) //i 는 height 값
                    {
                        rowMin = j;
                    }
                    //최우단
                    if (rowMax < j) //i 는 height 값
                    {
                        rowMax = j;
                    }
                }

                //백보드 블럭 생성하는 코드 =================================================================
                if (color.a != 0f)
                {
                    GameObject backObj = Instantiate<GameObject>(_CubePrefab);
                    backObj.name = $"Cube_Back{i}{j}";

                    //백보드도 컬러와 컬럼 로우값을 가지고 있어야 함 ===========================>
                    BlockManager block = backObj.GetComponent<BlockManager>();
                    block.OriginColor = color;
                    block.col = i;
                    block.row = j;

                    //리지드바디와 콜라이더 컴포넌틀 없애야함

                    //먼저 백 오브젝트의 리지드바디 컴포넌트를 가져온 뒤 파괴한다
                    Rigidbody body = backObj.GetComponent<Rigidbody>();
                    Destroy(body);

                    Collider collider = backObj.GetComponent<Collider>();
                    Destroy(collider);

                    backObj.transform.SetParent(_Backobj.gameObject.transform);
                    //?
                    backObj.transform.position = new Vector3(j, i, 0.5f);

                    //백보드상의 블럭들도 블럭 컴포넌트를 가지고 있게 리스트에 저장함 ============>
                    _backBlockList.Add(backObj.GetComponent<BlockManager>());

                    _backBoard[i, j] = backObj;

                }


            }
        }

        //큐브들을 화면 가운데로 보정
        float midXvalue = width / 2;
        float midYvalue = height / 2;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //블럭이 있으면
                //위에서 블럭이 투명이 아닌 경우에만 생성해서 보드에 집어넣었음
                //따라서 보드가 널일 경우가 존재함
                //보드의 최상단, 하단, 좌단, 우단을 구해 셋팅
                if (_Board[i, j] != null)
                {
                    //불럭의 포지션값을 가져옴
                    Vector3 calPos = _Board[i, j].transform.position;
                    //중앙값에 위치하도록 조정
                    _Board[i, j].transform.position = new Vector3(calPos.x - midXvalue + 0.5f, calPos.y - midYvalue + 0.5f, 0f);
                    _backBoard[i, j].transform.position = new Vector3(calPos.x - midXvalue + 0.5f, calPos.y - midYvalue + 0.5f, 0f);
                }
            }
        }

        //화면에 출력 비율을 계산
        //최 좌측에서 최 우측을 빼서 실제 출력되는 비율
        int calColumnCount = columnMax - columnMin + 1;
        //최상단에서 최 하단을 빼서 실제 출력되는 비율
        int calRowCount = rowMax - rowMin + 1;

        float xScale = 0f;
        if (calColumnCount < calRowCount)
        {
            //세로보다 가로가 더 긴 경우에는 가로를 기준으로 잡음
            //유니티 단위체계로 바꾸기 위해 100으로 나눠줌
            xScale = XbaseWidth / 100f;
            xScale = xScale / calRowCount;
        }
        else
        {
            //세로가 더 긴 경우에는 세로를 기준으로 잡는다
            xScale = YbaseHeight / 100f;
            xScale = xScale / calColumnCount;
        }

        //xScale값이 0.22f 보다 크면 스케일 값을 고정시킨다
        if (xScale > 0.22f)
        {
            xScale = 0.22f;
        }

        //메인오브젝트의 스케일을 새로 설정해준다
        _MainObject.transform.localScale = new Vector3(xScale, xScale, xScale);
        _Backobj.transform.localScale = new Vector3(xScale, xScale, xScale);


        //블럭의 원래 스케일값을 블럭컴포넌트의 스케일에 저장======================>
        //게임보드상의 블럭의 원래 스케일값을 가지고 있도록 기록한다.
        foreach (var item in _Board)
        {
            if (item != null)
            {
                item.GetComponent<BlockManager>().OriginScale = item.transform.localScale;
            }
        }


        Vector3 mainPos = _MainObject.transform.position;

        _MainObject.transform.position = new Vector3(mainPos.x, mainPos.y + 2f, -0.01f);
        _Backobj.transform.position = new Vector3(mainPos.x, mainPos.y + 2f, 0f);

        //칼라리스트의 중복되는 컬러를 제거한 뒤 다시 컬러 리스트에 넣는다===========>
        _colorList = _colorList.Distinct().ToList();

        //컬러리스트의 컬러값과 블럭의 컬러값이 같은것을 찾고 인덱스를 찾아서 반환한다.==============>
        //컬러리스트를 리스트 형태로 만든 것
        //블럭의 원래 넘버는 리스트의 인덱스로 구별한다.
        foreach (var item in _backBlockList)
        {
            int index = _colorList.FindIndex(x => x == item.OriginColor);
            item.OriginNumber = index;
            //인덱스 갑 출력
            item.Numebr = index.ToString();

            //백보드상에 번호를 부여한 뒤 보드에도 번호를 부여한다
            _Board[item.col, item.row].GetComponent<BlockManager>().Numebr = index.ToString();
            _Board[item.col, item.row].GetComponent<BlockManager>().OriginNumber = index;

            //텍스트를 띄워준다 (백보드 블럭 + 원래 블럭들은 이미 블럭매니저 컴포넌트를 가지고 있음)
            item.ShowOnOffNumebrText(true);
        }

        //백보드상의 블럭의 위치값을 기록해서 프로퍼티에 저장=======================================>
        foreach (var item in _backBoard)
        {
            if (item != null)
            {
                //현재 백보드상에 있는 블럭의 위치값을 기록 한 뒤에
                item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0f);
                //백보드 블럭의 원래 포지션에 넣어준다.
                item.GetComponent<BlockManager>().OriginPos = item.transform.position;
            }
        }



        Invoke("Crash", 2f);
    }
    /// <summary>
    /// 이미지 파일을 리소스 폴더에서 읽어오는 함수
    /// </summary>
    /// <param name="CategoryName"></param>
    /// <param name="ImageName"></param>
    public void PlayGame(string CategoryName, string ImageName)
    {
        //읽어온 텍스쳐를 저장할 참조형 변수 생성
        Texture2D texture = null;
        //이미지 경로 생성
        string PATH = "StageImages" + "/" + CategoryName + "/" + ImageName;
        //리소스 로드 ( 경로, 읽어올 데이터 타입, 텍스쳐 2d 형태로 형변환해서 텍스쳐에 전달)
        texture = Resources.Load(PATH, typeof(Texture2D)) as Texture2D;
        //색을 입힐 함수 호출
        BuildConvert3D(texture);
    }
    //세로열 갯수
    private int _currentColumn = 0;
    //가로열 갯수
    private int _currentRow = 0;


    /// <summary>
    /// 읽어온 이미지 정보를 분석해서 3d 블럭 게임보드 생성하는 함수
    /// </summary>
    /// <param name="texture"></param>
    private void BuildConvert3D(Texture2D texture)
    {
        //텍스쳐의 필터 모드를 POINT로 설정한다 (오리지널 상태로 보여준다 )
        texture.filterMode = FilterMode.Point;
        //텍스쳐 포맷을 읽어온다
        var textureFormat = texture.format;
        //내가 원하는 포맷이 아니면 경고문 출력
        if (textureFormat != TextureFormat.RGBA32)
        {
            Debug.Log("잘못된 textureFormat 입니다");
        }

        //읽은 이미지의 사이즈가 다를 수 있음. 맞춰서 보드를 설정해야 한다
        int height = texture.height;
        int width = texture.width;

        //받아온 이미지의 사이즈를 멤버변수에 대입해준다 (앞으로 계속 쓸거임)
        _currentColumn = height;
        _currentRow = width;

        //텍스쳐 안에 있는 컬러값을 읽어서 버퍼에 담아준다 
        Color32[] colorBuffer = texture.GetPixels32();

        //픽셀 정보를 생성하는 함수
        var textureColors = GenerateColors(colorBuffer, height, width);

        //읽어들인 컬러를 인자로 전달한다
        CreateCube(textureColors, height, width);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="colorBuffer"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    private List<Color32> GenerateColors(Color32[] colorBuffer, int height, int width)
    {
        //배열에 담긴 컬러값을 리스트로 다시 넣어주는 행위
        //가로 세로만큼 컬러의 리스트를 만든다
        List<Color32> vertexColors = new List<Color32>(height * width);
        //컬러값을 하나씩 읽어서 리스트에 넣는다
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color32 color = colorBuffer[j + i * width];
                //만든 컬러들을 리스트에 추가한다
                vertexColors.Add(color);
            }
        }
        //해당 리스트를 리턴한다
        return vertexColors;
    }

    private void Crash()
    {
        foreach (var item in _Board)
        {
            //아이템이 널이 아니면
            if (item != null)
            {
                //아이템의 위치를 앞쪽으로 배치하고
                item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, -0.1f);
                //블럭에 있는 리지드바디 제약조건을 풀고 중력 영향을 받게 한다.
                item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                item.GetComponent<Rigidbody>().useGravity = true;
            }
        }
        Invoke("ResetColliderSize", 0.1f);
    }
    /// <summary>
    /// 충돌된 후 박스의 콜라이더 사이즈를 다시 1로 만드는 함수
    /// </summary>
    private void ResetColliderSize()
    {
        foreach (var item in _Board)
        {
            if (item != null)
            {
                BoxCollider collider = item.GetComponent<BoxCollider>();

                collider.size = new Vector3(1f, 1f, 1f);

            }
        }

    }
    //이미지에서 불러온 컬러값 저장
    private List<Color> _colorList = new List<Color>();
    //백블럭에 있는 블럭 컴포넌트 저장
    //백블럭에 저장된 컬러값을 구별하는 목적으로 생성한다
    private List<BlockManager> _backBlockList = new List<BlockManager>();


    /// <summary>
    /// 마우스 입력을 클릭하면 클릭된 스크린 좌표에서 발사된 광선과 겹치는
    /// 첫번째 큐브 블럭을 반환하는 함수
    /// </summary>
    /// <returns></returns>
    private GameObject ReturnClickedObject()
    {
        //ratcast로 광선을 발사해서 광선과 겹쳐지는 오브젝트를 찾아낸다
        GameObject target = null;
        //화면을 마우스로 클릭했을때의 직선의 광선을 생성해서 ray에 넣어준다 (광선 생성)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //광선과 겹쳐지는 오브젝트를 찾는다
        //여러개가 겹쳐져있는 오브젝트들이 다 찾아진다

        //광선에 충돌된 애들을 저장하는 데이터타입 배열을 만들고
        //레이의 원점부터 레이의 방향으로 10만큼의 길이와 겹쳐지는 애들을 돌려준다
        RaycastHit[] hitInfo = Physics.RaycastAll(ray.origin, ray.direction * 20);

        foreach (var item in hitInfo)
        {
            //광선에 부딪힌 블럭들중에 콜라이더가 있고 큐브 태그가 있는 블럭들만 
            if (item.collider != null && item.transform.tag == "Cube")
            {
                //처음 매치된 블럭만 찾고 조건문을 빠져나감
                target = item.collider.gameObject;

                //Destroy(target);
                break;
            }
        }
        return target;
    }

    /// <summary>
    /// 드래그된 블럭의 컬러값에 해당하는 백보드상의 블럭 텍스트 컬러 변경
    /// </summary>
    /// <param name="index"></param>
    /// <param name="color"></param>
    private void ChangeBlockTextColor(int index, Color color)
    {
        foreach (var item in _backBoard)
        {
            //백보드 상의 오브젝트가 널이 아니면 (존재하면)
            if (item != null)
            {
                //백보드상의 컬러값이 부여된 번호와 인덱스가 같으면
                if (item.GetComponent<BlockManager>().OriginNumber == index)
                {
                    //백보드상의 블럭들의 컬러값을 변경한다 ( 블럭 매니저에 있는 함수 호출한다)
                    item.GetComponent<BlockManager>().SetNumberTextColor(color);
                }
            }
        }
    }
    /// <summary>
    /// 백보드상의 블럭과 타겟블럭의 위치값과 컬러값이 일치하는지 비교하는 함수
    /// </summary>
    /// <returns></returns>
    public GameObject isMatchPosColorBlock(GameObject target)
    {
        foreach (var item in _backBlockList)
        {
            //백블럭상에는 널이 없음 (위치만 체크) => 그러나 위치만 체크하는건 의미가 없음 (컬러값도체크)
            //백보드상의 블럭이 이미 fixed가 아니어야 함 (fixed가 아닌 블럭만 처리)
            if (item.CurrentState != BlockManager.STATE.FIXED &&
                item.CheckMathchPos(target) &&
                item.CheckMatchColor(target))
            {
                //위치가 일치된 블럭을 리턴한다
                return item.gameObject;
            }
        }
        return null;
    }


    //업데이트문에서 클릭된 블럭을 찾는 멤버필드
    //마우스 클릭시 선택된 오브젝트
    private GameObject target = null;
    //마우스 클릭 후 드래그 상태인지 체크하는 멤버필드
    private bool isMouseDrag = false;

    //클릭된 오브젝트들의 위치를 보정하는 값
    private const float CLICK_Y_OFFSET = 1.3f;
    private const float CLOCK_Z_OFFSET = -0.5f;

    //스크린좌표와 화면간의 차이를 보정하기 위한 멤버필드
    private Vector3 screenPos;
    private Vector3 offset;

    //보드상의 블럭을 모두 맞췄는지 확인하는 필드-------------------------------------
    private bool isAllMadeToggle = false;

    public enum ItemType
    {
        STEP,   //하나씩 처리
        MAGNET, //주변에 있는 것들 처리
        AUTO    //한꺼번에 처리
    }
    //아이템 타입에 따른 플레이
    [SerializeField]
    private ItemType Type = ItemType.STEP;
    public ItemType type
    {
        set { type = value; }
        get { return Type; }
    }

    /// <summary>
    /// 마우스 클릭 이벤트 처리---------------------------------------------
    /// </summary>
    private void MouseEventProcess()
    {
        //어떤 블럭이 클릭되었는지 찾아낸다====================================>
        if (Input.GetMouseButtonDown(0))
        {
            //멤버필드에 반환된 오브젝트를 대입해준다.
            target = ReturnClickedObject();

            //리턴된 오브젝트가 널이 아니면 => 클릭된 큐브 블럭이 있으면
            if (target != null)
            {
                //블럭을 찍기만 하면 백블럭으로 달라붙음
                if (Type == ItemType.MAGNET)
                {
                    if(target.GetComponent<BlockManager>().CurrentState != BlockManager.STATE.FIXED)
                    {
                        AllBlockFixedPos(target.GetComponent<BlockManager>().OriginNumber);
                    }
                    target = null;
                    isMouseDrag= false;
                }
                else if(Type == ItemType.STEP)
                {
                    //마우스 드래그 상태로 설정
                    isMouseDrag = true;
                    //큐브를 선택 시 백보드 상에 해당 클릭된 큐브의 컬러값과 같은
                    //백보드 상의 블럭 텍스트 색을 변경
                    ChangeBlockTextColor(target.GetComponent<BlockManager>().OriginNumber, Color.red);

                    //타겟의 위치값을 가져와서 앞쪽으로 나오게 해준다
                    Vector3 clickedObjectPos = target.transform.position;
                    // 위치값을 바꾸면 물리값이 바뀌어서 떨어지므로 중력을 꺼준다
                    target.transform.position = new Vector3(clickedObjectPos.x, clickedObjectPos.y, -1f);
                    //리지드바디를 파괴한다
                    Destroy(target.GetComponent<Rigidbody>());
                    //선택된 블럭이 앞면을 보게 회전값을 초기화해준다
                    target.transform.localEulerAngles = Vector3.zero;

                    //선택된 블럭이 클릭된 위치보다 위쪽에 표시되도록 처리한다 (선택 시)
                    Vector3 pos = target.transform.position;
                    target.transform.position = new Vector3(pos.x, pos.y + CLICK_Y_OFFSET, pos.z + CLOCK_Z_OFFSET);

                    //선택된 블럭의 스케일을 조정한다 (눌렀을 때 앞으로 나오고 커지게)
                    target.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                    //블럭은 월드에 있고, 큐브의 월드 의 좌표값 스크린 좌표로 바꾸어서 저장
                    screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
                    //실제로 클릭한 곳과 월드공간의 차를 오프셋에 저장
                    offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z));
                }
            }
        }

        //마우스 버튼을 놨을 때
        if (Input.GetMouseButtonUp(0))
        {
            //드래그가 풀린 상태로 설정
            isMouseDrag = false;

            // 버튼을 놨을 때 블럭이 있으면 다시 검정색으로 바꿔준다.
            if (target != null)
            {
                // 겟 컴포넌트로 넘버를 가져오지 않고 그냥 인덱스를 -1로 설정하면
                //인덱스에 해당되는 것이 없기 때문에 모든 블럭이 검정으로 바뀐다.
                ChangeBlockTextColor(target.GetComponent<BlockManager>().OriginNumber, Color.black);

                //블럭을 잡았다가 놨을 경우에 리지드바디를 추가해준다 (잡았을 시 파괴했음)
                target.AddComponent<Rigidbody>();
                target.transform.localScale = target.GetComponent<BlockManager>().OriginScale;
            }

            //타겟을 널로 초기화
            target = null;
        }

        //마우스가 클릭된 상태에서 마우스의 이동 시 처리
        if (isMouseDrag)
        {
            //마우스가 클릭된 위치를 가져오고
            Vector3 currentScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z);
            //마우스가 클릭된 위치를 월드 좌표로 바꿔준 다음, 보정한 오프셋을 적용한다
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(currentScreenPos) + offset;
            /*
             블럭의 정 중앙을 클릭하는것이 아니기 때문에 중앙으로부터 클릭한 위치까지를 보정했음
             */
            if (target != null)
            {
                target.transform.position = currentPos;

                //이동할때마다 백보드 블럭과 일반 블럭의 좌표가 겹쳤는지 확인하는 함수 필요함
                //드래그된 블럭이 이동할때마다 일치하는 백보드상의 블럭이 있는지 체크한다
                //드래그 상태이면 계속 비교
                GameObject matchObj = isMatchPosColorBlock(target);

                if (matchObj != null)
                {
                    //일치된 큐브 블럭을 백보드상에 위치시킨다
                    //타겟비교해서 타켓 블럭을 리턴된 블럭의 위치를 대입시킨다
                    target.transform.position = matchObj.GetComponent<BlockManager>().OriginPos;
                    target.transform.localScale = target.GetComponent<BlockManager>().OriginScale;
                    //매치확인된 블럭은 비활성화 시켜준다
                    matchObj.SetActive(false);

                    ///매치완료
                    ///해당 위치에 매칭 되었음을 처리한다
                    target.GetComponent<BlockManager>().CurrentState = BlockManager.STATE.FIXED;
                    matchObj.GetComponent<BlockManager>().CurrentState = BlockManager.STATE.FIXED;

                    //매치된 오브젝트들의 테두리를 꺼준다 //?
                    matchObj.GetComponent<BlockManager>().ShowOnOffNumebrText(true);

                    //타겟블럭은 이미 매치가 되었으므로 콜라이더를 날려버린다
                    Destroy(target.GetComponent<Collider>());

                    //화면상에 변경되었던 텍스트 컬러값을 초기화한다
                    ChangeBlockTextColor(target.GetComponent<BlockManager>().OriginNumber, Color.black);

                    //매치된 블럭에 린트윈 애니메이션 효과를 준다
                    //target.GetComponent<BlockManager>().MatchAnimationStart();
                    target.GetComponent<BlockManager>()._Particle.Play();
                    //?
                    target.GetComponent<BlockManager>().MatchAnimationStart();

                    

                    //타겟을 널로 바꿔주고 드래그 상태를 false로 바꿔준다
                    target = null;
                    isMouseDrag = false;
                   
                    if(Type == ItemType.AUTO)
                    {
                        RangeMatchItem(_backBoard, _Board, matchObj, _currentRow, _currentColumn);
                    }
                   
                    
                }
            }
        }
    }

    void Update()
    {
        MouseEventProcess();

        if(!isAllMadeToggle&& IsAllMatchBlock())
        {
            //한번만 호출되게 처리
            isAllMadeToggle = true;
            Debug.Log("AllBlockMatched");
            //블럭이 맞춰진 후에 애니메이션 처리
            AllBlockComplete();
        }
    }

    /// <summary>
    /// 보드 상의 모든 블럭이 매치되었는지체크하는 함수
    /// </summary>
    /// <returns></returns>
    private bool IsAllMatchBlock()
    {
        //백보드상에 들어간 블럭들이 매치가 되면 열거형 fixed로 바꿔주므로
        //열거영을 확인한다

        for (int i = 0; i < _currentColumn; i++)
        {
            for (int j = 0; j < _currentRow; j++)
            {
                if (_backBoard[i,j]!=null)
                {
                    if (_Board[i,j].GetComponent<BlockManager>().CurrentState != BlockManager.STATE.FIXED)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    /// <summary>
    /// 모든 블럭이 맞춰졌을 때 후단 처리용 함수
    /// </summary>
    private void AllBlockComplete()
    {
        SoundManager.Instance.Congreturation();
        _MainParticle.Play();
        Invoke("ClearAnimation", 1f);
    }
    /// <summary>
    /// 모든 블럭을 맞춘 후 애니메이션 처리
    /// </summary>
    private void ClearAnimation()
    {
        //블럭 하나의 스케일을 구하기 위해 나눠준다.
        float animationScale = XbaseWidth / _currentRow;
        //유니티 단위 100으로 나눠준다
        animationScale /= 100f;
        //스케일 값을 또 반으로 나눠준다
        animationScale /= 2f;

        Vector3 mainScale = new Vector3(animationScale, animationScale, animationScale);
        //현재 스케일을 메인 스케일로 0.5초동안 바꾼다

        LeanTween.scale(_MainObject, mainScale, 0.5f).
            setEase(LeanTweenType.easeInBack).
            setOnComplete(ClearAnimationComplete);
    }

    private void ClearAnimationComplete()
    {
        _MainParticle.Stop();
        
        _MainObject.GetComponent<Animator>().enabled = true;
        //유니티 애니메이션 실행
        _MainObject.GetComponent<Animator>().SetTrigger("ClearAnim");
    }

    //CSV파일을 쪼개서 데이터를 담는 리스트 선언
    //List<StageData> stageData = new List<StageData>();
    //현재 스테이지를 저장할 멤버변수 (-1로 초기화한다)
    List<StageData> stageData = new List<StageData>();
    private int CurrenStageNum = -1;

    /// <summary>
    /// 다음 스테이지를 실행하게 하는 함수
    public void PlayNextStage()
    {
        CurrenStageNum++;
        PlayGame(stageData[CurrenStageNum].CategoryName, stageData[CurrenStageNum].ImageName);
        //스테이지를 클리어 해야 증가해야한다.
    }

    //주변에 서로 붙어있는 동일한 컬러 블럭 저장용
    private List<GameObject> _matchObject = new List<GameObject>();

    /// <summary>
    /// 매치된 오브젝트들을 한꺼번에 저장
    /// 매치체크 함수 안에서 다시 같은 함수를 호출해서 주변에 같은 블럭이 있는지 호출한다
    /// 주변의 같은 색의 블럭을 _matchObject 리스트에 저장한다
    /// 점수계산 등으로 이용하기 위해 매치오브젝트의 카운트를 반환한다
    /// </summary>
    /// <param name="backBoard"></param>
    /// <param name="board"></param>
    /// <param name="matchObject"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    
    public int RangeMatchItem(GameObject[,] backBoard, GameObject[,] board,
                                GameObject matchObject,int width, int height)
    {
        //매치오브젝트에 블럭이 들어와 있으면
        if(_matchObject.Count > 0)
        {
            //이전에 처리했던 값들을 초기화한다
            _matchObject.Clear();
        }

        foreach (var item in board)
        {
            if(item != null)
            {
                //보드상 블럭의 isMatchedCheck 초기화
                item.GetComponent<BlockManager>().IsMatchedCHeck = false;
            }
        }
        matchObject.GetComponent<BlockManager>().IsMatchedCHeck = true;
        BlockManager matchObjectBlock = matchObject.GetComponent<BlockManager>();

        //백트레킹

        MatchCheck(backBoard, matchObjectBlock.col,matchObjectBlock.row - 1,
            matchObjectBlock.OriginNumber,width,height,DIRECTION.LEFT);
        MatchCheck(backBoard, matchObjectBlock.col, matchObjectBlock.row + 1,
            matchObjectBlock.OriginNumber, width, height, DIRECTION.RIGHT);
        MatchCheck(backBoard, matchObjectBlock.col - 1, matchObjectBlock.row,
            matchObjectBlock.OriginNumber, width, height, DIRECTION.UP);
        MatchCheck(backBoard, matchObjectBlock.col + 1, matchObjectBlock.row,
            matchObjectBlock.OriginNumber, width, height, DIRECTION.DOWN);


        if(_matchObject.Count > 0)
        {
            //같은 블럭이 여전히 있음
            List<GameObject> flyBlocks = new List<GameObject>();
            foreach (var item in board) 
            { 
                if(item != null && item.GetComponent<BlockManager>().CurrentState != BlockManager.STATE.FIXED
                    && item.GetComponent<BlockManager>().OriginNumber == matchObject.GetComponent<BlockManager>().OriginNumber)
                {
                    //처리되지 않고 바닥에 떨어진 블럭들 중에 백보드상의 넘버(컬러)가 동일하다면
                    //블럭을 리스트에 넣어놓는다
                    flyBlocks.Add(item);

                    //매치된 블럭의 숫자와 처리되지 않은 블럭의 갯수가 같은지 확인
                    if(_matchObject.Count == flyBlocks.Count) 
                    {
                        break;
                    }
                }
            }

            int cnt = 0;
            foreach (var item in flyBlocks) 
            {
                //날리는 함수 추가
                Destroy(item.GetComponent<Collider>());
                //매치된 오브젝트가 들어간 배열을 증가시키면서 백블럭에 넣는다.
                item.GetComponent<BlockManager>().MoveToFixedPos(_matchObject[cnt++]);
            }
        }
        return _matchObject.Count;
    }
    
    

    /// <summary>
    /// 블럭의 재귀호출을 위해 이용하는 열거형
    /// </summary>
    public enum DIRECTION
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    /// <summary>
    /// 재귀함수를 통해 상하좌우로 같은 블럭이 있는지 체크하는 재귀함수  
    /// </summary>
    private void MatchCheck(GameObject[,] board, int col, int row, int blockNum, int width, int height, DIRECTION dir)
    {
        //보드상의 범위를 벗어나거나, 같은 색이 아니거나, 이미 fixed 된 블럭이면 리턴한다
        if(col < 0 || //보드상의 범위를 벗어나거나
           col >= height|| 
           row < 0 ||
           row >= width || 
           board[col,row] == null || //블럭이 없거나
           board[col,row].GetComponent<BlockManager>().CurrentState == BlockManager.STATE.FIXED ||  //이미fixed되거나
           board[col,row].GetComponent<BlockManager>().OriginNumber != blockNum || //블럭의 번호가 다르거나
           board[col,row].GetComponent<BlockManager>().IsMatchedCHeck)  //이미 매치된 블럭이거나
                                                                        //처리할 필요가 없음
        {
            return;
        }

        //체크가 되지 않은 블럭이면
        if (!board[col,row].GetComponent<BlockManager>().IsMatchedCHeck)
        {
            //매치 블럭 리스트에 추가한다
            _matchObject.Add(board[col, row]);
            //매치체크 처리
            board[col, row].GetComponent<BlockManager>().IsMatchedCHeck = true;
        }

        //왔던 방향의 반대 방향으로는 보내지 않는다 (주변의 같은 색인 블럭을 찾음)
        if(dir == DIRECTION.UP)
        {
            MatchCheck(board, col - 1, row, blockNum, width, height, DIRECTION.UP);
            MatchCheck(board, col, row + 1, blockNum, width, height, DIRECTION.RIGHT);
            MatchCheck(board, col, row - 1, blockNum, width, height, DIRECTION.LEFT);
        }else if(dir == DIRECTION.DOWN)
        {
            MatchCheck(board, col + 1, row, blockNum, width, height, DIRECTION.DOWN);
            MatchCheck(board, col, row + 1, blockNum, width, height, DIRECTION.RIGHT);
            MatchCheck(board,  col, row - 1, blockNum, width, height, DIRECTION.LEFT);
        }else if(dir == DIRECTION.LEFT)
        {
            MatchCheck(board, col + 1, row, blockNum, width, height, DIRECTION.DOWN);
            MatchCheck(board, col - 1, row, blockNum, width, height, DIRECTION.UP);
            MatchCheck(board, col, row - 1, blockNum, width, height, DIRECTION.LEFT);
        }else if(dir == DIRECTION.RIGHT)
        {
            MatchCheck(board, col + 1, row, blockNum, width, height, DIRECTION.DOWN);
            MatchCheck(board, col - 1, row, blockNum, width, height, DIRECTION.UP);
            MatchCheck(board, col, row + 1, blockNum, width, height, DIRECTION.RIGHT);
        }

        
    }
    /// <summary>
    /// 보드상의 선택된 블럭 컬러(인덱스)와 맞는 블럭을 찾아서 모두 매치
    /// </summary>
    private void AllBlockFixedPos(int index)
    {
        List<GameObject> matchIndexBlockList = new List<GameObject>();
        for (int i = 0; i < _currentColumn; i++)
        {
            for (int j = 0; j < _currentRow; j++)
            {
                if (_backBoard[i,j]!=null)
                {
                    BlockManager block = _backBoard[i,j].GetComponent<BlockManager>();
                    if(block.CurrentState != BlockManager.STATE.FIXED&& block.OriginNumber == index)
                    {
                        matchIndexBlockList.Add(_backBoard[i,j]);
                    }
                }
            }
        }

        int cnt = 0;
        for (int i = 0; i < _currentColumn; i++)
        {
            for (int j = 0; j < _currentRow; j++)
            {
                if (_Board[i,j] !=null)
                {
                    if (_Board[i,j].GetComponent<BlockManager>().CurrentState != BlockManager.STATE.FIXED
                        && _Board[i,j].GetComponent<BlockManager>().OriginNumber ==index)
                    {
                        Destroy(_Board[i, j].GetComponent<Collider>());
                        _Board[i, j].GetComponent<BlockManager>().MoveToFixedPos(matchIndexBlockList[cnt++]);
                    }
                }
            }
        }
    }



}




