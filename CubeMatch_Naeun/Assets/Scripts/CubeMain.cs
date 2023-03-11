using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMain : MonoBehaviour
{
    [SerializeField]
    private Convert3D convert3D;

    [SerializeField]
    private GameObject stageView;

    [SerializeField]
    private GameObject gameView;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    /// <summary>
    /// 이 오브젝트에 붇어있는 애니메이션을 끈다 ( 애니메이션 이벤트 함수)
    /// </summary>
    public void PlayNexStage()
    {
        StartCoroutine("Delay");
        //gameView.SetActive(false);
        //stageView.SetActive(true);
        //this.gameObject.GetComponent<Animator>().enabled = false;
        //convert3D.PlayNextStage();
    }

    public IEnumerator Delay()
    {
        yield return new WaitForSeconds(3f);
        gameView.SetActive(false);
        stageView.SetActive(true);
        this.gameObject.GetComponent<Animator>().enabled = false;

    }


}
