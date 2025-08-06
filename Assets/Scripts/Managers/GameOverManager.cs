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
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the Game Over Screen functionality including score display and saving.
/// Handles both local and global top score management.
/// </summary>
public class GameOverManager : MonoBehaviour {
    [Header("Game Over UI Elements")]
    [SerializeField] private TMP_InputField m_inputName;
    [SerializeField] private TextMeshProUGUI m_localTopScore;
    [SerializeField] private TextMeshProUGUI m_scoresList;
    [SerializeField] private TextMeshProUGUI m_scoreNamesList;
    [SerializeField] private UnityEngine.UI.Button m_saveButton;

    private int m_currentScore, m_indexTopScore;
    private bool m_isWorldTop10Score = false;
    private bool m_isLocalTop10Score = false;
    private bool m_isGamveOverMenu = false;
    private List<int> m_scoresIntList;
    private List<string> m_namesStringList;
    private Top10LoaderUpdater m_loaderUpdater;
    private int m_tryConnectionCount;

    /// <summary>
    /// Initializes the manager state and starts loading top scores.
    /// </summary>
    private void OnEnable () {
        m_tryConnectionCount = 0;
        m_isLocalTop10Score = m_saveButton.transform.parent.gameObject.activeSelf;
        m_saveButton.interactable = false;
        m_isGamveOverMenu = gameObject.name.Equals("GameOver Menu Panel");
        m_loaderUpdater = GetComponent<Top10LoaderUpdater>();
        m_currentScore = UIManager.instance.lastTopScore;
        UpdateTop10UIFromServer();
    }

    /// <summary>
    /// Handles the save button click event.
    /// Saves the player's score either locally or to the global leaderboard.
    /// </summary>
    public void OnSaveButton () {
        if (m_inputName.text.Length > 0) {
            PlayerPrefs.SetString("LastPlayerUseGame", m_inputName.text);
            if (m_isLocalTop10Score) {
                PlayerPrefs.SetString("TopPlayerName", m_inputName.text);
                PlayerPrefs.SetInt("TopScore", m_currentScore);
                m_localTopScore.text = $"{PlayerPrefs.GetString("TopPlayerName")}: {PlayerPrefs.GetInt("TopScore")}";
            }
            if (m_isWorldTop10Score) {
                m_namesStringList.Insert(m_indexTopScore, m_inputName.text);
                m_scoresIntList.Insert(m_indexTopScore, m_currentScore);
                while (m_namesStringList.Count > 10) {
                    m_namesStringList.RemoveAt(m_namesStringList.Count - 1);
                }
                while (m_scoresIntList.Count > 10) {
                    m_scoresIntList.RemoveAt(m_scoresIntList.Count - 1);
                }
                m_loaderUpdater = GetComponent<Top10LoaderUpdater>();
                m_loaderUpdater.UpdateTop10(m_scoresIntList, m_namesStringList, (success) => {
                    if (success) {
                        UpdateTop10UIFromServer();
                        m_saveButton.interactable = false;
                    } else {
                        Debug.LogError("Failed to update Top 10.");
                    }
                });
            }
            m_saveButton.interactable = false;
        } else {
            ((TextMeshProUGUI) m_inputName.placeholder).text = "YOUR NICKNAME!";
        }
    }

    /// <summary>
    /// Updates the top 10 scores UI by loading data from the server.
    /// Displays loading messages while waiting for the data.
    /// </summary>
    public void UpdateTop10UIFromServer () {
        if (m_tryConnectionCount == 0) {
            m_scoresList.text = "It may\n..Loading..\n..Loading..\n..Loading..\n..Loading..\n" +
                "..Loading..\n..Loading..\n..Loading..\n..Loading..\n..Loading..\n";
            m_scoreNamesList.text = "takes a while\n...Loading...\n...Loading...\n...Loading...\n...Loading...\n" +
                "...Loading...\n...Loading...\n...Loading...\n...Loading...\n...Loading...\n";
        }
        if (m_loaderUpdater != null) {
            m_loaderUpdater.Top10Loader(OnTop10Loaded);
        } else {
            m_scoresList.text = "No loader found";
            m_scoreNamesList.text = "No loader found";
        }
        m_saveButton.interactable = true;
    }

    /// <summary>
    /// Callback for when top 10 scores are loaded from the server.
    /// Formats and displays the scores and names.
    /// </summary>
    /// <param name="topNames">List of top player names</param>
    /// <param name="topScores">List of top scores corresponding to the names</param>
    private void OnTop10Loaded (List<string> topNames, List<int> topScores) {
        try {
            if (topNames == null || topNames.Count == 0) {
                m_scoresList.text = "No data";
                m_scoreNamesList.text = "No data";
                throw new System.Exception("No data");
            }
            m_scoresIntList = new List<int>(topScores);
            m_namesStringList = new List<string>(topNames);
            m_isWorldTop10Score = false;
            System.Text.StringBuilder scores = new System.Text.StringBuilder();
            System.Text.StringBuilder names = new System.Text.StringBuilder();
            for (int i = 0; i < topNames.Count; i++) {
                string topScore = topScores[i].ToString();
                while (topScore.Length < 11) {
                    topScore = "." + topScore;
                }
                string topName = topNames[i];
                while (topName.Length < 14) {
                    topName = topName + ".";
                }
                scores.AppendLine(topScore);
                names.AppendLine(topName);
                if (m_isGamveOverMenu && !m_isWorldTop10Score && (m_currentScore >= topScores[i])) {
                    m_isWorldTop10Score = true;
                    m_indexTopScore = i;
                    m_saveButton.transform.parent.gameObject.SetActive(true);
                }
            }
            m_scoresList.text = scores.ToString();
            m_scoreNamesList.text = names.ToString();
        } catch (System.Exception) {
            HandleLoadFailure();
        }
    }

    /// <summary>
    /// Handles failures when loading top scores from the server.
    /// Implements a retry mechanism with up to 3 attempts.
    /// </summary>
    private void HandleLoadFailure () {
        m_tryConnectionCount++;
        if (m_tryConnectionCount < 3) {
            m_scoreNamesList.text = "Failed to\nload Top 10,\n retrying...";
            UpdateTop10UIFromServer();
        } else {
            m_scoresList.text = "Failed to\nload Top 10\nafter\nmultiple\nattempts.";
            m_scoreNamesList.text = "Failed to\nload Top 10\nafter\nmultiple\nattempts.";
        }
    }
}
