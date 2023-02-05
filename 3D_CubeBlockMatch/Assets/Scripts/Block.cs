using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
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


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
