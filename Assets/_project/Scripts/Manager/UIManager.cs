using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    private const string BestScorePassed = "You are amazing, nice!";
    private const string NoBestScore = "You are good, try again!";

    [Header("** Components **")]
    [Space]

    [SerializeField] private Image _cannonReloadingBar;
    [SerializeField] private Image _artilleryReloadingBar;
    [SerializeField] private GameObject _pauseBG;
    [SerializeField] private GameObject _pausePopup;
    [SerializeField] private GameObject _gameOverBg;
    [SerializeField] private GameObject _gameOverPopup;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _exitGameButton;
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private Button _returnToMenuButton;
    [SerializeField] private TextMeshProUGUI _bestScoreDisplayHUD;
    [SerializeField] private TextMeshProUGUI _currentScoreDisplayHUD;
    [SerializeField] private TextMeshProUGUI _bestScoreDisplayEndGame;
    [SerializeField] private TextMeshProUGUI _currentScoreDisplayEndGame;
    [SerializeField] private TextMeshProUGUI _endGameMessage;

    [Header("** Variable **")]
    [Space]
    
    [SerializeField] private float _animSpeed;
    [SerializeField] private float _bestScore;
    [SerializeField] private float _currentScore;
    [SerializeField] private Color _bestPassed;
    [SerializeField] private Color _bestNoPassed;
    private void Start()
    {
        Initialization();
        Subscribe();
    }

    private void Initialization()
    {
        _bestScore = PlayerPrefs.GetFloat("BestScore", 10f);
        _bestScoreDisplayHUD.text = $"{_bestScore} Pts";

        _resumeButton.onClick.AddListener(ResumeGame);
        _exitGameButton.onClick.AddListener(LeaveGame);
        _playAgainButton.onClick.AddListener(PlayAgainCallback);
        _returnToMenuButton.onClick.AddListener(GoToMainMenu);

        GameController.Instance.OnGameStart?.Invoke();
    }

    private void CannonShootReloadingBar(float reloadTime)
    {
        _cannonReloadingBar.DOFillAmount(1f, reloadTime).OnComplete(() => _cannonReloadingBar.fillAmount = 0);
    }

    private void ArtilleryShootReloadingBar(float reloadTime)
    {
        _artilleryReloadingBar.DOFillAmount(1f, reloadTime).OnComplete(() => _artilleryReloadingBar.fillAmount = 0);
    }

    private void ResumeGame()
    {
        GameController.Instance.GamePause();
    }

    private void LeaveGame()
    {
        {
#if UNITY_EDITOR

UnityEditor.EditorApplication.isPlaying = false;        
return;

#endif
            Application.Quit();
        }
    }

    private void GoToMainMenu()
    {
        GameController.Instance.ReturnToMenu();
    }

    private void PlayAgainCallback()
    {
        GameController.Instance.PlayGameAgain();
    }

    private void PauseGame(bool status)
    {
        if (status)
        {
            _pauseBG.SetActive(true);
            _pausePopup.transform.DOScale(1f, _animSpeed).SetUpdate(true);
        }
        else
        {
            _pausePopup.transform.DOScale(0f, _animSpeed).SetUpdate(true).OnComplete(() => _pauseBG.SetActive(false));
        }
    }

    private void GameOverCallback()
    {
        _gameOverBg.SetActive(true);
        _gameOverPopup.transform.DOScale(1f, _animSpeed).SetUpdate(true);

        _bestScoreDisplayEndGame.text = $"{_bestScore} Pts";
        _currentScoreDisplayEndGame.text = $"{_currentScore} Pts";

        if (_currentScore >= _bestScore)
        {
            _endGameMessage.text = BestScorePassed;
            _endGameMessage.color = _bestPassed;
        }
        else
        {
            _endGameMessage.text = NoBestScore;
            _endGameMessage.color = _bestNoPassed;
        }
    }

    private void UpdateScore(float value)
    {
        _currentScore += value;
        _currentScoreDisplayHUD.text = $"{_currentScore} Pts";

        if (_currentScore > _bestScore)
        {
            _bestScore = _currentScore;
            _bestScoreDisplayHUD.text = $"{_bestScore} Pts";
            PlayerPrefs.SetFloat("BestScore", _bestScore);
        }
    }

    private void Subscribe()
    {
        GameController.Instance.OnShootCannon += CannonShootReloadingBar;
        GameController.Instance.OnShootArtillery += ArtilleryShootReloadingBar;
        
        GameController.Instance.OnKillEnemy += UpdateScore;
        GameController.Instance.OnGamePause += PauseGame;
        GameController.Instance.OnPlayerDead += GameOverCallback;
    }

    private void Unsubscribe()
    {
        GameController.Instance.OnShootCannon -= CannonShootReloadingBar;
        GameController.Instance.OnShootArtillery -= ArtilleryShootReloadingBar;
        
        GameController.Instance.OnKillEnemy -= UpdateScore;
        GameController.Instance.OnGamePause -= PauseGame;
        GameController.Instance.OnPlayerDead -= GameOverCallback;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }
}
