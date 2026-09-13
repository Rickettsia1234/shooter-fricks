using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;

    private void Awake()
    {
        Instance = this;

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
        }

        bgmSource.loop = true;
        bgmSource.playOnAwake = false;
    }

    public void SetBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.loop = true;
    }

    public void PlayBGM()
    {
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PauseBGM()
    {
        bgmSource.Pause();
    }

    public void PlayOneBGM()
    {
        bgmSource.loop = false;
        bgmSource.Play();
    }
}