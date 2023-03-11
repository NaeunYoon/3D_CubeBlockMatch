using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitView : MonoBehaviour
{
    [SerializeField]
    private Button Stbtn;
    [SerializeField]
    private Button Opbtn;
    [SerializeField]
    private GameObject StageView;
    [SerializeField]
    private GameObject OptionView;

    public void OnClickStartBtn()
    {
        SoundManager.Instance.PlayBtnClick();
        this.gameObject.SetActive(false);
        StageView.SetActive(true);
    }

    public void OnClickOptiontBtn()
    {
        OptionView.gameObject.SetActive(true);
    }

    public void OnClickExitOptiontBtn()
    {
        OptionView.gameObject.SetActive(false);
    }

}
