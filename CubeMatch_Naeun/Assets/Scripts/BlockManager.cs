using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    //블럭에 부여된 번호를 표시
    [SerializeField]
    private TextMesh _NumberText;

    //테두리 표시
    [SerializeField]
    private GameObject _Edge;

    //파티클 표시
    [SerializeField]
    public ParticleSystem _Particle;


    //꺠진 블럭들이 백보드 상의 위치값을 기억해서 찾아가야 함
    //세로열 위치
    public int col { set; get; }
    //가로열 위치
    public int row { set; get; }    

    //블럭 생성시 위치를 저장
    public Vector3 OriginPos { set; get; }

    //블럭 생성시 스케일를 저장
    public Vector3 OriginScale { set; get; }

    //블럭 생성시 색을 저장
    public Color OriginColor { set; get; }

    //블럭 생성시 넘버를 저장
    public int OriginNumber { set; get; }

    public string Numebr
    {
        set
        {
            OriginNumber = int.Parse(value);
            _NumberText.text = value;
        }
        get
        {
            return _NumberText.text;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    /// <summary>
    /// 블럭에 부여된 인덱스 값을 끄거나 키는 함수 (출력여부 확인)
    /// </summary>
    public void ShowOnOffNumebrText(bool onOff)
    {
        _NumberText.gameObject.SetActive(onOff);
        _Edge.SetActive(onOff);
    }
    /// <summary>
    /// 텍스트 컬러를 지정해주는 함수 (변경)
    /// </summary>
    /// <param name="color"></param>
    public void SetNumberTextColor(Color color)
    {
        _NumberText.color = color;
    }


    //감도(크게줄수록 쉽게 달라붙음)
    private const float CHECK_POS_RANGE = 0.2f;


    /// <summary>
    /// 인자로 들어온 블럭과 위치가 같은지 체크하는 함수
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CheckMathchPos(GameObject target)
    {
        //블럭의 원래 오리진 포스값은 블럭의 중간값이다.
        //01.만큼 뺴고 더한다는 것은 중점 좌표값이 작은 사각형 안에 포함되는지 확인한다
        //감도만큼 더하면 되기 때문에 감도조절을 해야한다 ( 백블럭이 큰 네모, 오리진 블럭이 매치할 블럭)
        //타겟의 위치로 확인한다
        if((OriginPos.x - CHECK_POS_RANGE) < target.transform.position.x
            && OriginPos.x + CHECK_POS_RANGE > target.transform.position.x
            && OriginPos.y - CHECK_POS_RANGE < target.transform.position.y
            && OriginPos.y + CHECK_POS_RANGE > target.transform.position.y
            )
        {
            return true;
        }
        return false;

        
    }

    /// <summary>
    /// 인자로 전달된 블럭의 컬러와 백보드상의 컬러값이 같은지 비교
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CheckMatchColor(GameObject target)
    {
        if(OriginColor == target.GetComponent<BlockManager>().OriginColor)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 블럭이 같은 자리에 계속 들어갈 수 있기 때문에 상태처리를 한다
    /// </summary>
    public enum STATE
    {
        STOP,   //정지
        MOVE,   //움직이려고 하는 상태
        MOVING, //움직이는 상태
        FIXED   //백보드 상에 위치해있는 상태
    }
    /// <summary>
    /// 블럭의 현재 상태값을 저장하는 변수를 만든다
    /// </summary>
    public STATE CurrentState
    {set; get;}

    /// <summary>
    /// 린트윈을 이용한 애니메이션 효과
    /// </summary>
    public void MatchAnimationStart()
    {
        LeanTween.scale(this.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.3f).
            setEase(LeanTweenType.easeInQuint). //중간에 값을 어떻게 변화시킬것인지
            setOnComplete(MatchAnimationEnd);   //애니매이션이 끝났을 때 어떻게 할 건지 함수 호출

        //화면 안쪽으로 0.8초동안 한바퀴 돈다
        LeanTween.rotateAround(this.gameObject, Vector3.forward, 360f, 0.8f);
    }

    public void MatchAnimationEnd()
    {
        LeanTween.scale(this.gameObject, Vector3.one, 0.3f) //0.3초동안 원래 사이즈로 줄여준다
            .setEase(LeanTweenType.easeInBounce);
    }

    /// <summary>
    /// 블럭의 매치체크를 했는지 안했는지 처리하는 프로퍼티
    /// </summary>
    public bool IsMatchedCHeck { set; get; }

    /// <summary>
    /// 백보드 블럭의 위치가 들어옴, 바닥에 있는 블럭들을 백보드로 보내고 비활성화 시켜준다
    /// 다시 호출되지 않게 fixed로 처리해준다
    /// 블럭의 인자로 들어온 백블럭의 위치로 블럭을 날린다.
    /// </summary>
    /// <param name="gameObject"></param>
    public void MoveToFixedPos(GameObject gameObject)
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<BlockManager>().CurrentState = STATE.FIXED;

        //0.8초동안 이 게임 오브젝트가 보드의 위치로 0.8초동안 날아가는 효과 적용
        LeanTween.move(this.gameObject, gameObject.transform, 0.8f).setEase(LeanTweenType.easeInBack)
            .setOnComplete(MoveToFixedPosComplete);
    }
    /// <summary>
    /// 린트윈 효과 끝난 후 적용
    /// </summary>
    public void MoveToFixedPosComplete()
    {
        SoundManager.Instance.Play_BlockClick();
        //날아간 블럭의 회전값을 초기화한다
        this.gameObject.transform.localEulerAngles = Vector3.zero;
        //그리는 순서 조정
        this.GetComponent<MeshRenderer>().material.renderQueue = 2900;
        //원래 스케일 값으로 바꿔준다
        this.gameObject.transform.localScale = OriginScale;
        CurrentState = STATE.FIXED;
        //블럭의 숫자텍스트를 표시하지 않는다
        ShowOnOffNumebrText(false);
        //리지드바디와 콜라이더를 삭제처리
        Destroy(this.gameObject.GetComponent<Rigidbody>());
        Destroy(this.gameObject.GetComponent<Collider>());
    }


    

}
