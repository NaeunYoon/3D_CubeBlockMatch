using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class Convert_3D : MonoBehaviour
{
    //ť�� ������
    [SerializeField] private GameObject _sCubePrefab;
    //ť���� �θ� ������Ʈ ( ��ġ�� �����Ѵ�)
    [SerializeField] private GameObject _sMainObject;

    //�Ͽ�¡
    //���� ��
    [SerializeField] private GameObject _sRightFence;
    //���� ��
    [SerializeField] private GameObject _sLeftFence;

    [SerializeField] private GameObject _sBackObject;   //��ť���� �θ� ������Ʈ


    //�÷����� ������ ����Ʈ (�̹������� �ҷ��� �÷��� ����
    private List<Color> _colorList = new List<Color>();
    //����� ����� �÷����� ������ �������� ����
    private List<Block> _backBlockList = new List<Block>();

    void Start()
    {
        //�Ͽ�¡
        //��ũ�� ��ǥ���� ���ߴ� ��ǥ���� ������ǥ�� ���� ���Ѵ�
        Vector3 p1 = Camera.main.ScreenToWorldPoint(new Vector3(0f,Screen.height/2f, 10f));
        Debug.Log(p1);

        //�������� �������� ��ġ�� �����Ѵ�
        //���� ������ �ҽ��� ���� ���ؿͼ� �������ش�
        Vector3 RightFencePosition = _sRightFence.transform.position;
        Vector3 LeftFencePosition = _sLeftFence.transform.position;

        //���� ���� ������ ���� ��ġ���� ȭ�� ����� ���缭 ��ġ��Ų�� (������ ���� �������ش�)
        _sRightFence.transform.position = new Vector3(p1.x, RightFencePosition.y, 0f);
        _sLeftFence.transform.position = new Vector3( -p1.x, LeftFencePosition.y, 0f);
        
        // Create3Cube(,10, 10);
        PlayGame("char", "yellow_bird");
    }

    /// <summary>
    /// 3D CUBE ���带 �����Ѵ� (�����ġ�� �迭)
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    //���Ӻ���
    private GameObject[,] _board = null;
    private GameObject[,] _Backboard = null;
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

            foreach (var item in _Backboard)
            {
                //�麸�忡 ť�갡 ������ �����Ѵ�
                if(item!=null)
                {
                    Destroy(item);
                }
            }
        }

        _board = null;

        _Backboard = null;

        //���Ӻ��� �����Ѵ�
        _board = new GameObject[height, width];

        _Backboard= new GameObject[height, width];

        //ȭ�鿡 ���� ��µǴ� �̹��� ����� ����ϱ� ���� ���� ����
        //�̹��� ���Ͽ��� �ȼ��� ������ ������ �ȼ��� �����ϰ� ��µǴ� �̹����� ���ο� ������ ���ο� ������ ����ϱ� ���� ����
        //������ �ּҰ� ( �ֻ�� �÷��� ������ �ִ� ť���� ���� ��ġ)
        int columMin = height;
        //������ �ִ밪
        int columnMax = 0;
        //�������� (���� �ּҰ�)
        int roMin = width;
        //���� �ִ밪
        int roMax = 0;

        //���带 ��� �۾�
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //���ڷ� ���޵� �÷� ����Ʈ���� �ش���ġ�� �÷����� �����´�
                Color32 color = colors[i * width + j];

                //�÷���ť�� ���� ����
                //�������� ���� ���鸸 ����Ѵ�
                if(color.a != 0f)
                {
                    //�������� ť�긦 �����Ѵ�
                    GameObject obj = Instantiate(_sCubePrefab);
                    //ť�꿡 �̸��� �ο��Ѵ�
                    obj.name = $"Cube[{i},{j}]";
                    //�������� ���� ť�꿡 �θ� ������Ʈ�� �����Ѵ�
                    //(ť��_sMainObject ��ġ�� ��� �θ� �ڽ����� �����Ѵ�)
                    obj.transform.SetParent(_sMainObject.transform);

                        obj.tag = "CubeBlock"; // 2023-02-11 ( ����ĳ��Ʈ�� �ĺ��ϱ� ���ؼ� �±� �����Ѵ�)
                        Block block =obj.GetComponent<Block>();
                        block.OriginColor = color;
                        block.col = i;
                        block.row = j;

                    //ť���� ��ġ�� �����Ѵ�
                    obj.transform.position = new Vector3(j, i, 0.0f);
                    //����Ʈ�� �ִ� �÷����� �� ������Ʈ�� �÷��� �������ش� 
                    obj.GetComponent<MeshRenderer>().material.color = color;

                    //���Ӻ��忡 ������ ť���� �������� �߰��Ѵ�
                    _board[i, j] = obj;

                    //�÷� ����Ʈ�� �����
                    _colorList.Add(color);

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

                        //��ġ�� �÷����� ����Ѵ� ( �麸��󿡵� �÷����� ����س��� ���� �����ٳ��� �� 
                        //���� ������ �� �ִ�)
                        Block block = backObj.GetComponent<Block>();
                        block.OriginColor = color;
                        block.col = i;
                        block.row = j;


                    //�麸�� ť�긦 ���� �� ������� �ʴ� ������ٵ�� �ݶ��̴��� �����Ѵ�
                    //������Ʈ ����
                    Rigidbody rigidbody= backObj.GetComponent<Rigidbody>();
                    Destroy(rigidbody);
                    Collider collider = backObj.GetComponent<Collider>();
                    Destroy(collider);

                    //�θ� �����ϰ� ��ġ�� ��´�
                    backObj.transform.SetParent(_sBackObject.transform);
                    backObj.transform.position = new Vector3(j,i,0.0f);

                    //��������Ʈ �ϳ��ϳ� ����Ʈ�� �ִ´�(�麸����� ��������Ʈ ����Ʈ�� ����
                    //ť�꿡 �� ������Ʈ ����
                    _backBlockList.Add(backObj.GetComponent<Block>());

                    _Backboard[i,j] = backObj;

                }
            }
        }
        //ť�긦 ȭ�� ����� �����Ѵ�

        float midXvalue = width / 2f;
        float midYValue = height / 2f;

        
        for (int i = 0; i < height; i++)
        {
            
            for (int j = 0; j < width; j++)
            {
                //���Ӻ��忡 ť�갡 ������
                if (_board[i,j] != null)
                {
                    //������ �����ǰ��� �����´�
                    Vector3 calPosition = _board[i, j].transform.position;
                    //���� ����ִ� ��ġ�� �������ش� (�� ����� 1�̱� ������ 0.5 �� �������ش�)
                    _board[i, j].transform.position = 
                        new Vector3(calPosition.x - midXvalue + 0.5f, calPosition.y - midYValue + 0.5f, 0f);

                    //�麸����� ��ġ�� ����� �����ϰ� ����ش�
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
            //����Ƽ ����ü��� �ٲ۴�
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

        //���� ������ ������ ť���� �θ� ������Ʈ�� �����Ѵ�.(0,0,0)
        _sMainObject.transform.localScale = new Vector3(Xscale, Xscale, Xscale);
        //�麸�� ��ġ ���� ���� ������ ��ġ�� �����ϰ� �����.
        _sBackObject.transform.localScale = new Vector3(Xscale, Xscale, Xscale);

        //���� ���� ����� ����Ѵ�
        //���Ӻ������ �� ���� �����ϰ��� �� ������Ʈ�� �����
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

        //�ߺ��Ǵ� �÷��� ���� ����ũ�� ���� ������.
        //�÷�����Ʈ�� ����� �ߺ� �÷� ���� �����Ѵ�
        //System.Linq ��¡����� ��
        //�ߺ��Ǵ°� �����ϰ� �ٽ� ����Ʈ�� ������
        _colorList = _colorList.Distinct().ToList();

        //�麸���� �÷��� �ִ� �ε��� ���� ���� ���� ���� ���� ã�� �����Ѵ�.
        foreach (var item in _backBlockList)
        {
            int index = _colorList.FindIndex(x => x == item.OriginColor);
            //�麸�� �� �ε��� ���� �������� ���� �ѹ��� �ε��� ���� �ȴ�.
            item.BlockNumebr = index;
            //�麸�� �ε��� ���� �ѹ� �ؽ�Ʈ�� �ִ´�
            item.NumberText = index.ToString();
            //���� ���忡 �ִ� ������ �ؽ�Ʈ�� �ѹ����� �Ȱ��� �־��ش� 
            _board[item.col, item.row].GetComponent<Block>().NumberText = index.ToString();
            _board[item.col, item.row].GetComponent<Block>().BlockNumebr = index;

            //�麸����� ���� �÷���ȣ ���
            item.ShowNumberText(true);

        }

        //�麸����� ���� ��ġ���� ������ �����ǿ� �����Ѵ�
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
    /// �̹��� ������ ���ҽ� �������� �о�´� 
    /// ���ҽ� ���� �̸� , �ȼ� �̹����� �̸�
    /// </summary>
    /// <param name="categoryName"></param>
    /// <param name="imageName"></param>
    public void PlayGame(string categoryName, string imageName)
    {
        Texture2D texture = null;

        //�̹��� ��θ��� ����
        string PATH = "StageImages/" + categoryName + "/" + imageName;
        //���ҽ��� �������� �ؼ� �ȿ� �ִ� ��ü�� �ε��Ѵ�
        //�о�� ������ Ÿ�� as�� ������Ÿ�� ���� ��ȯ�Ѵ� 
        texture = Resources.Load(PATH,typeof(Texture2D)) as Texture2D;
        //�ȿ� �ִ� ���� ������ �����ͼ� ť�긦 �����
        BuildConvert3D(texture);
    }
    //����
    private int _CurrentColumn = 0;
    //����
    private int _CurrentRow = 0;
    /// <summary>
    /// �о�� �̹��� ������ �м��ؼ� ���� �ִ� 3D ���带 �����Ѵ�
    /// </summary>
    /// <param name="texture"></param>
    private void BuildConvert3D(Texture2D texture)
    {
        //�о�� �ؽ��ĸ� ���͸��� �����Ѵ�
        texture.filterMode = FilterMode.Point;

        var textureFormat = texture.format;
        //�ؽ��� ������ �츮�� ���ϴ� ������ �ƴϸ� ����� �ش�
        //���� ��� �׸� ���İ� 8����Ʈ���̴�.
        if( textureFormat != TextureFormat.RGBA32)
        {
            Debug.Log("32��Ʈ �÷��� ���");
        }
        //�о���� �̹������� ����� �ٸ��� ������(16x16 �Ǵ� 32x32�� �� ������
        //�о�� �̹����� ���� ���� ���� �����´�

        //������ �ȼ� ����
        int height = texture.height;
        //������ �ȼ� ����
        int width = texture.width;

        //�̹����� ���� ���� �� ����ؼ� ����� ����Ѵ�
        _CurrentColumn = height;
        _CurrentRow = width;

        //�ؽ��� �ȿ��� �ȼ� ������ �����´�(�ؽ��� �ȿ� �ִ� �ȼ� ���� ����)
        Color32[] colorBuffer = texture.GetPixels32();

        //�ȼ������� �о�´�
        var TextureColors = GeneratedColors(colorBuffer, height, width);

        Create3Cube(TextureColors, height, width);
    }
    /// <summary>
    /// �迭 ���� ����Ʈ�� ��ȯ�Ѵ�
    /// </summary>
    /// <param name="colorBuffer"></param>
    /// <param name="height"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    private List<Color32> GeneratedColors(Color32[] colorBuffer, int height, int width)
    {
        //�÷� ����Ʈ�� �����
        List<Color32> vertexColors = new List<Color32>(height * width);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Color32 c = colorBuffer[j + i * width];
                //�о���� �÷��� ����Ʈ�� �����Ѵ�
                vertexColors.Add(c);
            }
        }
        //�ش� ����Ʈ�� �����Ѵ�
        return vertexColors;
    }
    //�̹����� ��� �������̶�� �����ϰ� ��� ����� ����
    private float XBaseWidth = 500f;
    private float YBaseHeight = 500f;

    /// <summary>
    /// �� ��Ʈ���� �Լ�
    /// </summary>
    private void Crash()
    {
        foreach(GameObject obj in _board)
        {
            //���Ӻ���� ���尡 ���� �ƴϸ�
            if(obj != null)
            {
                //ť���� ������ ���� �����Ѵ�
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -0.001f);
                //������ üũ�� Ǯ���ش�
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                //�߷� ������ �ް� �Ѵ�
                obj.GetComponent<Rigidbody>().useGravity = true;

               Invoke("ResetColliderSize", 0.1f);
            }
        }
    }
    /// <summary>
    /// �浹�ڽ��� ������ ������ �μ��� ������ ���ֱ� ������
    /// 0.1�� �ڿ� �浹�ڽ��� ������� ������.
    /// </summary>
    private void ResetColliderSize()
    {
        foreach (var obj in _board)
        {
            if (obj != null)
            {
                //�ڽ��ݶ��̴��� ���� �ְ�
                var colli = obj.GetComponent<BoxCollider>();
                //������ ����� ������� �������´�.
                colli.size = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }
    private GameObject ReturnClickObject()
    {

        //��ũ����ǥ�踦 ������ǥ�� �ٲ� ������ ������ �� ����� ���ԵǴ��� �ƴ� ������� �ϱ� ����
        //����ĳ��Ʈ�� ȭ���� Ŭ���ؼ� ������ �������� ���� ������ �ִ� ������Ʈ�� ã�Ƴ���

        GameObject target = null;
        //ȭ���� Ŭ���������� ������ ���̿� ����
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //������ �������� ������ ã�´� (������Ÿ���� �迭�� �����Ѵ�) ��� �������� ��ŭ�� ���̸� �� ���� Ȯ��
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction * 10);
        //�迭 �ȿ��� �ϳ��� ������ ����
        foreach (var item in hits)
        {
            //�ݶ��̴��� ���� �ƴϰ� �±װ� ť�� ���� ù��° ���� Ÿ������ �����Ѵ�
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
    /// �巡�׵� ���� �÷����� �����ϴ� �麸����� ������ �ؽ�ƮĮ�� �����Ѵ�
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
    /// �麸����� ���� Ÿ�� ���� ��ġ���� �÷����� ��ġ�ϴ��� ����
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private GameObject IsMatchPositionColorBlock(GameObject target)
    {
        foreach (var item in _backBlockList)
        {
            //�������Ʈ�� �ִ� ������ ��ġ�� ���� ��ġ�ϴ��� �Ǵ��Ѵ�
            if(item.CurrentState == Block.STATE.FIXED &&item.CheckMatchPosition(target) && item.CheckMatchColor(target))
            {
                //��ġ�� �÷��� ��ġ�� ���� ���´� 
                return item.gameObject;
            }
        }
        return null;
    }
    //���콺 Ŭ�� �� ���õ� ��
    private GameObject _target = null;
    //���콺 Ŭ�� �� �巡�� �������� üũ
    private bool isMouseDrag = false;
    //Ŭ���� ��ġ ������
    //Ŭ���� ���� ��ġ�� �����ϴ� ��(y,z)
    private const float CLICKYOFFSET = 1.3f;
    private const float CLICKZOFFSET = -0.5f;
    private Vector3 _ScreenPosition;
    private Vector3 _offset;

    void Update()
    {
        //���콺 ���� ��ư ���� Ȯ��
        if (Input.GetMouseButtonDown(0))
        {
            //���콺�� Ŭ���ϰ� Ŭ���� ť����� �����´�
            _target = ReturnClickObject();

            //���ϵȰ� ���� �ƴϸ�? => Ŭ���� ���� �ִ�
            if (_target != null)
            {
                //Ŭ���� ť����� ������? => ���콺 �巡�׻��� true�� �������ش�
                isMouseDrag = true;

                //ť�긦 ���� �� �麸��� Ŭ���� ť���� �÷����� ���� �麸����� ���� �ؽ�Ʈ�� ���������� �����Ѵ�

                ChangeBlockColor(_target.GetComponent<Block>().BlockNumebr, Color.red);

                //Ŭ���� ������Ʈ���� ��ġ���� �����´�
                Vector3 clickObjectPosition = _target.transform.position;
                //Ŭ���� ������Ʈ���� ��ġ�� �������ش� ( ���� �������� �ű�) => ���������� �޾Ƽ� ������
                _target.transform.position = new Vector3(clickObjectPosition.x, clickObjectPosition.y, 0f);

                //ť����� �߷���(����������) ������ ���� �ʵ��� �Ѵ�.
                Destroy(_target.GetComponent<Rigidbody>());

                //���õ� ���� ȸ������ �ʱ�ȭ�Ͽ� ������ ���̵��� ó�� ( ���� Ŭ������ �� ������ ��������)
                _target.transform.localEulerAngles = new Vector3(0f, 0f, 0f);

                //���õ� ���� Ŭ���� ��ġ���� ���ʿ� ǥ�õǵ��� ó��
                //(����� ���� ��쿣 �հ������� Ŭ���ϱ� ������ ��ó�� Ŭ���ص� ���̰Բ� �Ѵ�)
                Vector3 pos = _target.transform.position;
                _target.transform.position = new Vector3(pos.x, pos.y + CLICKYOFFSET, pos.z + CLICKZOFFSET);

                //���õ� ���� �������� �����Ѵ� (Ŭ���� ���� �ٸ� ���� ����ȭ�ǰ� ũ�⸦ Ű���ش�)
                _target.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

                //���忡 �ִ� ť���� ��ũ������ ��ǥ���� ���Ѵ�
                _ScreenPosition = Camera.main.WorldToScreenPoint(_target.transform.position);
                //���� ���콺 Ŭ�� ��ġ��(��ũ��) ť���� ��ġ ���� �����¿� �����ϰ� �� ���̸�ŭ �������� �ε巴�� ���ش�
                _offset = _target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _ScreenPosition.z));

                //���콺�� Ŭ���� ���¿��� ����������
            }
        }
        //���콺 ��ư�� ���� �� Ȯ��
        if (Input.GetMouseButtonUp(0))
        {
            //���콺�� �������� �巡�� ���¸� Ǯ���ش� false
            isMouseDrag = false;
            //_target = null;

            if(_target != null)
            {
                //ChangeBlockColor(_target.GetComponent<Block>().BlockNumebr, Color.black);
                ChangeBlockColor(-1, Color.black);
                //���콺 Ŭ���� �����ߴ� ������ٵ� �߰��ؼ� ���� ������ �ش�
                _target.AddComponent<Rigidbody>();
                //Ȯ���� �����ϰ��� ���� �����ϰ����� �������ش�
                _target.transform.localScale = _target.GetComponent<Block>().OriginScale;
            }
            _target = null;

        }
        //���콺�� Ŭ���� ���¿��� ���콺�� �̵� �� ó��
        if(isMouseDrag)
        {
            //��ũ������ ��ġ���� �����´�
            Vector3 currentScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _ScreenPosition.z);
            //���� ��ġ + ������ ��ġ���� ���� (��ũ������ ��ġ���� ���尪���� �ٲٰ� ������ ��ġ �����Ѵ�)
            //�������� �ǹ� : ��Ȯ�� ���� �߾��� Ŭ������ �ʱ� ������ ��ũ���� ȭ���� �Ÿ��� �������ش�
            Vector3 currentPos = Camera.main.ScreenToWorldPoint(currentScreenPos) + _offset;

            //Ÿ���� �������� ����
            if (_target != null)
            {
                _target.transform.position = currentPos;

                //�巡�׵� ���� �̵��Ҷ����� ��ġ�ϴ� �麸����� ���� �ִ��� üũ
                GameObject matchObj = IsMatchPositionColorBlock(_target);
                //������ �麸�� ���� �������� �����Ѵ�
                if(matchObj != null)    //��ġ�ϴ°� ����
                {
                    //��ġ�� ť����� �麸��� ��ġ��Ų�� (���� ����)
                    _target.transform.position = matchObj.GetComponent<Block>().OriginPos;
                    //Ȯ��� ť���� ũ�⸦ ���� ũ��� �ٿ��ش�
                    _target.transform.localScale = _target.GetComponent<Block>().OriginScale;
                    //�麸�� ���� �ʿ� ���� ������ ��Ȱ��ȭ ���ش�
                    matchObj.SetActive(false);

                    //�ش� ��ġ�� ��Ī �Ǿ��ٰ� ó��
                    _target.GetComponent<Block>().CurrentState = Block.STATE.FIXED;
                    matchObj.GetComponent<Block>().CurrentState = Block.STATE.FIXED;

                    //Ÿ�ٺ��� �ݶ��̵� ����
                    Destroy(_target.GetComponent<Collider>());
                    //���������� �Ǿ��� Ÿ���� �̸��� �������� �������ش� (�÷��� �ʱ�ȭ)
                    ChangeBlockColor(_target.GetComponent<Block>().BlockNumebr, Color.black);

                    _target.GetComponent<Block>().MatchBlockAnimationStart();
                    _target = null;
                    isMouseDrag = false;
                }
            }
        }



    }
}
