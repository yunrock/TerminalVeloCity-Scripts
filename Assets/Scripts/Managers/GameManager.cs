/*
 * Author: Yunuén Vladimir
 * Project: Terminal Velo.City
 * Website: https://yunuenvladimir.itch.io
 * License: GNU GPLv3
 * Tools: Unity 6, C#, PHP, SQL
 *
 * This script is part of the Terminal Velo.City project.
 * Redistribution and modification are allowed under the terms of the GNU General Public License v3.
 */
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Manages the overall game state and scene transitions.
/// Implements a singleton pattern to ensure only one instance persists across scenes.
/// </summary>
public class GameManager : MonoBehaviour {
    /// <summary>
    /// Singleton instance of the GameManager.
    /// </summary>
    public static GameManager instance {
        get; private set;
    }

    /// <summary>
    /// Current active state of the game.
    /// </summary>
    public GameState currentState {
        get; private set;
    }
    /// <summary>
    /// Previous state before the last state switch (used for returning from Options).
    /// </summary>
    private GameState previousState;

    [Header("Scene Management")]
    /// <summary>
    /// Name of the gameplay scene to be loaded additively.
    /// </summary>
    [SerializeField] private string m_gameplaySceneName = "GameScene";

    private void Awake () {
        SingletonSetup();
    }

