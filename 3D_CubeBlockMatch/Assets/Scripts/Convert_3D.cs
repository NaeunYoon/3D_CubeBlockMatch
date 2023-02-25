using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class Convert_3D : MonoBehaviour
{
    //큐브 프리팹
    [SerializeField] private GameObject _sCubePrefab;
    //큐브의 부모 오브젝트 ( 위치를 설정한다)
    [SerializeField] private GameObject _sMainObject;

    //하우징
    //우측 벽
    [SerializeField] private GameObject _sRightFence;
    //좌측 벽
    [SerializeField] private GameObject _sLeftFence;

    [SerializeField] private GameObject _sBackObject;   //백큐브의 부모 오브젝트


    //컬러값을 저장할 리스트 (이미지에서 불러온 컬러값 저장
    private List<Color> _colorList = new List<Color>();
    //백블럭에 저장된 컬러값을 구별할 목적으로 생성
    private List<Block> _backBlockList = new List<Block>();

    void Start()
    {
        //하우징
        //스크린 좌표계의 좌중단 좌표값의 월드좌표의 값을 구한다
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(0f,Screen.height/2f, 10f));
        Debug.Log(p1);

        //좌측벽과 우측벽의 위치를 조정한다
        //현재 오른쪽 팬스의 값을 구해와서 대입해준다
        Vector3 RightFencePosition = _sRightFence.transform.position;
        Vector3 LeftFencePosition = _sLeftFence.transform.position;

        //왼쪽 벽과 오른쪽 벽의 위치값을 화면 사이즈에 맞춰서 위치시킨다 (보정한 값을 대입해준다)
        _sRightFence.transform.position = new Vector3(p1.x, RightFencePosition.y, 0f);
        _sLeftFence.transform.position = new Vector3( -p1.x, LeftFencePosition.y, 0f);
        
        // Create3Cube(,10, 10);
        PlayGame("char", "yellow_bird");
    }

    /// <summary>
    /// 3D CUBE 보드를 생성한다 (보드배치할 배열)
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    //게임보드
    private GameObject[,] _board = null;
    private GameObject[,] _Backboard = null;
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

        _Backboard = null;

        //게임보드 생성한다
        _board = new GameObject[height, width];

        _Backboard= new GameObject[height, width];

        //화면에 실제 출력되는 이미지 사이즈를 계산하기 위한 값을 저장
        //이미지 파일에서 픽셀의 색깔값이 투명인 픽셀을 제외하고 출력되는 이미지의 가로열 갯수와 세로열 갯수를 계산하기 위한 변수
        //세로의 최소값 ( 최상단 컬러를 가지고 있는 큐브의 세로 위치)
        int columMin = height;
        //세로의 최대값
        int columnMax = 0;
        //최좌측값 (가로 최소값)
        int roMin = width;
        //가로 최대값
        int roMax = 0;

        //보드를 까는 작업
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //인자로 전달된 컬러 리스트에서 해당위치의 컬러값을 가져온다
                Color32 color = colors[i * width + j];

                //플레이큐브 보드 생성
                //투명하지 않은 블럭들만 출력한다
                if(color.a != 0f)
                {
                    //동적으로 큐브를 생성한다
                    GameObject obj = Instantiate(_sCubePrefab);
                    //큐브에 이름을 부여한다
                    obj.name = $"Cube[{i},{j}]";
                    //동적으로 만든 큐브에 부모 오브젝트를 설정한다
                    //(큐브_sMainObject 위치를 잡는 부모에 자식으로 설정한다)
                    obj.transform.SetParent(_sMainObject.transform);

                        obj.tag = "CubeBlock"; // 2023-02-11 ( 레이캐스트로 식별하기 위해서 태그 지정한다)
                        Block block =obj.GetComponent<Block>();
                        block.OriginColor = color;
                        block.col = i;
                        block.row = j;

                    //큐브의 위치를 설정한다
                    obj.transform.position = new Vector3(j, i, 0.0f);
                    //리스트에 있는 컬러값을 각 오브젝트의 컬러에 대입해준다 
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

                        //위치의 컬러값을 기록한다 ( 백보드상에도 컬러값을 기록해놓고 블럭을 가져다놨을 때 
                        //블럭을 고정할 수 있다)
                        Block block = backObj.GetComponent<Block>();
                        block.OriginColor = color;
                        block.col = i;
                        block.row = j;


                    //백보드 큐브를 생성 후 사용하지 않는 리지드바디와 콜라이더를 삭제한다
                    //컴포넌트 삭제
                    Rigidbody rigidbody= backObj.GetComponent<Rigidbody>();
                    Destroy(rigidbody);
                    Collider collider = backObj.GetComponent<Collider>();
                    Destroy(collider);

                    //부모 설정하고 위치를 잡는다
                    backObj.transform.SetParent(_sBackObject.transform);
                    backObj.transform.position = new Vector3(j,i,0.0f);

                    //블럭컴포넌트 하나하나 리스트에 넣는다(백보드상의 블럭컴포넌트 리스트에 저장
                    //큐브에 블럭 컴포넌트 있음
                    _backBlockList.Add(backObj.GetComponent<Block>());

                    _Backboard[i,j] = backObj;

                }
            }
        }
        //큐브를 화면 가운데로 보정한다

        float midXvalue = width / 2f;
        float midYValue = height / 2f;

        
        for (int i = 0; i < height; i++)
        {
            
            for (int j = 0; j < width; j++)
            {
                //게임보드에 큐브가 있으면
                if (_board[i,j] != null)
                {
                    //보드의 포지션값을 가져온다
                    Vector3 calPosition = _board[i, j].transform.position;
                    //블럭이 들어있는 위치를 고정해준다 (블럭 사이즈가 1이기 때문에 0.5 씩 보정해준다)
                    _board[i, j].transform.position = 
                        new Vector3(calPosition.x - midXvalue + 0.5f, calPosition.y - midYValue + 0.5f, 0f);

                    //백보드상의 위치를 보드와 동일하게 잡아준다
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
            //유니티 단위체계로 바꾼다
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

        //계산된 스케일 값으로 큐브의 부모 오브젝트를 설정한다.(0,0,0)
        _sMainObject.transform.localScale = new Vector3(Xscale, Xscale, Xscale);
        //백보드 위치 값도 원래 보드의 위치와 동일하게 맞춘다.
        _sBackObject.transform.localScale = new Vector3(Xscale, Xscale, Xscale);

        //원래 블럭의 사이즈를 기록한다
        //게임보드상의 블럭 원래 스케일값을 블럭 컴포넌트에 기록함
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

        //중복되는 컬러를 빼고 유니크한 값만 가진다.
        //컬러리스트에 저장된 중복 컬러 값을 제거한다
        //System.Linq 유징해줘야 함
        //중복되는걸 제거하고 다시 리스트에 연결함
        _colorList = _colorList.Distinct().ToList();

        //백보드의 컬러에 있는 인덱스 값과 블럭의 색이 같은 것을 찾아 리턴한다.
        foreach (var item in _backBlockList)
        {
            int index = _colorList.FindIndex(x => x == item.OriginColor);
            //백보드 블럭 인덱스 값을 가져오고 블럭의 넘버가 인덱스 값이 된다.
            item.BlockNumebr = index;
            //백보드 인덱스 값을 넘버 텍스트에 넣는다
            item.NumberText = index.ToString();
            //메인 보드에 있는 블럭들의 텍스트와 넘버에도 똑같이 넣어준다 
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
    /// 리소스 폴더 이름 , 픽셀 이미지의 이름
    /// </summary>
    /// <param name="categoryName"></param>
    /// <param name="imageName"></param>
    public void PlayGame(string categoryName, string imageName)
    {
        Texture2D texture = null;

        //이미지 경로명을 생성
        string PATH = "StageImages/" + categoryName + "/" + imageName;
        //리소스를 기준으로 해서 안에 있는 개체를 로드한다
        //읽어올 데이터 타입 as로 데이터타입 형을 반환한다 
        texture = Resources.Load(PATH,typeof(Texture2D)) as Texture2D;
        //안에 있는 색깔 정보를 가져와서 큐브를 만든다
        BuildConvert3D(texture);
    }
    //세로
    private int _CurrentColumn = 0;
    //가로
    private int _CurrentRow = 0;
    /// <summary>
    /// 읽어온 이미지 정보를 분석해서 색이 있는 3D 보드를 생성한다
    /// </summary>
    /// <param name="texture"></param>
    private void BuildConvert3D(Texture2D texture)
    {
        //읽어온 텍스쳐를 필터모드로 변경한다
        texture.filterMode = FilterMode.Point;

        var textureFormat = texture.format;
        //텍스쳐 포멧이 우리가 원하는 포멧이 아니면 경고문을 준다
        //레드 블루 그린 알파값 8바이트씩이다.
        if( textureFormat != TextureFormat.RGBA32)
        {
            Debug.Log("32비트 컬러를 사용");
        }
        //읽어들인 이미지마다 사이즈가 다르기 때문에(16x16 또는 32x32일 수 있으니
        //읽어온 이미지의 가로 세로 값을 가져온다

        //세로축 픽셀 갯수
        int height = texture.height;
        //가로축 픽셀 갯수
        int width = texture.width;

        //이미지의 가로 세로 값 기록해서 멤버로 사용한다
        _CurrentColumn = height;
        _CurrentRow = width;

        //텍스쳐 안에서 픽셀 정보를 가져온다(텍스쳐 안에 있는 픽셀 정보 읽음)
        Color32[] colorBuffer = texture.GetPixels32();

        //픽셀정보를 읽어온다
        var TextureColors = GeneratedColors(colorBuffer, height, width);

        Create3Cube(TextureColors, height, width);
    }
    /// <summary>
    /// 배열 값을 리스트로 변환한다
    /// </summary>
    /// <param name="colorBuffer"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    private List<Color32> GeneratedColors(Color32[] colorBuffer, int height, int width)
    {
        //컬러 리스트를 만든다
        List<Color32> vertexColors = new List<Color32>(height * width);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color32 c = colorBuffer[j + i * width];
                //읽어들인 컬러를 리스트에 저장한다
                vertexColors.Add(c);
            }
        }
        //해당 리스트를 리턴한다
        return vertexColors;
    }
    //이미지가 모드 정방향이라고 간주하고 출력 사이즈를 설정
    private float XBaseWidth = 500f;
    private float YBaseHeight = 500f;

    /// <summary>
    /// 블럭 꺠트리는 함수
    /// </summary>
    private void Crash()
    {
        foreach(GameObject obj in _board)
        {
            //게임보드상에 보드가 널이 아니면
            if(obj != null)
            {
                //큐브의 포지션 값을 조정한다
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -0.001f);
                //설정한 체크를 풀어준다
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                //중력 영향을 받게 한다
                obj.GetComponent<Rigidbody>().useGravity = true;

               Invoke("ResetColliderSize", 0.1f);
            }
        }
    }
    /// <summary>
    /// 충돌박스를 돌리지 않으면 부서진 블럭들이 떠있기 때문에
    /// 0.1초 뒤에 충돌박스를 원래대로 돌린다.
    /// </summary>
    private void ResetColliderSize()
    {
        foreach (var obj in _board)
        {
            if (obj != null)
            {
                //박스콜라이더를 블럭에 넣고
                var colli = obj.GetComponent<BoxCollider>();
                //블럭들의 사이즈를 원래대로 돌려놓는다.
                colli.size = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }
    private GameObject ReturnClickObject()
    {

        //스크린좌표계를 월드좌표로 바꾼 다음에 퍼즐의 블럭 사이즈에 포함되는지 아는 방식으로 하기 힘듬
        //레이캐스트로 화면을 클릭해서 광선과 겹쳐지는 면을 가지고 있는 오브젝트를 찾아낸다

        GameObject target = null;
        //화면을 클릭했을떄의 광선을 레이에 저장
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //광선과 겹쳐지는 블럭들을 찾는다 (데이터타입을 배열로 설정한다) 어느 방향으로 얼만큼의 길이를 갈 건지 확인
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction * 10);
        //배열 안에서 하나씩 끄집어 내서
        foreach (var item in hits)
        {
            //콜라이더가 널이 아니고 태그가 큐브 블럭인 첫번째 블럭을 타겟으로 지정한다
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
        foreach (var item in _Backboard)
        {
            if(item != null)
            {

                if (index == -1)
                {
                    item.GetComponent<Block>().SetNumberTextColor(color);
                }
                else if (item.GetComponent<Block>().BlockNumebr == index)
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
            //백블럭리스트에 있는 블럭들의 위치와 색이 일치하는지 판단한다
            if(item.CurrentState == Block.STATE.FIXED &&item.CheckMatchPosition(target) && item.CheckMatchColor(target))
            {
                //위치와 컬러가 일치된 블럭이 들어온다 
                return item.gameObject;
            }
        }
        return null;
    }
    //마우스 클릭 시 선택된 블럭
    private GameObject _target = null;
    //마우스 클릭 후 드래그 상태인지 체크
    private bool isMouseDrag = false;
    //클릭된 위치 조정값
    //클릭된 블럭의 위치를 조정하는 값(y,z)
    private const float CLICKYOFFSET = 1.3f;
    private const float CLICKZOFFSET = -0.5f;
    private Vector3 _ScreenPosition;
    private Vector3 _offset;

    void Update()
    {
        //마우스 우측 버튼 누름 확인
        if (Input.GetMouseButtonDown(0))
        {
            //마우스를 클릭하고 클릭된 큐브블럭을 가져온다
            _target = ReturnClickObject();

            //리턴된게 널이 아니면? => 클릭된 블럭이 있다
            if (_target != null)
            {
                //클릭된 큐브블럭이 있으면? => 마우스 드래그상태 true로 변경해준다
                isMouseDrag = true;

                //큐브를 선택 시 백보드상에 클릭된 큐브의 컬러값과 같은 백보드상의 블럭의 텍스트를 빨간색으로 변경한다

                ChangeBlockColor(_target.GetComponent<Block>().BlockNumebr, Color.red);

                //클릭된 오브젝트들의 위치값을 가져온다
                Vector3 clickObjectPosition = _target.transform.position;
                //클릭된 오브젝트들의 위치를 조정해준다 ( 조금 앞쪽으로 옮김) => 물리영향을 받아서 떨어짐
                _target.transform.position = new Vector3(clickObjectPosition.x, clickObjectPosition.y, 0f);

                //큐브블럭이 중력의(물리엔진의) 영향을 받지 않도록 한다.
                Destroy(_target.GetComponent<Rigidbody>());

                //선택된 블럭의 회전값을 초기화하여 전면이 보이도록 처리 ( 내가 클릭했을 때 정면이 보여야함)
                _target.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                //선택된 블럭이 클린된 위치보다 위쪽에 표시되도록 처리
                //(모바일 같은 경우엔 손가락으로 클릭하기 떄문에 근처만 클릭해도 보이게끔 한다)
                Vector3 pos = _target.transform.position;
                _target.transform.position = new Vector3(pos.x, pos.y + CLICKYOFFSET, pos.z + CLICKZOFFSET);

                //선택된 블럭의 스케일을 조절한다 (클릭된 블럭이 다른 블럭과 차별화되게 크기를 키워준다)
                _target.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

                //월드에 있는 큐브의 스크린상의 좌표값을 구한다
                _ScreenPosition = Camera.main.WorldToScreenPoint(_target.transform.position);
                //실제 마우스 클릭 위치와(스크린) 큐브의 위치 차를 오프셋에 저장하고 그 차이만큼 움직임을 부드럽게 해준다
                _offset = _target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _ScreenPosition.z));

                //마우스가 클릭된 상태에서 움직여야함
            }
        }
        //마우스 버튼을 놨을 때 확인
        if (Input.GetMouseButtonUp(0))
        {
            //마우스를 놓았으면 드래그 상태를 풀어준다 false
            isMouseDrag = false;
            //_target = null;

            if(_target != null)
            {
                //ChangeBlockColor(_target.GetComponent<Block>().BlockNumebr, Color.black);
                ChangeBlockColor(-1, Color.black);
                //마우스 클릭시 제거했던 리지드바디를 추가해서 물리 영향을 준다
                _target.AddComponent<Rigidbody>();
                //확대한 스케일값을 원래 스케일값으로 변경해준다
                _target.transform.localScale = _target.GetComponent<Block>().OriginScale;
            }
            _target = null;

        }
        //마우스가 클릭된 상태에서 마우스의 이동 시 처리
        if(isMouseDrag)
        {
            //스크린상의 위치값을 가져온다
            Vector3 currentScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _ScreenPosition.z);
            //현재 위치 + 오프셋 위치까지 보정 (스크린상의 위치값을 월드값으로 바꾸고 오프셋 위치 보정한다)
            //오프셋의 의미 : 정확히 블럭의 중앙을 클릭하지 않기 때문에 스크린과 화면의 거리를 보정해준다
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(currentScreenPos) + _offset;

            //타겟이 있을때만 적용
            if (_target != null)
            {
                _target.transform.position = currentPos;

                //드래그된 블럭이 이동할때마다 일치하는 백보드상의 블럭이 있는지 체크
                GameObject matchObj = IsMatchPositionColorBlock(_target);
                //있으면 백보드 블럭의 참조값을 리턴한다
                if(matchObj != null)    //일치하는게 있음
                {
                    //일치된 큐브블럭을 백보드상에 위치시킨다 (블럭이 들어간다)
                    _target.transform.position = matchObj.GetComponent<Block>().OriginPos;
                    //확대된 큐브의 크기를 원래 크기로 줄여준다
                    _target.transform.localScale = _target.GetComponent<Block>().OriginScale;
                    //백보드 블럭은 필요 없기 때문에 비활성화 해준다
                    matchObj.SetActive(false);

                    //해당 위치에 매칭 되었다고 처리
                    _target.GetComponent<Block>().CurrentState = Block.STATE.FIXED;
                    matchObj.GetComponent<Block>().CurrentState = Block.STATE.FIXED;

                    //타겟블럭의 콜라이드 제거
                    Destroy(_target.GetComponent<Collider>());
                    //빨간색으로 되었던 타켓의 이름을 검정으로 변경해준다 (컬러값 초기화)
                    ChangeBlockColor(_target.GetComponent<Block>().BlockNumebr, Color.black);

                    _target.GetComponent<Block>().MatchBlockAnimationStart();
                    _target = null;
                    isMouseDrag = false;
                }
            }
        }



    }
}
