using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;

public class GameController : MonoBehaviour
{
    public delegate float SetGameSettingValue(float value);
    public SetGameSettingValue OnGameTimeChange;
    public SetGameSettingValue OnEnemyRespawnTimeChange;
    public SetGameSettingValue OnDetectionAreaChange;

    public Action<float> OnAudioVolumeChange;
    public Action OnGoingToGame;

    public static GameController Instance;

    [field: Header("** Game Settings **")]
    [field: Space]
    [field: SerializeField] public float AudioVolume { get; private set; }
    [field: SerializeField] public float GameTime { get; private set; }
    [field: SerializeField] public float RespawnTime { get; private set; }
    [field: SerializeField] public float DetectionArea { get; private set; }

    [Header("** Components **")]
    [Space]
    [SerializeField] private AudioMixer _audioMixer;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialization();

        OnGameTimeChange += (value) => GameTime = value;
        OnEnemyRespawnTimeChange += (value) => RespawnTime = value;
        OnDetectionAreaChange += (value) => DetectionArea = value;
        OnAudioVolumeChange += SetVolumeHandler;
        OnGoingToGame += SaveGamePreferences;
    }

    private void Initialization()
    {
        AudioVolume = PlayerPrefs.GetFloat("Volume", 0.8f);
        GameTime = PlayerPrefs.GetFloat("GameTime", 2f);
        RespawnTime = PlayerPrefs.GetFloat("RespawnTime", 0.5f);
        DetectionArea = PlayerPrefs.GetFloat("DetectionArea", 2f);

        SetVolumeHandler(AudioVolume);
    }

    private void SetVolumeHandler(float value)
    {
        if (value > 20f) value = 20f;
        if (value < -80f) value = -80f;

        AudioVolume = value;
        _audioMixer.SetFloat("Volume", value);

        PlayerPrefs.SetFloat("Volume", AudioVolume);
    }

    private void SaveGamePreferences()
    {
        PlayerPrefs.SetFloat("GameTime", GameTime);
        PlayerPrefs.SetFloat("RespawnTime", RespawnTime);
        PlayerPrefs.SetFloat("DetectionArea", DetectionArea);
    }
}
