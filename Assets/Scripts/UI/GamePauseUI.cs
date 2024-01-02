using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;

    private void Awake()
    {
        _resumeButton.onClick.AddListener(() =>
        {
            KitchenGameManager.Instance.TogglePauseGame();
        });
        _mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        KitchenGameManager.Instance.OnGamePause += KitchenGameManager_OnGamePause;
        KitchenGameManager.Instance.OnGameUnPause += KitchenGameManager_OnGameUnPause;

        Hide();
    }

    private void KitchenGameManager_OnGameUnPause(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void KitchenGameManager_OnGamePause(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
