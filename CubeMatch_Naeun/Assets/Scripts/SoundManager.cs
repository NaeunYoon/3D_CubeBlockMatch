using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//반드시 필요한 컴포넌트는 자동으로 추가해준다
[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _effectSound;

    [SerializeField]
    private AudioClip _backgroundSound;

    private static SoundManager _instance;

    private AudioSource audio;
    private AudioSource background;

    public static SoundManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject project = (GameObject)Instantiate((GameObject)Resources.Load("Manager/SoundManager"));
                _instance = project.GetComponent<SoundManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        audio = gameObject.GetComponent<AudioSource>();
        BackgroundMusic();
    }

    public void PlayBtnClick()
    {
        audio.PlayOneShot(_effectSound[0]);
    }

    public void Play_BlockClick()
    {
        audio.PlayOneShot(_effectSound[1]);
    }
    public void Congreturation()
    {
        audio.PlayOneShot(_effectSound[2]);
    }

    public void BackgroundMusic()
    {
        audio.clip = _backgroundSound;
        audio.loop = true;
        audio.Play(0);

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
