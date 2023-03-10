using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


/*
 유니티와 상관없는 클래스이기 때문에 모노비헤비어를 제거해준다 ( 컴포넌트로 쓰지 않는다
 
 */

public class CSV_FileLoad
{
    /// <summary>
    /// 어디서나 접근가능할 수 있게 static으로 만든다
    /// </summary>
    /// <param name="fileName"></param>
    public static void OnLoadCSV(string fileName, List<StageData> stageData)
    {
        string filePath = "csv/";
        //문자열을 합쳐주는 함수 Concat
        filePath = string.Concat(filePath, fileName);
        //리소스 폴더에 있는 파일을 불러온다
        TextAsset textAsset = Resources.Load<TextAsset>(filePath);
        //확인 출력
        Debug.Log("Text = " + textAsset.text);

        //인자로 들어온 참조값에 파싱한 값을 넣어준다
        OnLoadTextAsset(textAsset.text, stageData);

        Resources.UnloadAsset(textAsset);

        //널처리
        textAsset = null;   
    }

    public static void OnLoadTextAsset(string data, List<StageData> stageData)
    {
        //읽어온 텍스트 문자열을 줄 바꿈 기준으로 잡는다
        //줄바꿈 문자를 기준으로 잘라서 배열에 넣음
        string[] str_line = data.Split("\n");
        //잘린 값을 쉽표를 기준으로 해서 잘라낸다

        //첫 줄을 제외하고 문자열 데이터를 쉼표를 기준으로 잘라서 배열에 저장 
        for (int i = 1; i < str_line.Length-1; i++)
        {
            //1,Test,Topspin,0,1
            string[] values = str_line[i].Split(",");
            /*
             values[0] = "1"            //숫자
             values[1] = "Test"         //문자열
             values[2] = "TopSpin"      //문자열
             values[3] = "0"            //숫자
             values[4] = "1"            //숫자
             */

            //문자값을 숫자값으로 변환한다
            StageData sd = new StageData(int.Parse(values[0]), 
                                        values[1], 
                                        values[2], 
                                        int.Parse(values[3]), 
                                        int.Parse(values[4]));

            //파싱한 데이터를 sd에 넣었음
            stageData.Add(sd);

        }
    }
}
/// <summary>
/// 스테이지 데이터 저장하는 클래스 
/// </summary>
public class StageData
{
    //스테이지 번호
    public int StageNum { set; get; }

    //폴더명
    public string CategoryName { set; get; }

    //이미지 파일 이름
    public string ImageName { set; get; }

    //전체 스테이지 컬러중에 몇 개의 컬러 블럭을 남길건지에 대한 값
    //드롭블럭 갯수
    public int DropColorCount { set; get; }

    //스테이지 클리어시 보여줄 애니매이션 번호
    public int EndingAnimation { set; get; }

    /// <summary>
    /// 스테이지 데이터 프로퍼티
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

