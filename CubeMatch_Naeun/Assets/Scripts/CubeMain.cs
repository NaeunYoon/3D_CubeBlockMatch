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
    /// �� ������Ʈ�� �Ѿ��ִ� �ִϸ��̼��� ���� ( �ִϸ��̼� �̺�Ʈ �Լ�)
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
