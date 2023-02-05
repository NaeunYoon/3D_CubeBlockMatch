using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Convert_3D : MonoBehaviour
{
    //ť�� ������
    [SerializeField] private GameObject _sCubePrefab;
    //ť���� �θ� ������Ʈ
    [SerializeField] private GameObject _sMainObject;

    //�Ͽ�¡
    [SerializeField] private GameObject _sRightFence;
    [SerializeField] private GameObject _sLeftFence;

    [SerializeField] private GameObject _sBackObject;   //��ť���� �θ� ������Ʈ

    //���Ӻ���
    private GameObject[,] _board = null;
    private GameObject[,] _Backboard = null;
    void Start()
    {
        //��ũ�� ��ǥ���� ���ߴ� ��ǥ���� ������ǥ�� ���� ���Ѵ�
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(0f,Screen.height/2f, 10f));
        Debug.Log(p1);

        Vector3 RightFencePosition = _sRightFence.transform.position;
        Vector3 LeftFencePosition = _sLeftFence.transform.position;

        //���� ���� ������ ���� ��ġ���� ȭ�� ����� ���缭 ��ġ��Ų��
        _sRightFence.transform.position = new Vector3(p1.x, RightFencePosition.y, 0f);
        _sLeftFence.transform.position = new Vector3( -p1.x, LeftFencePosition.y, 0f);

        
        
        // Create3Cube(,10, 10);
        PlayGame("foods", "DragonFruit");

        
    }

    /// <summary>
    /// 3D CUBE ���带 ��ġ�Ѵ�
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    private void Create3Cube(List<Color32> colors, int height, int width)
    {
        //���Ӻ��� �ʱ�ȭ
        if(_board != null)
        {
            foreach (var item in _board)
            {
                //���Ӻ��忡 ť�갡 ������ �����Ѵ�
                if(item != null)
                {
                    Destroy(item);
                }
            }
        }

        _board = null;

        _Backboard= null;

        //���Ӻ��� ����
        _board = new GameObject[height, width];

        _Backboard= new GameObject[height, width];

        //ȭ�鿡 ���� ��µǴ� �̹��� ����� ����ϱ� ���� ���� ����
        //�̹��� ���Ͽ��� �ȼ��� ������ ������ �ȼ��� �����ϰ� ��µǴ� �̹����� ���ο� ������ ���ο� ������ ����ϱ� ���� ����
        //������ �ּҰ� ( �ֻ�� �÷��� ������ �ִ� ť���� ���� ��ġ)
        int columMin = height;
        //������ �ִ밪
        int columnMax = 0;
        int roMin = width;
        int roMax = 0;


        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color32 color = colors[i * width + j];

                //�÷���ť�� ���� ����
                //�������� ���� ���鸸 ����Ѵ�
                if(color.a != 0f)
                {
                    GameObject obj = Instantiate(_sCubePrefab);
                    //ť�꿡 �̸��� �ο��Ѵ�
                    obj.name = $"Cube[{i},{j}]";
                    //�������� ���� ť�꿡 �θ� ������Ʈ�� �����Ѵ�
                    obj.transform.SetParent(_sMainObject.transform);

                
                    obj.transform.position = new Vector3(j, i, 0.0f);

                    obj.GetComponent<MeshRenderer>().material.color = color;

                    //���Ӻ��忡 ������ ť���� �������� �߰��Ѵ�
                    _board[i, j] = obj;

                    //���� ���� ���� ������ ���
                    if(columMin>i)  //�ֻ��
                    {
                        columMin = i;
                    }

                    if(columnMax <i)    //���ϴ�
                    {
                        columnMax = i;
                    }

                    if(roMin > j)   //���´�
                    {
                        roMin = j;
                    }

                    if(roMax < j)   //�ֿ��
                    {
                        roMax = j;
                    }

                }
                //�麸�� ���� ����
                if(color.a != 0f)
                {
                    GameObject backObj = Instantiate(_sCubePrefab);
                    backObj.name = $"CubeBack[{i},{j}]";
                    //������Ʈ ����
                    Rigidbody rigidbody= backObj.GetComponent<Rigidbody>();
                    Destroy(rigidbody);
                    Collider collider = backObj.GetComponent<Collider>();
                    Destroy(collider);

                    backObj.transform.SetParent(_sBackObject.transform);
                    backObj.transform.position = new Vector3(j,i,0.0f);
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

        //ȭ�鿡 ��� ������ ����Ѵ�
        int calColumnCount = columnMax - columMin + 1;
        int calRowCount = roMax - roMin + 1;

        //�����ϰ� ���
        float Xscale = 0f;
        //���ΰ� �� ŭ
        //���ΰ����� ���ΰ��� ū ��� ���ΰ��� �������� ������ ����
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

        //���� ������ ����,�� ť���� �θ� ������Ʈ�� �����Ѵ�.
        _sMainObject.transform.localScale = new Vector3(Xscale, Xscale, Xscale);
        _sBackObject.transform.localScale = new Vector3(Xscale, Xscale, Xscale);
        Vector3 mainPosition = _sMainObject.transform.position;
        _sMainObject.transform.position = new Vector3(mainPosition.x, mainPosition.y + 2f, 0f);
        Vector3 backPosition = _sBackObject.transform.position;
        _sBackObject.transform.position = new Vector3(backPosition.x, backPosition.y + 2f, 0f);

        Invoke("Crash", 1f);

    }
    /// <summary>
    /// �̹��� ������ ���ҽ� �������� �о�´�
    /// </summary>
    /// <param name="categoryName"></param>
    /// <param name="imageName"></param>
    public void PlayGame(string categoryName, string imageName)
    {
        Texture2D texture = null;

        //�̹��� ��θ��� ����
        string PATH = "StageImages/" + categoryName + "/" + imageName;

        texture = Resources.Load(PATH,typeof(Texture2D)) as Texture2D;

        BuildConvert3D(texture);
    }


    private int _CurrentColumn = 0;
    private int _CurrentRow = 0;
    /// <summary>
    /// �о�� �̹��� ������ �м��ؼ� 3D ���带 �����Ѵ�
    /// </summary>
    /// <param name="texture"></param>
    private void BuildConvert3D(Texture2D texture)
    {
        texture.filterMode = FilterMode.Point;

        var textureFormat = texture.format;

        if( textureFormat != TextureFormat.RGBA32)
        {
            Debug.Log("32��Ʈ �÷��� ���");
        }

        //������ �ȼ� ����
        int height = texture.height;
        //������ �ȼ� ����
        int width = texture.width;

        //�̹����� ���� ���� �� ���
        _CurrentColumn = height;
        _CurrentRow = width;

        Color32[] colorBuffer = texture.GetPixels32();

        //�ȼ������� �о�´�
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
    //�̹����� ��� �������̶�� �����ϰ� ��� ����� ����
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
                
            }
        }
    }


    void Update()
    {
        
    }
}
