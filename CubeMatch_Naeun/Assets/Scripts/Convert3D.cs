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
    //ť�� ������
    [SerializeField]
    private GameObject _CubePrefab;
    //ť���� �θ� ������Ʈ
    [SerializeField]
    private GameObject _MainObject;
    //ť�갡 �������� ��ƼŬ ����
    [SerializeField]
    private ParticleSystem _MainParticle;
    //���� ��ġ 2���� �迭 ���� (���Ӻ���)
    private GameObject[,] _Board = null;


    //ȭ�� ����� ���缭 �������� �������� �����Ǿ�� ��
    [SerializeField]
    private GameObject _RightFence;
    [SerializeField]
    private GameObject _LeftFence;

    //��ť���� �θ� ������Ʈ ����
    [SerializeField]
    private GameObject _Backobj;
    private GameObject[,] _backBoard = null;


    /// </summary>

    void Start()
    {
        //������� ����Ʈ�� �������� �����͸� ��´�
        CSV_FileLoad.OnLoadCSV("StageDatas", stageData);
        //����غ���
        Debug.Log("StageData.Count " + stageData.Count);


        //��ũ���� ���� �߰����� ���Ѵ�(Screen.height ���� ��ũ���� ���� ���� �����ͼ� 2�� ������)
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2f, 10f));
        //p1�� �߰����� ��ȯ�ȴ�
        Vector3 RightFensPos = _RightFence.transform.position;
        Vector3 LeftFencePos = _LeftFence.transform.position;

        //���� ���� ������ ���� ��ġ���� ȭ�� ����� ����� ( ȭ�� �ػ󵵰� ��� ���� �� �����Ƿ�)
        _RightFence.transform.position = new Vector3(p1.x, RightFensPos.y, 0f);
        _LeftFence.transform.position = new Vector3(-p1.x, LeftFencePos.y, 0f);

        //����Ʈ�� �ӽ÷� ������ְ�



        //PlayGame("Test", "TopSpin");
        PlayNextStage();
    }


    //�̹����� �������̶�� �����ϰ� ��� ����� �����Ѵ�
    private float XbaseWidth = 500f;
    private float YbaseHeight = 500f;

    /// <summary>
    /// 3D ť�� ���带 �����ϴ� �Լ�
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    private void CreateCube(List<Color32> colors, int height, int width)
    {
        #region ���� ���� �� ť�� �����ϰ� �ʱ�ȭ�ϱ�
        //�ش� ���尡 ���� �ƴϸ� ( ���忡 ť�갡 ���� )
        if (_Board != null)
        {
            //���Ӻ��� �ʱ�ȭ
            foreach (var item in _Board)
            {
                //���Ӻ��忡 ť�갡 ������ �����Ѵ�
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            //�麸�� �ʱ�ȭ 
            foreach (var item in _backBoard)
            {
                //�麸�忡 ť�갡 ������ �����Ѵ�
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        //�η� ������ش�
        _Board = null;

        //�麸�� �߰�
        _backBoard = null;
        #endregion

        //���� ���� �ٽ� �����Ѵ�
        _Board = new GameObject[height, width];

        //�麸�� ����
        _backBoard = new GameObject[height, width];

        //�÷�����Ʈ�� �ʱ�ȭ
        _colorList.Clear();

        //��� ����Ʈ �ʱ�ȭ
        _backBlockList.Clear();

        //�� ������Ʈ�� ���ο�����Ʈ�� ũ��� ��ġ �ʱ�ȭ--------------------------------------
        _MainObject.transform.localScale = Vector3.one;
        _MainObject.transform.position = Vector3.zero;

        _Backobj.transform.position = Vector3.zero;
        _Backobj.transform.localScale = Vector3.one;

        //��� �ʱ�ȭ
        isAllMadeToggle = false;

        //ȭ�鿡 ���� ��µǴ� �̹��� ����� ����ϱ� ���� ���� �����ϴ� ����
        //�̹������Ͽ��� �ȼ��� ������ ������ ���� �����ϰ� ��µǴ� �̹����� ���� ���� ������ ����ϱ� ����

        //���� ���� ���� ã���Ŷ� ���� ���� �� �ִ� ���� ū ���� ����
        //�� ��� �÷��� ������ �ִ� ť���� ���� ��ġ ( ������ �ּҰ�)
        int columnMin = height;
        //���������� �ʱ�ȭ ( ������ �ִ밪 )
        int columnMax = 0;
        //�� �������� ������ �ͼ� ū ���� ���� (������ �ּҰ�)
        int rowMin = width;
        //������ �ִ밪
        int rowMax = 0;


        //������ ����
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //���ڷ� ���� �÷��� ����Ʈ���� �ش� ��ġ�� �÷����� �����´�
                Color32 color = colors[i * width + j];

                //////////////////////////////////////////////////////////////////

                if (color.a != 0f)    //�÷��� ���İ��� 0�� �ƴѰ͵鸸 �����Ѵ�
                {
                    //�������� �������� �����.
                    GameObject obj = Instantiate<GameObject>(_CubePrefab);
                    //ť�� �̸� �����ֱ�
                    obj.name = $"Cube{i},{j}";
                    //�θ� �����ֱ�
                    obj.transform.SetParent(_MainObject.transform);
                    //�±� ����================================================>
                    obj.tag = "Cube";


                    //�� �Ŵ����� ������ �÷�, �÷�, �ο찪�� �������ش� ================>
                    BlockManager block = obj.GetComponent<BlockManager>();
                    block.OriginColor = color;
                    block.col = i;
                    block.row = j;

                    //��ġ ����ֱ�
                    obj.transform.position = new Vector3(j, i, 0f);

                    obj.GetComponent<MeshRenderer>().material.color = color;

                    //���Ӻ��忡 ������ ť���� �������� �߰��Ѵ�
                    _Board[i, j] = obj;

                    //�о�� �÷����� ����Ʈ�� �����Ѵ� ==========================>�麸�� ��Ī �� ���
                    _colorList.Add(color);

                    //�÷����� �־�� ���´� (����/ ���� ���� ������ ���)

                    //�ֻ��
                    if (columnMin > i) //i �� height ��
                    {
                        columnMin = i;
                    }
                    //���ϴ�
                    if (columnMax < i) //i �� height �� ������ �ִ� �� �߿� ū��
                    {
                        columnMax = i;
                    }
                    //���´�
                    if (rowMin > j) //i �� height ��
                    {
                        rowMin = j;
                    }
                    //�ֿ��
                    if (rowMax < j) //i �� height ��
                    {
                        rowMax = j;
                    }
                }

                //�麸�� �� �����ϴ� �ڵ� =================================================================
                if (color.a != 0f)
                {
                    GameObject backObj = Instantiate<GameObject>(_CubePrefab);
                    backObj.name = $"Cube_Back{i}{j}";

                    //�麸�嵵 �÷��� �÷� �ο찪�� ������ �־�� �� ===========================>
                    BlockManager block = backObj.GetComponent<BlockManager>();
                    block.OriginColor = color;
                    block.col = i;
                    block.row = j;

                    //������ٵ�� �ݶ��̴� ������Ʋ ���־���

                    //���� �� ������Ʈ�� ������ٵ� ������Ʈ�� ������ �� �ı��Ѵ�
                    Rigidbody body = backObj.GetComponent<Rigidbody>();
                    Destroy(body);

                    Collider collider = backObj.GetComponent<Collider>();
                    Destroy(collider);

                    backObj.transform.SetParent(_Backobj.gameObject.transform);
                    //?
                    backObj.transform.position = new Vector3(j, i, 0.5f);

                    //�麸����� ���鵵 �� ������Ʈ�� ������ �ְ� ����Ʈ�� ������ ============>
                    _backBlockList.Add(backObj.GetComponent<BlockManager>());

                    _backBoard[i, j] = backObj;

                }


            }
        }

        //ť����� ȭ�� ����� ����
        float midXvalue = width / 2;
        float midYvalue = height / 2;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //���� ������
                //������ ���� ������ �ƴ� ��쿡�� �����ؼ� ���忡 ����־���
                //���� ���尡 ���� ��찡 ������
                //������ �ֻ��, �ϴ�, �´�, ����� ���� ����
                if (_Board[i, j] != null)
                {
                    //�ҷ��� �����ǰ��� ������
                    Vector3 calPos = _Board[i, j].transform.position;
                    //�߾Ӱ��� ��ġ�ϵ��� ����
                    _Board[i, j].transform.position = new Vector3(calPos.x - midXvalue + 0.5f, calPos.y - midYvalue + 0.5f, 0f);
                    _backBoard[i, j].transform.position = new Vector3(calPos.x - midXvalue + 0.5f, calPos.y - midYvalue + 0.5f, 0f);
                }
            }
        }

        //ȭ�鿡 ��� ������ ���
        //�� �������� �� ������ ���� ���� ��µǴ� ����
        int calColumnCount = columnMax - columnMin + 1;
        //�ֻ�ܿ��� �� �ϴ��� ���� ���� ��µǴ� ����
        int calRowCount = rowMax - rowMin + 1;

        float xScale = 0f;
        if (calColumnCount < calRowCount)
        {
            //���κ��� ���ΰ� �� �� ��쿡�� ���θ� �������� ����
            //����Ƽ ����ü��� �ٲٱ� ���� 100���� ������
            xScale = XbaseWidth / 100f;
            xScale = xScale / calRowCount;
        }
        else
        {
            //���ΰ� �� �� ��쿡�� ���θ� �������� ��´�
            xScale = YbaseHeight / 100f;
            xScale = xScale / calColumnCount;
        }

        //xScale���� 0.22f ���� ũ�� ������ ���� ������Ų��
        if (xScale > 0.22f)
        {
            xScale = 0.22f;
        }

        //���ο�����Ʈ�� �������� ���� �������ش�
        _MainObject.transform.localScale = new Vector3(xScale, xScale, xScale);
        _Backobj.transform.localScale = new Vector3(xScale, xScale, xScale);


        //���� ���� �����ϰ��� ��������Ʈ�� �����Ͽ� ����======================>
        //���Ӻ������ ���� ���� �����ϰ��� ������ �ֵ��� ����Ѵ�.
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

        //Į�󸮽�Ʈ�� �ߺ��Ǵ� �÷��� ������ �� �ٽ� �÷� ����Ʈ�� �ִ´�===========>
        _colorList = _colorList.Distinct().ToList();

        //�÷�����Ʈ�� �÷����� ���� �÷����� �������� ã�� �ε����� ã�Ƽ� ��ȯ�Ѵ�.==============>
        //�÷�����Ʈ�� ����Ʈ ���·� ���� ��
        //���� ���� �ѹ��� ����Ʈ�� �ε����� �����Ѵ�.
        foreach (var item in _backBlockList)
        {
            int index = _colorList.FindIndex(x => x == item.OriginColor);
            item.OriginNumber = index;
            //�ε��� �� ���
            item.Numebr = index.ToString();

            //�麸��� ��ȣ�� �ο��� �� ���忡�� ��ȣ�� �ο��Ѵ�
            _Board[item.col, item.row].GetComponent<BlockManager>().Numebr = index.ToString();
            _Board[item.col, item.row].GetComponent<BlockManager>().OriginNumber = index;

            //�ؽ�Ʈ�� ����ش� (�麸�� �� + ���� ������ �̹� ���Ŵ��� ������Ʈ�� ������ ����)
            item.ShowOnOffNumebrText(true);
        }

        //�麸����� ���� ��ġ���� ����ؼ� ������Ƽ�� ����=======================================>
        foreach (var item in _backBoard)
        {
            if (item != null)
            {
                //���� �麸��� �ִ� ���� ��ġ���� ��� �� �ڿ�
                item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, 0f);
                //�麸�� ���� ���� �����ǿ� �־��ش�.
                item.GetComponent<BlockManager>().OriginPos = item.transform.position;
            }
        }



        Invoke("Crash", 2f);
    }
    /// <summary>
    /// �̹��� ������ ���ҽ� �������� �о���� �Լ�
    /// </summary>
    /// <param name="CategoryName"></param>
    /// <param name="ImageName"></param>
    public void PlayGame(string CategoryName, string ImageName)
    {
        //�о�� �ؽ��ĸ� ������ ������ ���� ����
        Texture2D texture = null;
        //�̹��� ��� ����
        string PATH = "StageImages" + "/" + CategoryName + "/" + ImageName;
        //���ҽ� �ε� ( ���, �о�� ������ Ÿ��, �ؽ��� 2d ���·� ����ȯ�ؼ� �ؽ��Ŀ� ����)
        texture = Resources.Load(PATH, typeof(Texture2D)) as Texture2D;
        //���� ���� �Լ� ȣ��
        BuildConvert3D(texture);
    }
    //���ο� ����
    private int _currentColumn = 0;
    //���ο� ����
    private int _currentRow = 0;


    /// <summary>
    /// �о�� �̹��� ������ �м��ؼ� 3d �� ���Ӻ��� �����ϴ� �Լ�
    /// </summary>
    /// <param name="texture"></param>
    private void BuildConvert3D(Texture2D texture)
    {
        //�ؽ����� ���� ��带 POINT�� �����Ѵ� (�������� ���·� �����ش� )
        texture.filterMode = FilterMode.Point;
        //�ؽ��� ������ �о�´�
        var textureFormat = texture.format;
        //���� ���ϴ� ������ �ƴϸ� ��� ���
        if (textureFormat != TextureFormat.RGBA32)
        {
            Debug.Log("�߸��� textureFormat �Դϴ�");
        }

        //���� �̹����� ����� �ٸ� �� ����. ���缭 ���带 �����ؾ� �Ѵ�
        int height = texture.height;
        int width = texture.width;

        //�޾ƿ� �̹����� ����� ��������� �������ش� (������ ��� ������)
        _currentColumn = height;
        _currentRow = width;

        //�ؽ��� �ȿ� �ִ� �÷����� �о ���ۿ� ����ش� 
        Color32[] colorBuffer = texture.GetPixels32();

        //�ȼ� ������ �����ϴ� �Լ�
        var textureColors = GenerateColors(colorBuffer, height, width);

        //�о���� �÷��� ���ڷ� �����Ѵ�
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
        //�迭�� ��� �÷����� ����Ʈ�� �ٽ� �־��ִ� ����
        //���� ���θ�ŭ �÷��� ����Ʈ�� �����
        List<Color32> vertexColors = new List<Color32>(height * width);
        //�÷����� �ϳ��� �о ����Ʈ�� �ִ´�
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color32 color = colorBuffer[j + i * width];
                //���� �÷����� ����Ʈ�� �߰��Ѵ�
                vertexColors.Add(color);
            }
        }
        //�ش� ����Ʈ�� �����Ѵ�
        return vertexColors;
    }

    private void Crash()
    {
        foreach (var item in _Board)
        {
            //�������� ���� �ƴϸ�
            if (item != null)
            {
                //�������� ��ġ�� �������� ��ġ�ϰ�
                item.transform.position = new Vector3(item.transform.position.x, item.transform.position.y, -0.1f);
                //���� �ִ� ������ٵ� ���������� Ǯ�� �߷� ������ �ް� �Ѵ�.
                item.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                item.GetComponent<Rigidbody>().useGravity = true;
            }
        }
        Invoke("ResetColliderSize", 0.1f);
    }
    /// <summary>
    /// �浹�� �� �ڽ��� �ݶ��̴� ����� �ٽ� 1�� ����� �Լ�
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
    //�̹������� �ҷ��� �÷��� ����
    private List<Color> _colorList = new List<Color>();
    //����� �ִ� �� ������Ʈ ����
    //����� ����� �÷����� �����ϴ� �������� �����Ѵ�
    private List<BlockManager> _backBlockList = new List<BlockManager>();


    /// <summary>
    /// ���콺 �Է��� Ŭ���ϸ� Ŭ���� ��ũ�� ��ǥ���� �߻�� ������ ��ġ��
    /// ù��° ť�� ���� ��ȯ�ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    private GameObject ReturnClickedObject()
    {
        //ratcast�� ������ �߻��ؼ� ������ �������� ������Ʈ�� ã�Ƴ���
        GameObject target = null;
        //ȭ���� ���콺�� Ŭ���������� ������ ������ �����ؼ� ray�� �־��ش� (���� ����)
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //������ �������� ������Ʈ�� ã�´�
        //�������� �������ִ� ������Ʈ���� �� ã������

        //������ �浹�� �ֵ��� �����ϴ� ������Ÿ�� �迭�� �����
        //������ �������� ������ �������� 10��ŭ�� ���̿� �������� �ֵ��� �����ش�
        RaycastHit[] hitInfo = Physics.RaycastAll(ray.origin, ray.direction * 20);

        foreach (var item in hitInfo)
        {
            //������ �ε��� �����߿� �ݶ��̴��� �ְ� ť�� �±װ� �ִ� ���鸸 
            if (item.collider != null && item.transform.tag == "Cube")
            {
                //ó�� ��ġ�� ���� ã�� ���ǹ��� ��������
                target = item.collider.gameObject;

                //Destroy(target);
                break;
            }
        }
        return target;
    }

    /// <summary>
    /// �巡�׵� ���� �÷����� �ش��ϴ� �麸����� �� �ؽ�Ʈ �÷� ����
    /// </summary>
    /// <param name="index"></param>
    /// <param name="color"></param>
    private void ChangeBlockTextColor(int index, Color color)
    {
        foreach (var item in _backBoard)
        {
            //�麸�� ���� ������Ʈ�� ���� �ƴϸ� (�����ϸ�)
            if (item != null)
            {
                //�麸����� �÷����� �ο��� ��ȣ�� �ε����� ������
                if (item.GetComponent<BlockManager>().OriginNumber == index)
                {
                    //�麸����� ������ �÷����� �����Ѵ� ( �� �Ŵ����� �ִ� �Լ� ȣ���Ѵ�)
                    item.GetComponent<BlockManager>().SetNumberTextColor(color);
                }
            }
        }
    }
    /// <summary>
    /// �麸����� ���� Ÿ�ٺ��� ��ġ���� �÷����� ��ġ�ϴ��� ���ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    public GameObject isMatchPosColorBlock(GameObject target)
    {
        foreach (var item in _backBlockList)
        {
            //����󿡴� ���� ���� (��ġ�� üũ) => �׷��� ��ġ�� üũ�ϴ°� �ǹ̰� ���� (�÷�����üũ)
            //�麸����� ���� �̹� fixed�� �ƴϾ�� �� (fixed�� �ƴ� ���� ó��)
            if (item.CurrentState != BlockManager.STATE.FIXED &&
                item.CheckMathchPos(target) &&
                item.CheckMatchColor(target))
            {
                //��ġ�� ��ġ�� ���� �����Ѵ�
                return item.gameObject;
            }
        }
        return null;
    }


    //������Ʈ������ Ŭ���� ���� ã�� ����ʵ�
    //���콺 Ŭ���� ���õ� ������Ʈ
    private GameObject target = null;
    //���콺 Ŭ�� �� �巡�� �������� üũ�ϴ� ����ʵ�
    private bool isMouseDrag = false;

    //Ŭ���� ������Ʈ���� ��ġ�� �����ϴ� ��
    private const float CLICK_Y_OFFSET = 1.3f;
    private const float CLOCK_Z_OFFSET = -0.5f;

    //��ũ����ǥ�� ȭ�鰣�� ���̸� �����ϱ� ���� ����ʵ�
    private Vector3 screenPos;
    private Vector3 offset;

    //������� ���� ��� ������� Ȯ���ϴ� �ʵ�-------------------------------------
    private bool isAllMadeToggle = false;

    public enum ItemType
    {
        STEP,   //�ϳ��� ó��
        MAGNET, //�ֺ��� �ִ� �͵� ó��
        AUTO    //�Ѳ����� ó��
    }
    //������ Ÿ�Կ� ���� �÷���
    [SerializeField]
    private ItemType Type = ItemType.STEP;
    public ItemType type
    {
        set { type = value; }
        get { return Type; }
    }

    /// <summary>
    /// ���콺 Ŭ�� �̺�Ʈ ó��---------------------------------------------
    /// </summary>
    private void MouseEventProcess()
    {
        //� ���� Ŭ���Ǿ����� ã�Ƴ���====================================>
        if (Input.GetMouseButtonDown(0))
        {
            //����ʵ忡 ��ȯ�� ������Ʈ�� �������ش�.
            target = ReturnClickedObject();

            //���ϵ� ������Ʈ�� ���� �ƴϸ� => Ŭ���� ť�� ���� ������
            if (target != null)
            {
                //���� ��⸸ �ϸ� ������� �޶����
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
                    //���콺 �巡�� ���·� ����
                    isMouseDrag = true;
                    //ť�긦 ���� �� �麸�� �� �ش� Ŭ���� ť���� �÷����� ����
                    //�麸�� ���� �� �ؽ�Ʈ ���� ����
                    ChangeBlockTextColor(target.GetComponent<BlockManager>().OriginNumber, Color.red);

                    //Ÿ���� ��ġ���� �����ͼ� �������� ������ ���ش�
                    Vector3 clickedObjectPos = target.transform.position;
                    // ��ġ���� �ٲٸ� �������� �ٲ� �������Ƿ� �߷��� ���ش�
                    target.transform.position = new Vector3(clickedObjectPos.x, clickedObjectPos.y, -1f);
                    //������ٵ� �ı��Ѵ�
                    Destroy(target.GetComponent<Rigidbody>());
                    //���õ� ���� �ո��� ���� ȸ������ �ʱ�ȭ���ش�
                    target.transform.localEulerAngles = Vector3.zero;

                    //���õ� ���� Ŭ���� ��ġ���� ���ʿ� ǥ�õǵ��� ó���Ѵ� (���� ��)
                    Vector3 pos = target.transform.position;
                    target.transform.position = new Vector3(pos.x, pos.y + CLICK_Y_OFFSET, pos.z + CLOCK_Z_OFFSET);

                    //���õ� ���� �������� �����Ѵ� (������ �� ������ ������ Ŀ����)
                    target.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

                    //���� ���忡 �ְ�, ť���� ���� �� ��ǥ�� ��ũ�� ��ǥ�� �ٲپ ����
                    screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
                    //������ Ŭ���� ���� ��������� ���� �����¿� ����
                    offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z));
                }
            }
        }

        //���콺 ��ư�� ���� ��
        if (Input.GetMouseButtonUp(0))
        {
            //�巡�װ� Ǯ�� ���·� ����
            isMouseDrag = false;

            // ��ư�� ���� �� ���� ������ �ٽ� ���������� �ٲ��ش�.
            if (target != null)
            {
                // �� ������Ʈ�� �ѹ��� �������� �ʰ� �׳� �ε����� -1�� �����ϸ�
                //�ε����� �ش�Ǵ� ���� ���� ������ ��� ���� �������� �ٲ��.
                ChangeBlockTextColor(target.GetComponent<BlockManager>().OriginNumber, Color.black);

                //���� ��Ҵٰ� ���� ��쿡 ������ٵ� �߰����ش� (����� �� �ı�����)
                target.AddComponent<Rigidbody>();
                target.transform.localScale = target.GetComponent<BlockManager>().OriginScale;
            }

            //Ÿ���� �η� �ʱ�ȭ
            target = null;
        }

        //���콺�� Ŭ���� ���¿��� ���콺�� �̵� �� ó��
        if (isMouseDrag)
        {
            //���콺�� Ŭ���� ��ġ�� ��������
            Vector3 currentScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPos.z);
            //���콺�� Ŭ���� ��ġ�� ���� ��ǥ�� �ٲ��� ����, ������ �������� �����Ѵ�
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(currentScreenPos) + offset;
            /*
             ���� �� �߾��� Ŭ���ϴ°��� �ƴϱ� ������ �߾����κ��� Ŭ���� ��ġ������ ��������
             */
            if (target != null)
            {
                target.transform.position = currentPos;

                //�̵��Ҷ����� �麸�� ���� �Ϲ� ���� ��ǥ�� ���ƴ��� Ȯ���ϴ� �Լ� �ʿ���
                //�巡�׵� ���� �̵��Ҷ����� ��ġ�ϴ� �麸����� ���� �ִ��� üũ�Ѵ�
                //�巡�� �����̸� ��� ��
                GameObject matchObj = isMatchPosColorBlock(target);

                if (matchObj != null)
                {
                    //��ġ�� ť�� ���� �麸��� ��ġ��Ų��
                    //Ÿ�ٺ��ؼ� Ÿ�� ���� ���ϵ� ���� ��ġ�� ���Խ�Ų��
                    target.transform.position = matchObj.GetComponent<BlockManager>().OriginPos;
                    target.transform.localScale = target.GetComponent<BlockManager>().OriginScale;
                    //��ġȮ�ε� ���� ��Ȱ��ȭ �����ش�
                    matchObj.SetActive(false);

                    ///��ġ�Ϸ�
                    ///�ش� ��ġ�� ��Ī �Ǿ����� ó���Ѵ�
                    target.GetComponent<BlockManager>().CurrentState = BlockManager.STATE.FIXED;
                    matchObj.GetComponent<BlockManager>().CurrentState = BlockManager.STATE.FIXED;

                    //��ġ�� ������Ʈ���� �׵θ��� ���ش� //?
                    matchObj.GetComponent<BlockManager>().ShowOnOffNumebrText(true);

                    //Ÿ�ٺ��� �̹� ��ġ�� �Ǿ����Ƿ� �ݶ��̴��� ����������
                    Destroy(target.GetComponent<Collider>());

                    //ȭ��� ����Ǿ��� �ؽ�Ʈ �÷����� �ʱ�ȭ�Ѵ�
                    ChangeBlockTextColor(target.GetComponent<BlockManager>().OriginNumber, Color.black);

                    //��ġ�� ���� ��Ʈ�� �ִϸ��̼� ȿ���� �ش�
                    //target.GetComponent<BlockManager>().MatchAnimationStart();
                    target.GetComponent<BlockManager>()._Particle.Play();
                    //?
                    target.GetComponent<BlockManager>().MatchAnimationStart();

                    

                    //Ÿ���� �η� �ٲ��ְ� �巡�� ���¸� false�� �ٲ��ش�
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
            //�ѹ��� ȣ��ǰ� ó��
            isAllMadeToggle = true;
            Debug.Log("AllBlockMatched");
            //���� ������ �Ŀ� �ִϸ��̼� ó��
            AllBlockComplete();
        }
    }

    /// <summary>
    /// ���� ���� ��� ���� ��ġ�Ǿ�����üũ�ϴ� �Լ�
    /// </summary>
    /// <returns></returns>
    private bool IsAllMatchBlock()
    {
        //�麸��� �� ������ ��ġ�� �Ǹ� ������ fixed�� �ٲ��ֹǷ�
        //���ſ��� Ȯ���Ѵ�

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
    /// ��� ���� �������� �� �Ĵ� ó���� �Լ�
    /// </summary>
    private void AllBlockComplete()
    {
        SoundManager.Instance.Congreturation();
        _MainParticle.Play();
        Invoke("ClearAnimation", 1f);
    }
    /// <summary>
    /// ��� ���� ���� �� �ִϸ��̼� ó��
    /// </summary>
    private void ClearAnimation()
    {
        //�� �ϳ��� �������� ���ϱ� ���� �����ش�.
        float animationScale = XbaseWidth / _currentRow;
        //����Ƽ ���� 100���� �����ش�
        animationScale /= 100f;
        //������ ���� �� ������ �����ش�
        animationScale /= 2f;

        Vector3 mainScale = new Vector3(animationScale, animationScale, animationScale);
        //���� �������� ���� �����Ϸ� 0.5�ʵ��� �ٲ۴�

        LeanTween.scale(_MainObject, mainScale, 0.5f).
            setEase(LeanTweenType.easeInBack).
            setOnComplete(ClearAnimationComplete);
    }

    private void ClearAnimationComplete()
    {
        _MainParticle.Stop();
        
        _MainObject.GetComponent<Animator>().enabled = true;
        //����Ƽ �ִϸ��̼� ����
        _MainObject.GetComponent<Animator>().SetTrigger("ClearAnim");
    }

    //CSV������ �ɰ��� �����͸� ��� ����Ʈ ����
    //List<StageData> stageData = new List<StageData>();
    //���� ���������� ������ ������� (-1�� �ʱ�ȭ�Ѵ�)
    List<StageData> stageData = new List<StageData>();
    private int CurrenStageNum = -1;

    /// <summary>
    /// ���� ���������� �����ϰ� �ϴ� �Լ�
    public void PlayNextStage()
    {
        CurrenStageNum++;
        PlayGame(stageData[CurrenStageNum].CategoryName, stageData[CurrenStageNum].ImageName);
        //���������� Ŭ���� �ؾ� �����ؾ��Ѵ�.
    }

    //�ֺ��� ���� �پ��ִ� ������ �÷� �� �����
    private List<GameObject> _matchObject = new List<GameObject>();

    /// <summary>
    /// ��ġ�� ������Ʈ���� �Ѳ����� ����
    /// ��ġüũ �Լ� �ȿ��� �ٽ� ���� �Լ��� ȣ���ؼ� �ֺ��� ���� ���� �ִ��� ȣ���Ѵ�
    /// �ֺ��� ���� ���� ���� _matchObject ����Ʈ�� �����Ѵ�
    /// ������� ������ �̿��ϱ� ���� ��ġ������Ʈ�� ī��Ʈ�� ��ȯ�Ѵ�
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
        //��ġ������Ʈ�� ���� ���� ������
        if(_matchObject.Count > 0)
        {
            //������ ó���ߴ� ������ �ʱ�ȭ�Ѵ�
            _matchObject.Clear();
        }

        foreach (var item in board)
        {
            if(item != null)
            {
                //����� ���� isMatchedCheck �ʱ�ȭ
                item.GetComponent<BlockManager>().IsMatchedCHeck = false;
            }
        }
        matchObject.GetComponent<BlockManager>().IsMatchedCHeck = true;
        BlockManager matchObjectBlock = matchObject.GetComponent<BlockManager>();

        //��Ʈ��ŷ

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
            //���� ���� ������ ����
            List<GameObject> flyBlocks = new List<GameObject>();
            foreach (var item in board) 
            { 
                if(item != null && item.GetComponent<BlockManager>().CurrentState != BlockManager.STATE.FIXED
                    && item.GetComponent<BlockManager>().OriginNumber == matchObject.GetComponent<BlockManager>().OriginNumber)
                {
                    //ó������ �ʰ� �ٴڿ� ������ ���� �߿� �麸����� �ѹ�(�÷�)�� �����ϴٸ�
                    //���� ����Ʈ�� �־���´�
                    flyBlocks.Add(item);

                    //��ġ�� ���� ���ڿ� ó������ ���� ���� ������ ������ Ȯ��
                    if(_matchObject.Count == flyBlocks.Count) 
                    {
                        break;
                    }
                }
            }

            int cnt = 0;
            foreach (var item in flyBlocks) 
            {
                //������ �Լ� �߰�
                Destroy(item.GetComponent<Collider>());
                //��ġ�� ������Ʈ�� �� �迭�� ������Ű�鼭 ����� �ִ´�.
                item.GetComponent<BlockManager>().MoveToFixedPos(_matchObject[cnt++]);
            }
        }
        return _matchObject.Count;
    }
    
    

    /// <summary>
    /// ���� ���ȣ���� ���� �̿��ϴ� ������
    /// </summary>
    public enum DIRECTION
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    /// <summary>
    /// ����Լ��� ���� �����¿�� ���� ���� �ִ��� üũ�ϴ� ����Լ�  
    /// </summary>
    private void MatchCheck(GameObject[,] board, int col, int row, int blockNum, int width, int height, DIRECTION dir)
    {
        //������� ������ ����ų�, ���� ���� �ƴϰų�, �̹� fixed �� ���̸� �����Ѵ�
        if(col < 0 || //������� ������ ����ų�
           col >= height|| 
           row < 0 ||
           row >= width || 
           board[col,row] == null || //���� ���ų�
           board[col,row].GetComponent<BlockManager>().CurrentState == BlockManager.STATE.FIXED ||  //�̹�fixed�ǰų�
           board[col,row].GetComponent<BlockManager>().OriginNumber != blockNum || //���� ��ȣ�� �ٸ��ų�
           board[col,row].GetComponent<BlockManager>().IsMatchedCHeck)  //�̹� ��ġ�� ���̰ų�
                                                                        //ó���� �ʿ䰡 ����
        {
            return;
        }

        //üũ�� ���� ���� ���̸�
        if (!board[col,row].GetComponent<BlockManager>().IsMatchedCHeck)
        {
            //��ġ �� ����Ʈ�� �߰��Ѵ�
            _matchObject.Add(board[col, row]);
            //��ġüũ ó��
            board[col, row].GetComponent<BlockManager>().IsMatchedCHeck = true;
        }

        //�Դ� ������ �ݴ� �������δ� ������ �ʴ´� (�ֺ��� ���� ���� ���� ã��)
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
    /// ������� ���õ� �� �÷�(�ε���)�� �´� ���� ã�Ƽ� ��� ��ġ
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




