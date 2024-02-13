using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Serialization;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class PauseManager : MonoBehaviour
{
    private bool _paused = false;
    private float _desiredVinetteAmount = .1f;

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


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            _paused = !_paused;

            if (_paused)
            {
                OnPauseStart?.Invoke();
                _desiredVinetteAmount = .7f;
            }
            else
            {
                OnPauseEnd?.Invoke();
                _desiredVinetteAmount = .1f;
            }
        }

        _vignette.intensity.value = Mathf.Lerp(_vignette.intensity.value, _desiredVinetteAmount, Time.deltaTime);
    }
}
