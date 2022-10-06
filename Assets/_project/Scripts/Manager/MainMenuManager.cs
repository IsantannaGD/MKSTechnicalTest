using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private const string GameScene = "GamePlay";

    [Header("** Settings Controllers **")]
    [Space]

    [SerializeField] private Button _audioPlus;
    [SerializeField] private Button _audioLess;
    [SerializeField] private Slider _audioBar;
    [SerializeField] private Button _gameTimePlus;
    [SerializeField] private Button _gameTimeLess;
    [SerializeField] private TMP_InputField _gameTimeInputField;
    [SerializeField] private Button _respawnTimePlus;
    [SerializeField] private Button _respawnTimeLess;
    [SerializeField] private TMP_InputField _respawnTimeInputField;
    [SerializeField] private Button _detectionAreaPlus;
    [SerializeField] private Button _detectionAreaLess;
    [SerializeField] private TMP_InputField _detectionAreaInputField;

    [Header("** Buttons **")]
    [Space]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _backButton;

    [Header("** Components **")]
    [Space]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private float _maxGameTime = 5f;
    [SerializeField] private float _maxRespawnTime = 2f;
    [SerializeField] private float _maxDetectionArea = 4f;
    [SerializeField] private float _minGameTime = 1f;
    [SerializeField] private float _minRespawnTime = 0.3f;
    [SerializeField] private float _minDetectionArea = 1f;

    [Header("** Audio Components **")]
    [Space]
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private AudioClip _buttonSound;
    [SerializeField] private AudioClip _backSound;

    [Header("** Variable **")]
    [Space]
    [SerializeField] private float _openFirstPanelSpeed;
    [SerializeField] private Color _selectedColor;
    [SerializeField] private float _audioVolume;
    [SerializeField] private float _gameTime;
    [SerializeField] private float _respawnTime;
    [SerializeField] private float _detectionArea;

    private void Start()
    {
        Initializations();

        _playButton.onClick.AddListener(PlayGameHandler);
        _settingsButton.onClick.AddListener(MainPanelInteractionAnim);
        _backButton.onClick.AddListener(MainPanelInteractionAnim);
        _exitButton.onClick.AddListener(ExitGame);

        _audioBar.onValueChanged.AddListener(ChangeVolumeInBar);
        _audioPlus.onClick.AddListener(IncreaseAudioVolume);
        _audioLess.onClick.AddListener(DecreaseAudioVolume);

        _gameTimeInputField.onSubmit.AddListener(ChangeGameTimeInInputField);
        _gameTimePlus.onClick.AddListener(IncreaseGameTime);
        _gameTimeLess.onClick.AddListener(DecreaseGameTime);

        _respawnTimeInputField.onSubmit.AddListener(ChangeRespawnTimeInInputField);
        _respawnTimePlus.onClick.AddListener(IncreaseRespawnTime);
        _respawnTimeLess.onClick.AddListener(DecreaseRespawnTime);

        _detectionAreaInputField.onSubmit.AddListener(ChangeDetectionAreaInInputField);
        _detectionAreaPlus.onClick.AddListener(IncreaseDetectionArea);
        _detectionAreaLess.onClick.AddListener(DecreaseDetectionArea);
    }

    private void Initializations()
    {
        _audioVolume = GameController.Instance.AudioVolume;
        _gameTime = GameController.Instance.GameTime;
        _respawnTime = GameController.Instance.RespawnTime;
        _detectionArea = GameController.Instance.DetectionArea;

        _audioBar.value = _audioVolume;
        _gameTimeInputField.text = _gameTime.ToString();
        _respawnTimeInputField.text = _respawnTime.ToString();
        _detectionAreaInputField.text = _detectionArea.ToString();
    }

    private void PlayGameHandler()
    {
        GameController.Instance.OnGoingToGame?.Invoke();
        SceneLoaderController.Instance.LoadSceneWithLoading(GameScene);
    }

    private void ExitGame()
    {
#if UNITY_EDITOR

UnityEditor.EditorApplication.isPlaying = false;        
return;

#endif
        Application.Quit();
    }

    private void MainPanelInteractionAnim()
    {
        if (!_mainPanel.activeSelf)
        {
            _mainPanel.SetActive(true);
            _settingsButton.targetGraphic.color = _selectedColor;
            _mainPanel.transform.DOScale(1f, _openFirstPanelSpeed);
        }
        else
        {
            _mainPanel.transform.DOScale(0f, _openFirstPanelSpeed).OnComplete(() =>
            {
                _mainPanel.SetActive(false);
                _settingsButton.targetGraphic.color = Color.white;
            });
        }
    }

    private void ChangeVolumeInBar(float value)
    {
        _audioVolume = value;

        GameController.Instance.OnAudioVolumeChange?.Invoke(_audioVolume);
    }

    private void IncreaseAudioVolume()
    {
        if (_audioVolume >= 20f)
        {
            return;
        }

        _audioVolume += 10f;
        _audioBar.value = _audioVolume;

        GameController.Instance.OnAudioVolumeChange?.Invoke(_audioVolume);
    }

    private void DecreaseAudioVolume()
    {
        if (_audioVolume <= -80f)
        {
            return;
        }

        _audioVolume -= 10f;
        _audioBar.value = _audioVolume;

        GameController.Instance.OnAudioVolumeChange?.Invoke(_audioVolume);
    }

    private void ChangeGameTimeInInputField(string valueAsString)
    {
        float updatedValueForm = float.Parse(valueAsString);

        if (updatedValueForm < _minGameTime)
        {
            updatedValueForm = _minGameTime;
            _gameTimeInputField.text = updatedValueForm.ToString();
        }

        if (updatedValueForm > _maxGameTime)
        {
            updatedValueForm = _maxGameTime;
            _gameTimeInputField.text = updatedValueForm.ToString();
        }

        _gameTime = updatedValueForm;

        GameController.Instance.OnGameTimeChange?.Invoke(_gameTime);
    }

    private void IncreaseGameTime()
    {
        if (_gameTime >= _maxGameTime)
        {
            return;
        }

        _gameTime += 0.5f;
        if (_gameTime > _maxGameTime) _gameTime = _maxGameTime;
        
        _gameTimeInputField.text = _gameTime.ToString();

        GameController.Instance.OnGameTimeChange?.Invoke(_gameTime);
    }

    private void DecreaseGameTime()
    {
        if (_gameTime <= _minGameTime)
        {
            return;
        }

        _gameTime -= 0.5f;
        if (_gameTime < _minGameTime) _gameTime = _minGameTime;
        
        _gameTimeInputField.text = _gameTime.ToString();

        GameController.Instance.OnGameTimeChange?.Invoke(_gameTime); 
    }

    private void ChangeRespawnTimeInInputField(string valueAsString)
    {
        float updatedValueForm = float.Parse(valueAsString);

        if (updatedValueForm > _maxRespawnTime)
        {
            updatedValueForm = _maxGameTime;
            _respawnTimeInputField.text = updatedValueForm.ToString();
        }

        if (updatedValueForm < _minRespawnTime)
        {
            updatedValueForm = _minRespawnTime;
            _respawnTimeInputField.text = updatedValueForm.ToString();
        }

        _respawnTime = updatedValueForm;

        GameController.Instance.OnEnemyRespawnTimeChange?.Invoke(_respawnTime);
    }

    private void IncreaseRespawnTime()
    {
        if (_respawnTime >= _maxRespawnTime)
        {
            return;
        }

        _respawnTime += 0.5f;
        if (_respawnTime > _maxRespawnTime) _respawnTime = _maxRespawnTime;

        _respawnTimeInputField.text = _respawnTime.ToString();

        GameController.Instance.OnEnemyRespawnTimeChange?.Invoke(_respawnTime);
    }

    private void DecreaseRespawnTime()
    {
        if (_respawnTime <= _minRespawnTime)
        {
            return;
        }

        _respawnTime -= 0.5f;
        if (_respawnTime < _minRespawnTime) _respawnTime = _minRespawnTime;

        _respawnTimeInputField.text = _respawnTime.ToString();

        GameController.Instance.OnEnemyRespawnTimeChange?.Invoke(_respawnTime);
    }

    private void ChangeDetectionAreaInInputField(string valueAsString)
    {
        float updatedValueForm = float.Parse(valueAsString);

        if (updatedValueForm > _maxDetectionArea)
        {
            updatedValueForm = _maxDetectionArea;
            _detectionAreaInputField.text = updatedValueForm.ToString();
        }

        if (updatedValueForm < _minDetectionArea)
        {
            updatedValueForm = _minDetectionArea;
            _detectionAreaInputField.text = updatedValueForm.ToString();
        }

        _detectionArea = updatedValueForm;

        GameController.Instance.OnDetectionAreaChange?.Invoke(_detectionArea);
    }

    private void IncreaseDetectionArea()
    {
        if (_detectionArea >= _maxDetectionArea)
        {
            return;
        }

        _detectionArea += 0.5f;
        if (_detectionArea > _maxDetectionArea) _detectionArea = _maxDetectionArea;

        _detectionAreaInputField.text = _detectionArea.ToString();

        GameController.Instance.OnDetectionAreaChange?.Invoke(_detectionArea);
    }

    private void DecreaseDetectionArea()
    {
        if (_detectionArea <= _minDetectionArea)
        {
            return;
        }

        _detectionArea -= 0.5f;
        if (_detectionArea < _minDetectionArea) _detectionArea = _minDetectionArea;

        _detectionAreaInputField.text = _detectionArea.ToString();

        GameController.Instance.OnDetectionAreaChange?.Invoke(_detectionArea);
    }
}
