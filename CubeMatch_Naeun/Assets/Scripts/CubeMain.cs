using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMain : MonoBehaviour
{
    [SerializeField]
    private Convert3D convert3D;

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
        this.gameObject.GetComponent<Animator>().enabled = false;
        convert3D.PlayNextStage();
    }

}
