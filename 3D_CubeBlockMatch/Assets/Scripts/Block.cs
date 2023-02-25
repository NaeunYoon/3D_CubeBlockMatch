using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int col { set; get; }    //���ο� ��ġ
    public int row { set; get; }    //���ο� ��ġ
    public enum STATE{

        STOP,
        MOVE,
        MOVING,
        FIXED
    }
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


    //���� ���� ���°� ����
    public STATE CurrentState { set; get; }

    //�ο����� ��ȣ�� ǥ��(NumbertText)=>�������� �ؽ�Ʈ�� �����´�
    [SerializeField] private TextMesh _sNumberText;
    //�׵θ�(Edge)=>�������� �׵θ��� �����´� (�ڽ����� 2d sprite ����)
    //(2d�� sprite renderer), z ���� -0.5 �� ������ ������ �Ѵ�
    [SerializeField] private GameObject _sEdge;

    

    /// <summary>
    /// ť�꿡 �÷� �ε��� ���ڸ� ����ϴ� �Լ�
    /// �����ְų� �������� �ʰ� ó���Ѵ�
    /// </summary>
    /// <param name="onoff"></param>
    public void ShowNumberText(bool onoff)
    {
        _sNumberText.gameObject.SetActive(onoff);
        _sEdge.SetActive(onoff);
    }
    /// <summary>
    /// �ؽ�Ʈ�� �÷����� ����(�Է�)�ϴ� �Լ�
    /// </summary>
    /// <param name="color"></param>
    public void SetNumberTextColor(Color color)
    {
        _sNumberText.color = color;
    }

    private const float CHECKPOSITIONRANGE = 0.1f; //����ó��

    /// <summary>
    /// ���ڷ� ���� ���� ��ġ�� ������ üũ�Ѵ� ( ������ ����)
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CheckMatchPosition(GameObject target)   //���ڷ� ť����� ���ð���
    {
        //����� ���� ���� ������ �����Ѵ� (Ʈ���������� �����͵� �ȴ�)
        //�׸� �ΰ� �ִµ� ������ŭ�� ���� ���� ����. (�׸� ������ ���� �� ���̿� ���Դ��� üũ��)
        if((OriginPos.x - CHECKPOSITIONRANGE)< target.transform.position.x && 
           (OriginPos.x + CHECKPOSITIONRANGE) > target.transform.position.x &&
            (OriginPos.y - CHECKPOSITIONRANGE) < target.transform.position.y &&
            OriginPos.y + CHECKPOSITIONRANGE > target.transform.position.y) 
        {
            return true;    //��ħ
        }
        return false;   //�Ȱ�ħ
    }
    /// <summary>
    /// ���ڷ� ���޵� ���� �÷��� �麸����� ���� �÷����� ������ üũ
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CheckMatchColor(GameObject target) 
    { 
        //�麸�� ���� �ִ� ���� ���ڷ� ���޵� ���� ���� ������ Ȯ���Ѵ�
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
