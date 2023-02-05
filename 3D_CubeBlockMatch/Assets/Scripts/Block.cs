using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
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


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
