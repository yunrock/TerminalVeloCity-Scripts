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
using TMPro;

/// <summary>
/// UIManager is responsible for controlling the user interface elements:
/// main menu, pause menu, game over screen, options, and rankings.
/// It handles transitions between UI panels depending on the current game state,
/// updates relevant UI data such as scores and player names, and connects UI events
/// (e.g., button presses) to the appropriate game management methods.
/// </summary>
public class UIManager : MonoBehaviour {
    #region Serialize Field
    [Header("Menus")]
    [SerializeField] private GameObject m_mainMenu;
    [SerializeField] private GameObject m_pauseMenu;
    [SerializeField] private GameObject m_gameOverMenu;
    [SerializeField] private GameObject m_optionsMenu;
    [SerializeField] private GameObject m_rankingsMenu;

    [Header("Game Over UI Elements")]
    [SerializeField] private GameObject m_saveNewScoreForm;
    [SerializeField] private TextMeshProUGUI m_localTopScore;
    [SerializeField] private TextMeshProUGUI m_yourScore;
    [SerializeField] private TMP_InputField m_inputName;
    #endregion

    private int m_yourScoreValue;

    /// <summary>
    /// Singleton instance of the UIManager.
    /// </summary>
    public static UIManager instance {
        get; private set;
    }

    public int lastTopScore = 0;

    private void Awake () {
        SingletonSeutp();
    }

    /// <summary>
    /// Initializes the singleton instance and ensures it persists between scene loads.
    /// </summary>
    private void SingletonSeutp () {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        } else {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// Retrieves the top score stored locally using PlayerPrefs,
    /// and updates the top score label in the UI accordingly.
    /// </summary>
    /// <param name="topScore">Outputs the retrieved top score.</param>
    private void GetLocalTopScore (out int topScore) {
        topScore = PlayerPrefs.GetInt("TopScore", 0);
        string topPlayerName = PlayerPrefs.GetString("TopPlayerName", "");
        if (topScore > 0) {
            m_localTopScore.text = $"{topPlayerName}: {topScore}";
        } else {
            m_localTopScore.text = "VACANT";
        }
    }

    /// <summary>
    /// Hides all UI menus by deactivating their corresponding GameObjects.
    /// </summary>
    public void HideAllMenus () {
        m_mainMenu.SetActive(false);
        m_pauseMenu.SetActive(false);
        m_gameOverMenu.SetActive(false);
        m_optionsMenu.SetActive(false);
        m_rankingsMenu.SetActive(false);
    }

    /// <summary>
    /// Displays the main menu by hiding all others and activating the main menu GameObject.
    /// </summary>
    public void ShowMainMenu () {
        HideAllMenus();
        m_mainMenu.SetActive(true);
    }

    /// <summary>
    /// Displays the pause menu by hiding all others and activating the pause menu GameObject.
    /// </summary>
    public void ShowPauseMenu () {
        HideAllMenus();
        m_pauseMenu.SetActive(true);
    }

    /// <summary>
    /// Displays the game over menu, updates score fields, and activates
    /// the form to enter a new high score if applicable.
    /// </summary>
    public void ShowGameOverMenu () {
        m_yourScoreValue = RoundManager.instance != null ? RoundManager.instance.GetScore() : 0;
        HideAllMenus();
        FillGameOverMenu();
        m_gameOverMenu.SetActive(true);
    }

    /// <summary>
    /// Displays the rankings menu by hiding all others and activating the rankings menu GameObject.
    /// </summary>
    public void ShowRankingsMenu () {
        HideAllMenus();
        m_rankingsMenu.SetActive(true);
    }

    /// <summary>
    /// Displays the options menu by hiding all others and activating the options menu GameObject.
    /// </summary>
    public void ShowOptionsMenu () {
        HideAllMenus();
        m_optionsMenu.SetActive(true);
    }

    /// <summary>
    /// Fills the game over screen with relevant player score data,
    /// enables the new high score input form if the score is a new top score,
    /// and retrieves the last used player name from PlayerPrefs.
    /// </summary>
    public void FillGameOverMenu () {
        string lastPlayerUseGame;
        m_saveNewScoreForm.SetActive(false);
        GetLocalTopScore(out int topScore);
        m_yourScore.text = $"Your Score: {m_yourScoreValue}";
        lastTopScore = m_yourScoreValue;
        if ((RoundManager.instance != null) && (m_yourScoreValue >= topScore)) {
            m_saveNewScoreForm.SetActive(true);
        }
        lastPlayerUseGame = PlayerPrefs.GetString("LastPlayerUseGame", "");
        if (lastPlayerUseGame != "") {
            m_inputName.text = lastPlayerUseGame;
        }
    }

    /// <summary>
    /// Starts the game. Connected to the "Start Game" button in the UI.
    /// </summary>
    public void OnStartGameButton () => GameManager.instance.StartGame();

    /// <summary>
    /// Resumes the game from a paused state. Connected to the "Resume" button in the pause menu.
    /// </summary>
    public void OnResumeGameButton () => GameManager.instance.ResumeGame();

    /// <summary>
    /// Opens the options menu. Connected to the "Options" button in the UI.
    /// </summary>
    public void OnOptionsButton () => GameManager.instance.OpenOptions();

    /// <summary>
    /// Returns to the previous game state from the options menu. Connected to the "Back" button.
    /// </summary>
    public void OnBackFromOptionsButton () => GameManager.instance.BackToPreviousState();

    /// <summary>
    /// Returns to the main menu. Connected to the "Main Menu" button.
    /// </summary>
    public void OnMainMenuButton () => GameManager.instance.BackToMainMenu();

    /// <summary>
    /// Restarts the game. Connected to the "Restart" button on the game over screen.
    /// </summary>
    public void OnRestartGameButton () => GameManager.instance.RestartGame();

    /// <summary>
    /// Quits the application. Connected to the "Quit" button, in PC version only.
    /// </summary>
    public void OnQuitButton () => GameManager.instance.QuitGame();

    /// <summary>
    /// Opens the rankings screen. Connected to the "Rankings" button in the UI.
    /// </summary>
    public void OnRankingsButton() => GameManager.instance.OpenRankings();
}
