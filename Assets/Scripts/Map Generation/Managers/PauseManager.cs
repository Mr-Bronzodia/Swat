using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PauseManager : MonoBehaviour
{
    private bool _paused = false;
    private float _desiredVignetteAmount = .1f;
    private bool _processUpdates = true;

    [SerializeField]
    private Volume _volume;
    private Vignette _vignette;

    public static PauseManager Instance { get; private set; }

    public Action OnPauseStart;
    public Action OnPauseEnd;

    private void Awake()
    {
        if (_volume == null) Debug.LogError("volume not assigned in pause manager");

        _volume.profile.TryGet<Vignette>(out _vignette);

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameEnd += FinalPause;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameEnd -= FinalPause;
    }

    private void Pause()
    {
        OnPauseStart?.Invoke();
        _desiredVignetteAmount = .9f;

        GameManager.Instance.NoPause++;
    }

    private void FinalPause()
    {
        Pause();
        _processUpdates = false;
    }

    private void UnPause()
    {
        OnPauseEnd?.Invoke();
        _desiredVignetteAmount = .1f;
    }


    // Update is called once per frame
    void Update()
    {
        if (!_processUpdates) return;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            _paused = !_paused;

            if (_paused)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }

        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, _desiredVignetteAmount, Time.deltaTime);
    }
}
