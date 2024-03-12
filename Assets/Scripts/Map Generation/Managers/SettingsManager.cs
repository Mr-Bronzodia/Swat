using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    public Options Settings { get; private set; }

    public Action OnSettingChanged { get; set; }

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            ReadPrefs();

        }
    }

    public struct Options
    {
        public int Difficulty;
        public float Volume;
        public bool ShowHUD;
        public float CamMovSpeed;
        public float CamRootSpeed;
        public float CamZoomSpeed;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            ReadPrefs();
            OnSettingChanged?.Invoke();
        }
    }

    private void ReadPrefs()
    {
        Options options = new Options();
        options.Difficulty = PlayerPrefs.GetInt("NormalDifficulty");
        options.Volume = PlayerPrefs.GetFloat("MusicVolume");
        int showHud = PlayerPrefs.GetInt("ShowHUD");
        options.ShowHUD = showHud == 0 ? false : true;
        options.CamMovSpeed = PlayerPrefs.GetFloat("XSensitivity");
        options.CamRootSpeed = PlayerPrefs.GetFloat("YSensitivity");
        options.CamZoomSpeed = PlayerPrefs.GetFloat("MouseSmoothing");

        Settings = options;
    }
}
