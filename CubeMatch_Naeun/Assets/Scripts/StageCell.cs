using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageCell : MonoBehaviour
{
    [SerializeField]
    private Image stageImage;

    [SerializeField]
    private Text stageText;

    [HideInInspector]
    public Convert3D convert3D;

    [HideInInspector]
    public GameObject stageView;

    [HideInInspector]
    public string categoryName;

    [HideInInspector]
    public int dropCnt;

    [HideInInspector]
    public string imageName;


    /// <summary>
    /// 스테이지에 사용되는 이미지 출력
    /// </summary>
    public void AddStageImage(Sprite sprite)
    {
        stageImage.sprite = sprite;
    }
    /// <summary>
    /// 스테이지에 사용되는 이미지 이름 출력
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        stageText.text = text;
    }

    public void OnclickCell()
    {
        stageView.SetActive(false);
        convert3D.PlayGame(categoryName, imageName);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
