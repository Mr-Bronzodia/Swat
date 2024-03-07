using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private AudioSource _audioSourcePrefab;

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

        }
    }

    public void PlaySoundAtPoint(AudioClip clip, Vector3 point)
    {
        AudioSource audioSource = Instantiate(_audioSourcePrefab, point, Quaternion.identity);

        audioSource.clip = clip;

        Destroy(audioSource.gameObject, clip.length);

        audioSource.volume = _effectVolume;

        audioSource.Play();
    }
}
