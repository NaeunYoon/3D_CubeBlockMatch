using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Score : MonoBehaviour
{
    public Text score;
    public int scoreValue;
    public Text name;
    public static string BlockName;

    Convert3D convert;
    private void Start()
    {
        convert = GetComponent<Convert3D>();
        score = GetComponent<Text>();
        name = GetComponent<Text>();
    }
    private void Update()
    {
        
    }

}
