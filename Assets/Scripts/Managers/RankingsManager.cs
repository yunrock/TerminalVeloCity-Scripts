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
/// Manages the Rankings Menu functionality including displaying local and global top scores.
/// </summary>
public class RankingsManager : MonoBehaviour {

    [Header("Game Over UI Elements")]
    [SerializeField] private TextMeshProUGUI m_LocalTopScore;
    [SerializeField] private TextMeshProUGUI m_scoresList;
    [SerializeField] private TextMeshProUGUI m_scoreNamesList;

    private int m_tryConnectionCount;

    void OnEnable () {
        m_tryConnectionCount = 0;
        FillRankingsMenu();
        LoadTop10UIFromServer();
    }

    /// <summary>
    /// Prepares and displays the local top score from PlayerPrefs.
    /// </summary>
    public void FillRankingsMenu () {
        int topScore = PlayerPrefs.GetInt("TopScore", 0);
        string topPlayerName = PlayerPrefs.GetString("TopPlayerName", "");
        if (topScore > 0) {
            m_LocalTopScore.text = $"{topPlayerName}: {topScore}";
        } else {
            m_LocalTopScore.text = "VACANT";
        }
    }

    /// <summary>
    /// Updates the top 10 scores UI by loading data from the server.
    /// Displays loading messages while waiting for the data.
    /// </summary>
    public void LoadTop10UIFromServer () {
        if (m_tryConnectionCount == 0) {
            m_scoresList.text = "It may\n..Loading..\n..Loading..\n..Loading..\n..Loading..\n" +
                "..Loading..\n..Loading..\n..Loading..\n..Loading..\n..Loading..\n";
            m_scoreNamesList.text = "takes a while\n...Loading...\n...Loading...\n...Loading...\n...Loading...\n" +
                "...Loading...\n...Loading...\n...Loading...\n...Loading...\n...Loading...\n";
        }
        Top10LoaderUpdater loaderUpdater = GetComponent<Top10LoaderUpdater>();
        if (loaderUpdater != null) {
            loaderUpdater.Top10Loader(LoadTop10);
        } else {
            m_scoresList.text = "No loader found";
            m_scoreNamesList.text = "No loader found";
        }
    }

    /// <summary>
    /// Callback for when top 10 scores are loaded from the server.
    /// Formats and displays the scores and names.
    /// </summary>
    /// <param name="topNames">List of top player names</param>
    /// <param name="topScores">List of top scores corresponding to the names</param>
    private void LoadTop10 (List<string> topNames, List<int> topScores) {
        try {
            if (topNames == null || topNames.Count == 0) {
                m_scoresList.text = "No data";
                m_scoreNamesList.text = "No data";
                throw new System.Exception();
            }
        } catch (System.Exception) {
            HandleLoadFailure();
        }
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
        }
        m_scoresList.text = scores.ToString();
        m_scoreNamesList.text = names.ToString();
    }

    /// <summary>
    /// Handles failures when loading top scores from the server.
    /// Implements a retry mechanism with up to 3 attempts.
    /// </summary>
    private void HandleLoadFailure () {
        m_tryConnectionCount++;
        if (m_tryConnectionCount < 3) {
            m_scoreNamesList.text = "Failed to\nload Top 10,\n retrying...";
            LoadTop10UIFromServer();
        } else {
            m_scoresList.text = "Failed to\nload Top 10\nafter\nmultiple\nattempts.";
            m_scoreNamesList.text = "Failed to\nload Top 10\nafter\nmultiple\nattempts.";
        }
    }
}
