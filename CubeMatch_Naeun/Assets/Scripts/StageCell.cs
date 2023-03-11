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
    /// ���������� ���Ǵ� �̹��� ���
    /// </summary>
    public void AddStageImage(Sprite sprite)
    {
        stageImage.sprite = sprite;
    }
    /// <summary>
    /// ���������� ���Ǵ� �̹��� �̸� ���
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
