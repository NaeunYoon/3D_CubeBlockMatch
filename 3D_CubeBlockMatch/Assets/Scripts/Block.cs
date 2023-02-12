using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public enum STATE{
        STOP,
        MOVE,
        MOVING,
        FIXED
    }
    //현재 블럭의 상태값 저장
    public STATE CurrentState { set; get; }


    //부여받은 번호를 표시
    [SerializeField] private TextMesh _sNumberText;
    //테두리
    [SerializeField] private GameObject _sEdge;

    
    public int col { set; get; }    //세로열 위치
    public int row { set; get; }    //가로열 위치

    //블럭 생성시 위치를 저장
    public Vector3 OriginPos { set; get; }
    //블럭 생성시 스케일 값 저장
    public Vector3 OriginScale { set; get; }
    //블럭 컬러값을 저장
    public Color OriginColor { set; get; }
    //블럭의 색깔 번호 저장
    public int BlockNumebr { set; get; }
    //블럭 숫자 프로퍼티
    public string NumberText
    {
        set
        {
            BlockNumebr = int.Parse(value);
            _sNumberText.text = value;
        }
        get
        {
            return _sNumberText.text;

        }
    }
    /// <summary>
    /// 큐브에 컬러 인덱스 문자를 출력하는 함수
    /// </summary>
    /// <param name="onoff"></param>
    public void ShowNumberText(bool onoff)
    {
        _sNumberText.gameObject.SetActive(onoff);
        _sEdge.SetActive(onoff);
    }
    /// <summary>
    /// 텍스트의 컬러값을 변경하는 함수
    /// </summary>
    /// <param name="color"></param>
    public void SetNumberTextColor(Color color)
    {
        _sNumberText.color = color;
    }

    private const float CHECKPOSITIONRANGE = -0.1f;

    /// <summary>
    /// 인자로 들어온 블럭과 위치가 같은지 체크한다
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CheckMatchPosition(GameObject target)
    {
        if((OriginPos.x - CHECKPOSITIONRANGE)< target.transform.position.x && 
           (OriginPos.x + CHECKPOSITIONRANGE) > target.transform.position.x &&
            (OriginPos.y - CHECKPOSITIONRANGE) < target.transform.position.y &&
            OriginPos.y + CHECKPOSITIONRANGE > target.transform.position.y) 
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 인자로 전달된 블럭의 컬러와 백보드상의 블럭의 컬러값이 같은지 체크
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CheckMatchColor(GameObject target) 
    { 
        if(OriginColor == target.GetComponent<Block>().OriginColor)
        {
            return true;
        }
        return false;
    }

    public void MatchBlockAnimationStart()
    {
        LeanTween.scale(this.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.3f).
            setEase(LeanTweenType.easeInQuint).setOnComplete(MatchAnimathinEnd);

        LeanTween.rotateAround(this.gameObject, Vector3.forward, 360.0f, 0.8f);
    }

    public void MatchAnimathinEnd()
    {
        LeanTween.scale(this.gameObject, Vector3.one, 0.3f)
            .setEase(LeanTweenType.easeInBounce);

    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
