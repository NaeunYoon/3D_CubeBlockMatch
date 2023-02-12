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
    //���� ���� ���°� ����
    public STATE CurrentState { set; get; }


    //�ο����� ��ȣ�� ǥ��
    [SerializeField] private TextMesh _sNumberText;
    //�׵θ�
    [SerializeField] private GameObject _sEdge;

    
    public int col { set; get; }    //���ο� ��ġ
    public int row { set; get; }    //���ο� ��ġ

    //�� ������ ��ġ�� ����
    public Vector3 OriginPos { set; get; }
    //�� ������ ������ �� ����
    public Vector3 OriginScale { set; get; }
    //�� �÷����� ����
    public Color OriginColor { set; get; }
    //���� ���� ��ȣ ����
    public int BlockNumebr { set; get; }
    //�� ���� ������Ƽ
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
    /// ť�꿡 �÷� �ε��� ���ڸ� ����ϴ� �Լ�
    /// </summary>
    /// <param name="onoff"></param>
    public void ShowNumberText(bool onoff)
    {
        _sNumberText.gameObject.SetActive(onoff);
        _sEdge.SetActive(onoff);
    }
    /// <summary>
    /// �ؽ�Ʈ�� �÷����� �����ϴ� �Լ�
    /// </summary>
    /// <param name="color"></param>
    public void SetNumberTextColor(Color color)
    {
        _sNumberText.color = color;
    }

    private const float CHECKPOSITIONRANGE = -0.1f;

    /// <summary>
    /// ���ڷ� ���� ���� ��ġ�� ������ üũ�Ѵ�
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
    /// ���ڷ� ���޵� ���� �÷��� �麸����� ���� �÷����� ������ üũ
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