    /// <summary>
    /// Ensures the GameManager follows the singleton pattern and persists across scenes.
    /// </summary>
    private void SingletonSetup () {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start () {
        // Initializes the game by setting the state to MainMenu.
        SwitchState(GameState.MainMenu);
    }

    /// <summary>
    /// Changes the current game state and triggers corresponding UI and audio behavior.
    /// </summary>
    /// <param name="newState">The state to switch to.</param>
    public void SwitchState (GameState newState) {
        if (currentState == newState) {
            return;
        }

        currentState = newState;

        if (newState != GameState.Options) {
            previousState = currentState;
        }

        switch (newState) {
            case GameState.MainMenu:
                OnMainMenu();
                break;
            case GameState.Playing:
                OnPlaying();
                break;
            case GameState.Paused:
                OnPause();
                break;
            case GameState.PreGameOver:
                OnPreGameOver();
                break;
            case GameState.GameOver:
                OnGameOver();
                break;
            case GameState.Options:
                OnOptions();
                break;
            case GameState.Rankings:
                OnRankings();
                break;
        }
    }

    /// <summary>
    /// Begins the game by loading the gameplay scene.
    /// </summary>
    public void StartGame () {
        StartCoroutine(LoadGameplayAdditive());
    }

    /// <summary>
    /// Restarts the game by unloading and reloading the gameplay scene.
    /// </summary>
    public void RestartGame () {
        StartCoroutine(RestartGameRoutine());
    }

    /// <summary>
    /// Pauses the game if currently in Playing state.
    /// </summary>
    public void PauseGame () {
        if (currentState == GameState.Playing) {
            SwitchState(GameState.Paused);
        }
    }

    /// <summary>
    /// Resumes the game if currently paused.
    /// </summary>
    public void ResumeGame () {
        if (currentState == GameState.Paused) {
            SwitchState(GameState.Playing);
            AudioManager.instance.PlayGameplayMusic();
        }
    }

    /// <summary>
    /// Transitions to the PreGameOver state.
    /// </summary>
    public void PreGameOver () {
        SwitchState(GameState.PreGameOver);
    }

    /// <summary>
    /// Transitions to the GameOver state.
    /// </summary>
    public void GameOver () {
        SwitchState(GameState.GameOver);
    }

    /// <summary>
    /// Opens the Options menu state.
    /// </summary>
    public void OpenOptions () {
        SwitchState(GameState.Options);
    }

    /// <summary>
    /// Opens the Rankings menu state.
    /// </summary>
    public void OpenRankings () {
        SwitchState(GameState.Rankings);
    }

    /// <summary>
    /// Returns to the previous game state (used after closing Options).
    /// </summary>
    public void BackToPreviousState () {
        SwitchState(previousState);
    }

    /// <summary>
    /// Returns to the main menu with proper scene unloading.
    /// </summary>
    public void ReturnToMainMenu () {
        StartCoroutine(ReturnToMenuRoutine());
    }

    /// <summary>
    /// Immediately switches to the MainMenu state (in addition to ReturnToMainMenu logic).
    /// </summary>
    public void BackToMainMenu () {
        ReturnToMainMenu();
        SwitchState(GameState.MainMenu);
    }

    /// <summary>
    /// Quits the application on PC.
    /// </summary>
    public void QuitGame () {
        Application.Quit();
    }

    /// <summary>
    /// Displays the Rankings menu.
    /// </summary>
    private static void OnRankings () {
        UIManager.instance.ShowRankingsMenu();
    }

    /// <summary>
    /// Displays the Options menu.
    /// </summary>
    private static void OnOptions () {
        UIManager.instance.ShowOptionsMenu();
    }

    /// <summary>
    /// Handles GameOver UI, stops time, unloads scene, and plays music.
    /// </summary>
    private void OnGameOver () {
        UIManager.instance.ShowGameOverMenu();
        Time.timeScale = 0f;
        StartCoroutine(UnloadSceneOnGameOver());
        AudioManager.instance.PlayGameOverMenuMusic();
    }

    /// <summary>
    /// Triggers round-specific game-over behavior and plays sound effect.
    /// </summary>
    private static void OnPreGameOver () {
        if (RoundManager.instance != null) {
            RoundManager.instance.GameOverBehaviour();
        }
        AudioManager.instance.PlayGameOverSoundFX();
    }

    /// <summary>
    /// Displays pause menu, stops time, and plays pause music.
    /// </summary>
    private static void OnPause () {
        UIManager.instance.ShowPauseMenu();
        Time.timeScale = 0f;
        AudioManager.instance.PlayPauseMusic();
        AudioManager.instance.PlayPauseSoundFX();
    }

    /// <summary>
    /// Hides all menus and resumes time.
    /// </summary>
    private static void OnPlaying () {
        UIManager.instance.HideAllMenus();
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Shows the main menu and plays its music.
    /// </summary>
    private static void OnMainMenu () {
        UIManager.instance.ShowMainMenu();
        AudioManager.instance.PlayMainMenuMusic();
    }

    /// <summary>
    /// Loads the gameplay scene additively and switches to Playing state.
    /// </summary>
    private IEnumerator LoadGameplayAdditive () {
        if (SceneManager.GetSceneByName(m_gameplaySceneName).isLoaded) {
            yield return UnloadGameplayScene();
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_gameplaySceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone) {
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(m_gameplaySceneName));
        SwitchState(GameState.Playing);
        AudioManager.instance.PlayStartGameMusicAndThenGameplay();
    }

    /// <summary>
    /// Unloads the gameplay scene asynchronously.
    /// </summary>
    private IEnumerator UnloadGameplayScene () {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(m_gameplaySceneName);
        while (!asyncUnload.isDone) {
            yield return null;
        }
    }

    /// <summary>
    /// Coroutine that returns to main menu and resets timescale.
    /// </summary>
    private IEnumerator ReturnToMenuRoutine () {
        Time.timeScale = 1f;
        if (SceneManager.GetSceneByName(m_gameplaySceneName).isLoaded) {
            yield return UnloadGameplayScene();
        }
        SwitchState(GameState.MainMenu);
    }

    /// <summary>
    /// Coroutine that restarts the game by unloading and reloading gameplay scene.
    /// </summary>
    private IEnumerator RestartGameRoutine () {
        if (SceneManager.GetSceneByName(m_gameplaySceneName).isLoaded) {
            yield return UnloadGameplayScene();
        }
        yield return LoadGameplayAdditive();
    }

    /// <summary>
    /// Coroutine to unload gameplay scene when the game ends.
    /// </summary>
    public IEnumerator UnloadSceneOnGameOver () {
        if (SceneManager.GetSceneByName(m_gameplaySceneName).isLoaded) {
            yield return UnloadGameplayScene();
        }
    }
}

/// <summary>
/// Possible states the game can be in, used by the GameManager to control game flow.
/// </summary>
public enum GameState {
    None,
    MainMenu,
    Playing,
    Paused,
    PreGameOver,
    GameOver,
    Options,
    Rankings
}