using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageView : MonoBehaviour
{
    //�������� ������ �����
    public List<StageData> stageData { get;  set; }

    //�������� �� ������
    [SerializeField]
    private GameObject stageCellPrefab;

    [SerializeField]
    private Convert3D convert3D;

    //���� �θ� ������Ʈ
    [SerializeField]
    private GameObject contents;
    void Start()
    {
        stageData = new List<StageData>();
        CSV_FileLoad.OnLoadCSV("StageDatas", stageData);
        MakeStageCells();
    }
        //�ҷ��� �����͸� ������ ���� ����

    private void MakeStageCells()
    {
        foreach (var item in stageData)
        {
            GameObject stageCellObj = Instantiate<GameObject>(stageCellPrefab);
            //�̹��� ��θ� ����
            string path = string.Format("StageImages/{0}/{1}", item.CategoryName, item.ImageName);
            //�̹��� ��ο��� �̹����� �ε�
            Sprite sprite = Resources.Load<Sprite>(path);

            StageCell stageCellImage = stageCellObj.GetComponent<StageCell>();
            //���� ��������Ʈ �߰�
            stageCellImage.AddStageImage(sprite);
            //���� �̹��� �̸� ǥ��
            stageCellImage.SetText(item.ImageName);
            //ī�װ��̸�
            stageCellImage.categoryName = item.CategoryName;
            //�̹��� �̸�
            stageCellImage.imageName= item.ImageName;
            stageCellImage.dropCnt = item.DropColorCount;
            stageCellImage.stageView = this.gameObject;
            stageCellImage.convert3D = convert3D;

            stageCellImage.transform.localScale = Vector3.one;

            //�θ���
            stageCellImage.transform.SetParent(contents.transform);
        }

        //ui�� ��Ʈ Ʈ�������� ����
        Vector3 pos = contents.GetComponent<RectTransform>().anchoredPosition;
        int cnt = stageData.Count;
        float ypos = cnt / 2 * 250;
        ypos += 500;
        contents.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x,-ypos);
    }



    void Update()
    {
        
    }
}
