using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : Singleton<SoundManager>
{

    AudioSource BGMSource;
    AudioSource SFXSource;

    private Dictionary<BGMType, AudioClip> BGMDic = new Dictionary<BGMType, AudioClip>();
    private Dictionary<SFXType, AudioClip> SFXDic = new Dictionary<SFXType, AudioClip>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void InitSoundManager()
    {
        GameObject obj = new GameObject("SoundManager");
        Instance=obj.AddComponent<SoundManager>();
        DontDestroyOnLoad(obj);

        GameObject bgmObj = new GameObject("BGM");
        SoundManager.Instance.BGMSource = bgmObj.AddComponent<AudioSource>();
        bgmObj.transform.SetParent(obj.transform);

        GameObject sfxObj = new GameObject("SFX");
        SoundManager.Instance.SFXSource = sfxObj.AddComponent<AudioSource>();
        sfxObj.transform.SetParent(obj.transform);

        AudioClip[] BGMClips = Resources.LoadAll<AudioClip>("Sound/BGM");
        foreach(AudioClip clip in BGMClips)
        {
            try
            {
                BGMType type = (BGMType)Enum.Parse(typeof(BGMType), clip.name);
                SoundManager.Instance.BGMDic.Add(type, clip);
            }
            catch
            {
                Debug.LogWarning("bgm enum 필요 : " + clip.name);
            }
        }
        AudioClip[] SFXClips = Resources.LoadAll<AudioClip>("Sound/SFX");
        foreach (AudioClip clip in BGMClips)
        {
            try
            {
                SFXType type = (SFXType)Enum.Parse(typeof(SFXType), clip.name);
                SoundManager.Instance.SFXDic.Add(type, clip);
            }
            catch
            {
                Debug.LogWarning("bgm enum 필요 : " + clip.name);
            }
        }
    }
    public void PlayBGM(BGMType type)
    {
        BGMSource.Play();
    }
    public void PlaySFX(SFXType type)
    {
        SFXSource.PlayOneShot(SFXDic[type]);
    }
}
public enum BGMType
{

}
public enum SFXType
{

}
