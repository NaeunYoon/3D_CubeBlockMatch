using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/*
 ����Ƽ�� ������� Ŭ�����̱� ������ ������� �������ش� ( ������Ʈ�� ���� �ʴ´�
 
 */

public class CSV_FileLoad
{
    /// <summary>
    /// ��𼭳� ���ٰ����� �� �ְ� static���� �����
    /// </summary>
    /// <param name="fileName"></param>
    public static void OnLoadCSV(string fileName, List<StageData> stageData)
    {
        string filePath = "csv/";
        //���ڿ��� �����ִ� �Լ� Concat
        filePath = string.Concat(filePath, fileName);
        //���ҽ� ������ �ִ� ������ �ҷ��´�
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        //Ȯ�� ���
        Debug.Log("Text = " + textAsset.text);

        //���ڷ� ���� �������� �Ľ��� ���� �־��ش�
        OnLoadTextAsset(textAsset.text, stageData);

        Resources.UnloadAsset(textAsset);

        //��ó��
        textAsset = null;   
    }

    public static void OnLoadTextAsset(string data, List<StageData> stageData)
    {
        //�о�� �ؽ�Ʈ ���ڿ��� �� �ٲ� �������� ��´�
        //�ٹٲ� ���ڸ� �������� �߶� �迭�� ����
        string[] str_line = data.Split("\n");
        //�߸� ���� ��ǥ�� �������� �ؼ� �߶󳽴�

        //ù ���� �����ϰ� ���ڿ� �����͸� ��ǥ�� �������� �߶� �迭�� ���� 
        for (int i = 1; i < str_line.Length-1; i++)
        {
            //1,Test,Topspin,0,1
            string[] values = str_line[i].Split(",");
            /*
             values[0] = "1"            //����
             values[1] = "Test"         //���ڿ�
             values[2] = "TopSpin"      //���ڿ�
             values[3] = "0"            //����
             values[4] = "1"            //����
             */

            //���ڰ��� ���ڰ����� ��ȯ�Ѵ�
            StageData sd = new StageData(int.Parse(values[0]), 
                                        values[1], 
                                        values[2], 
                                        int.Parse(values[3]), 
                                        int.Parse(values[4]));

            //�Ľ��� �����͸� sd�� �־���
            stageData.Add(sd);

        }
    }
}
/// <summary>
/// �������� ������ �����ϴ� Ŭ���� 
/// </summary>
public class StageData
{
    //�������� ��ȣ
    public int StageNum { set; get; }

    //������
    public string CategoryName { set; get; }

    //�̹��� ���� �̸�
    public string ImageName { set; get; }

    //��ü �������� �÷��߿� �� ���� �÷� ���� ��������� ���� ��
    //��Ӻ� ����
    public int DropColorCount { set; get; }

    //�������� Ŭ����� ������ �ִϸ��̼� ��ȣ
    public int EndingAnimation { set; get; }

    /// <summary>
    /// �������� ������ ������Ƽ
    /// </summary>
    /// <param name="stageNum"></param>
    /// <param name="categoryName"></param>
    /// <param name="imageName"></param>
    /// <param name="dropColorCount"></param>
    /// <param name="endingAnimation"></param>
    public StageData(int stageNum, string categoryName, string imageName, int dropColorCount, int endingAnimation)
    {
        StageNum = stageNum;
        CategoryName = categoryName;
        ImageName = imageName;
        DropColorCount = dropColorCount;
        EndingAnimation = endingAnimation;
    }
}

