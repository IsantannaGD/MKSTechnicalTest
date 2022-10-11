using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.PlayerLoop;

public class GameController : MonoBehaviour
{
    private const string MainMenu = "MainMenu";
    private const string PlayAgain = "GamePlay";
    public delegate void SetGameSettingValue(float value);
    public SetGameSettingValue OnGameTimeChange;
    public SetGameSettingValue OnEnemyRespawnTimeChange;
    public delegate void GameEvents(float value);
    public GameEvents OnShootCannon;
    public GameEvents OnShootArtillery;
    public GameEvents OnKillEnemy;
    public GameEvents OnPlayerGetDamage;

    public Action<AudioClip> OnplaySfx;
    public Action<float> OnAudioVolumeChange;
    public Action<bool> OnGamePause;
    public Action OnGoingToGame;
    public Action OnGameStart;
    public Action OnPlayerDead;

    public static GameController Instance;

    [field: Header("** Game Settings **")]
    [field: Space]
    [field: SerializeField] public float AudioVolume { get; private set; }
    [field: SerializeField] public float GameTime { get; private set; }
    [field: SerializeField] public float RespawnTime { get; private set; }
    [field: SerializeField] public bool GamePaused { get; private set; }
    [field: SerializeField] public bool GameFinished { get; private set; }

    [Header("** Components **")]
    [Space]
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioSource _audioSource;

    public void GamePause()
    {
        GamePaused = !GamePaused;
        OnGamePause?.Invoke(GamePaused);

        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    public void PlayGameAgain()
    {
        GameFinished = false;
        SceneLoaderController.Instance.LoadSceneWithLoading(PlayAgain);
    }

    public void ReturnToMenu()
    {
        GameFinished = false;
        SceneLoaderController.Instance.LoadSceneWithLoading(MainMenu);
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Initialization();

        OnGameTimeChange += (value) => GameTime = value;
        OnEnemyRespawnTimeChange += (value) => RespawnTime = value;
        OnPlayerDead += () => GameFinished = true;
        OnplaySfx += PlaySfx;
        OnAudioVolumeChange += SetVolumeHandler;
        OnGameStart += GameStart;
        OnGoingToGame += SaveGamePreferences;
    }

    private void Initialization()
    {
        AudioVolume = PlayerPrefs.GetFloat("Volume", 0.8f);
        GameTime = PlayerPrefs.GetFloat("GameTime", 2f);
        RespawnTime = PlayerPrefs.GetFloat("RespawnTime", 0.5f);

        SetVolumeHandler(AudioVolume);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePause();
        }

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.T))
        {
            OnKillEnemy?.Invoke(1f);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            OnPlayerDead?.Invoke();
        }
#endif
    }

    private void SetVolumeHandler(float value)
    {
        if (value > 20f) value = 20f;
        if (value < -80f) value = -80f;

        AudioVolume = value;
        _audioMixer.SetFloat("Volume", value);

        PlayerPrefs.SetFloat("Volume", AudioVolume);
    }

    private void PlaySfx(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }

    private void SaveGamePreferences()
    {
        PlayerPrefs.SetFloat("GameTime", GameTime);
        PlayerPrefs.SetFloat("RespawnTime", RespawnTime);
    }

    private void GameStart()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        yield return new WaitForSeconds(GameTime * 60);

        OnPlayerDead?.Invoke();
    }
}
