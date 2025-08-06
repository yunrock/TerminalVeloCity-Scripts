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
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a single entry in the top score list, containing the player's name and score.
/// </summary>
[System.Serializable]
public class TopScoreEntry {
    public int score;
    public string name;
}

/// <summary>
/// Data container used for updating the top 10 leaderboard.
/// </summary>
[System.Serializable]
public class Top10UpdateData {
    public List<int> scores;
    public List<string> names;
}

/// <summary>
/// Handles loading and updating the top 10 scores from and to a remote server.
/// </summary>
public class Top10LoaderUpdater : MonoBehaviour {
    /// <summary>
    /// The URL used to load the top 10 leaderboard data.
    /// </summary>
    public string loaderUrl = "https://yunrock.helioho.st/top10.php";

    /// <summary>
    /// The URL used to send updated leaderboard data to the server.
    /// </summary>
    public string updateUrl = "https://yunrock.helioho.st/update_top10.php";

    private List<string> m_top10Names;
    private List<int> m_top10Scores;

    /// <summary>
    /// Starts the process to load the top 10 leaderboard entries from the server.
    /// </summary>
    /// <param name="onLoaded">Callback invoked with the list of names and scores upon success or with empty lists on failure.</param>
    public void Top10Loader (System.Action<List<string>, List<int>> onLoaded) {
        StartCoroutine(LoadTop10Coroutine(onLoaded));
    }

    /// <summary>
    /// Coroutine that handles the loading of top 10 scores from the server.
    /// </summary>
    /// <param name="onLoaded">Callback invoked after loading is complete.</param>
    private IEnumerator LoadTop10Coroutine (System.Action<List<string>, List<int>> onLoaded) {
        UnityWebRequest www = UnityWebRequest.Get(loaderUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success) {
            string json = www.downloadHandler.text;
            List<TopScoreEntry> entries = new List<TopScoreEntry>();
            try {
                entries = JsonUtility.FromJson<TopScoreList>("{\"list\":" + json + "}").list;
            } catch {
                Debug.LogError("Error parsing JSON: " + json);
            }
            m_top10Names = new List<string>();
            m_top10Scores = new List<int>();
            for (int i = 0; i < entries.Count; i++) {
                m_top10Scores.Add(entries[i].score);
                m_top10Names.Add(entries[i].name.Trim());
            }
            onLoaded?.Invoke(m_top10Names, m_top10Scores);
        } else {
            Debug.LogError("Error loading top 10: " + www.error);
            onLoaded?.Invoke(new List<string>(), new List<int>());
        }
    }

    /// <summary>
    /// Sends updated top 10 leaderboard data to the server.
    /// </summary>
    /// <param name="scoresIntList">List of new scores to update.</param>
    /// <param name="namesStringList">List of corresponding player names.</param>
    /// <param name="onComplete">Callback invoked with a boolean indicating success or failure.</param>
    public void UpdateTop10 (List<int> scoresIntList, List<string> namesStringList, System.Action<bool> onComplete) {
        StartCoroutine(UpdateTop10Coroutine(scoresIntList, namesStringList, onComplete));
    }

    /// <summary>
    /// Coroutine that handles sending updated leaderboard data to the server.
    /// </summary>
    /// <param name="scoresIntList">List of scores.</param>
    /// <param name="namesStringList">List of player names.</param>
    /// <param name="onComplete">Callback invoked when the operation completes.</param>
    private IEnumerator UpdateTop10Coroutine (List<int> scoresIntList, List<string> namesStringList, System.Action<bool> onComplete) {
        Top10UpdateData data = new Top10UpdateData {
            scores = scoresIntList,
            names = namesStringList
        };
        string jsonData = JsonUtility.ToJson(data);
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest www = new UnityWebRequest(updateUrl, "POST");
        www.uploadHandler = new UploadHandlerRaw(postData);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success) {
            try {
                UpdateResponse response = JsonUtility.FromJson<UpdateResponse>(www.downloadHandler.text);
                onComplete?.Invoke(response.success);
            } catch {
                Debug.LogError("Error parsing response: " + www.downloadHandler.text);
                onComplete?.Invoke(false);
            }
        } else {
            Debug.LogError("Error updating top 10: " + www.error);
            onComplete?.Invoke(false);
        }
    }

    /// <summary>
    /// Internal class used to deserialize the list of top score entries.
    /// </summary>
    [System.Serializable]
    private class TopScoreList {
        public List<TopScoreEntry> list;
    }

    /// <summary>
    /// Internal class used to deserialize the server's response when updating the leaderboard.
    /// </summary>
    [System.Serializable]
    private class UpdateResponse {
        public bool success;
    }
}
