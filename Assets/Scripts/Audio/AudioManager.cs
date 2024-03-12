using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private AudioSource _audioSourcePrefab;

    [SerializeField]
    private AudioMixer _audioMixer;

    private float _effectVolume = 1.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            SetVolume();
            Debug.Log(Mathf.Log(SettingsManager.Instance.Settings.Volume) * 20);
        }
    }

    private void OnEnable()
    {
        SettingsManager.Instance.OnSettingChanged += SetVolume;
    }

    private void OnDisable()
    {
        SettingsManager.Instance.OnSettingChanged -= SetVolume;
    }

    private void SetVolume() => _audioMixer.SetFloat("MasterVolume", Mathf.Log(SettingsManager.Instance.Settings.Volume) * 20);


    public void PlaySoundAtPoint(AudioClip clip, Vector3 point)
    {
        AudioSource audioSource = Instantiate(_audioSourcePrefab, point, Quaternion.identity);

        audioSource.clip = clip;

        Destroy(audioSource.gameObject, clip.length);

        audioSource.volume = _effectVolume;

        audioSource.Play();
    }
}
