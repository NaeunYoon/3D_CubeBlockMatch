using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageView : MonoBehaviour
{
    //스테이지 데이터 저장용
    public List<StageData> stageData { get;  set; }

    //스테이지 셀 프리팹
    [SerializeField]
    private GameObject stageCellPrefab;

    [SerializeField]
    private Convert3D convert3D;

    //셀의 부모 오브젝트
    [SerializeField]
    private GameObject contents;
    void Start()
    {
        stageData = new List<StageData>();
        CSV_FileLoad.OnLoadCSV("StageDatas", stageData);
        MakeStageCells();
    }
        //불러온 데이터를 가지고 셀을 만듬

    private void MakeStageCells()
    {
        foreach (var item in stageData)
        {
            GameObject stageCellObj = Instantiate<GameObject>(stageCellPrefab);
            //이미지 경로를 생성
            string path = string.Format("StageImages/{0}/{1}", item.CategoryName, item.ImageName);
            //이미지 경로에서 이미지를 로드
            Sprite sprite = Resources.Load<Sprite>(path);

            StageCell stageCellImage = stageCellObj.GetComponent<StageCell>();
            //셀에 스프라이트 추가
            stageCellImage.AddStageImage(sprite);
            //셀에 이미지 이름 표시
            stageCellImage.SetText(item.ImageName);
            //카테고리이름
            stageCellImage.categoryName = item.CategoryName;
            //이미지 이름
            stageCellImage.imageName= item.ImageName;
            stageCellImage.dropCnt = item.DropColorCount;
            stageCellImage.stageView = this.gameObject;
            stageCellImage.convert3D = convert3D;

            stageCellImage.transform.localScale = Vector3.one;

            //부모설정
            stageCellImage.transform.SetParent(contents.transform);
        }

        //ui는 렉트 트랜스폼을 쓴다
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
