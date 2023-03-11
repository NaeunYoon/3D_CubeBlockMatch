using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    //���� �ο��� ��ȣ�� ǥ��
    [SerializeField]
    private TextMesh _NumberText;

    //�׵θ� ǥ��
    [SerializeField]
    private GameObject _Edge;

    //��ƼŬ ǥ��
    [SerializeField]
    public ParticleSystem _Particle;


    //���� ������ �麸�� ���� ��ġ���� ����ؼ� ã�ư��� ��
    //���ο� ��ġ
    public int col { set; get; }
    //���ο� ��ġ
    public int row { set; get; }    

    //�� ������ ��ġ�� ����
    public Vector3 OriginPos { set; get; }

    //�� ������ �����ϸ� ����
    public Vector3 OriginScale { set; get; }

    //�� ������ ���� ����
    public Color OriginColor { set; get; }

    //�� ������ �ѹ��� ����
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
    /// ���� �ο��� �ε��� ���� ���ų� Ű�� �Լ� (��¿��� Ȯ��)
    /// </summary>
    public void ShowOnOffNumebrText(bool onOff)
    {
        _NumberText.gameObject.SetActive(onOff);
        _Edge.SetActive(onOff);
    }
    /// <summary>
    /// �ؽ�Ʈ �÷��� �������ִ� �Լ� (����)
    /// </summary>
    /// <param name="color"></param>
    public void SetNumberTextColor(Color color)
    {
        _NumberText.color = color;
    }


    //����(ũ���ټ��� ���� �޶����)
    private const float CHECK_POS_RANGE = 0.2f;


    /// <summary>
    /// ���ڷ� ���� ���� ��ġ�� ������ üũ�ϴ� �Լ�
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool CheckMathchPos(GameObject target)
    {
        //���� ���� ������ �������� ���� �߰����̴�.
        //01.��ŭ ���� ���Ѵٴ� ���� ���� ��ǥ���� ���� �簢�� �ȿ� ���ԵǴ��� Ȯ���Ѵ�
        //������ŭ ���ϸ� �Ǳ� ������ ���������� �ؾ��Ѵ� ( ����� ū �׸�, ������ ���� ��ġ�� ��)
        //Ÿ���� ��ġ�� Ȯ���Ѵ�
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
    /// ���ڷ� ���޵� ���� �÷��� �麸����� �÷����� ������ ��
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
    /// ���� ���� �ڸ��� ��� �� �� �ֱ� ������ ����ó���� �Ѵ�
    /// </summary>
    public enum STATE
    {
        STOP,   //����
        MOVE,   //�����̷��� �ϴ� ����
        MOVING, //�����̴� ����
        FIXED   //�麸�� �� ��ġ���ִ� ����
    }
    /// <summary>
    /// ���� ���� ���°��� �����ϴ� ������ �����
    /// </summary>
    public STATE CurrentState
    {set; get;}

    /// <summary>
    /// ��Ʈ���� �̿��� �ִϸ��̼� ȿ��
    /// </summary>
    public void MatchAnimationStart()
    {
        LeanTween.scale(this.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.3f).
            setEase(LeanTweenType.easeInQuint). //�߰��� ���� ��� ��ȭ��ų������
            setOnComplete(MatchAnimationEnd);   //�ִϸ��̼��� ������ �� ��� �� ���� �Լ� ȣ��

        //ȭ�� �������� 0.8�ʵ��� �ѹ��� ����
        LeanTween.rotateAround(this.gameObject, Vector3.forward, 360f, 0.8f);
    }

    public void MatchAnimationEnd()
    {
        LeanTween.scale(this.gameObject, Vector3.one, 0.3f) //0.3�ʵ��� ���� ������� �ٿ��ش�
            .setEase(LeanTweenType.easeInBounce);
    }

    /// <summary>
    /// ���� ��ġüũ�� �ߴ��� ���ߴ��� ó���ϴ� ������Ƽ
    /// </summary>
    public bool IsMatchedCHeck { set; get; }

    /// <summary>
    /// �麸�� ���� ��ġ�� ����, �ٴڿ� �ִ� ������ �麸��� ������ ��Ȱ��ȭ �����ش�
    /// �ٽ� ȣ����� �ʰ� fixed�� ó�����ش�
    /// ���� ���ڷ� ���� ����� ��ġ�� ���� ������.
    /// </summary>
    /// <param name="gameObject"></param>
    public void MoveToFixedPos(GameObject gameObject)
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<BlockManager>().CurrentState = STATE.FIXED;

        //0.8�ʵ��� �� ���� ������Ʈ�� ������ ��ġ�� 0.8�ʵ��� ���ư��� ȿ�� ����
        LeanTween.move(this.gameObject, gameObject.transform, 0.8f).setEase(LeanTweenType.easeInBack)
            .setOnComplete(MoveToFixedPosComplete);
    }
    /// <summary>
    /// ��Ʈ�� ȿ�� ���� �� ����
    /// </summary>
    public void MoveToFixedPosComplete()
    {
        SoundManager.Instance.Play_BlockClick();
        //���ư� ���� ȸ������ �ʱ�ȭ�Ѵ�
        this.gameObject.transform.localEulerAngles = Vector3.zero;
        //�׸��� ���� ����
        this.GetComponent<MeshRenderer>().material.renderQueue = 2900;
        //���� ������ ������ �ٲ��ش�
        this.gameObject.transform.localScale = OriginScale;
        CurrentState = STATE.FIXED;
        //���� �����ؽ�Ʈ�� ǥ������ �ʴ´�
        ShowOnOffNumebrText(false);
        //������ٵ�� �ݶ��̴��� ����ó��
        Destroy(this.gameObject.GetComponent<Rigidbody>());
        Destroy(this.gameObject.GetComponent<Collider>());
    }


    

}
