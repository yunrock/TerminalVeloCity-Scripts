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
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the graphical user interface (GUI) elements related to score and power-ups.
/// Implements a singleton pattern to ensure a single instance throughout the game.
/// </summary>
public class GUIManager : MonoBehaviour {

    /// <summary>
    /// Singleton instance of the GUIManager.
    /// </summary>
    public static GUIManager instance;

    [Header("Alpha Text Settings")]
    [SerializeField] private float m_minAlpha = 0.05f;
    [SerializeField] private float m_maxAlpha = 1f;

    [Header("PowerUp TMP Text")]
    [SerializeField] private TextMeshProUGUI m_pointsText;
    [SerializeField] private TextMeshProUGUI m_GunText;
    [SerializeField] private TextMeshProUGUI m_shieldText;
    [SerializeField] private TextMeshProUGUI m_bulletTimeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start () {
        SingletonSetup();
    }

    /// <summary>
    /// Sets up the singleton instance, ensuring only one instance exists.
    /// </summary>
    private void SingletonSetup () {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Updates the score text displayed on the UI.
    /// </summary>
    /// <param name="score">The current player score to display.</param>
    public void SetScore (int score) {
        if (m_pointsText != null) {
            m_pointsText.text = score.ToString();
        } else {
            Debug.LogWarning("Score Text is not assigned in GUIManager.");
        }
    }

    /// <summary>
    /// Makes the Gun power-up text fully visible.
    /// </summary>
    public void ShowGunText () {
        UnityEngine.Color currentColor = m_GunText.color;
        currentColor.a = m_maxAlpha;
        m_GunText.color = currentColor;
    }

    /// <summary>
    /// Makes the Gun power-up text partially transparent.
    /// </summary>
    public void HideGunText () {
        UnityEngine.Color currentColor = m_GunText.color;
        currentColor.a = m_minAlpha;
        m_GunText.color = currentColor;
    }

    /// <summary>
    /// Makes the Shield power-up text fully visible.
    /// </summary>
    public void ShowShieldText () {
        UnityEngine.Color currentColor = m_shieldText.color;
        currentColor.a = m_maxAlpha;
        m_shieldText.color = currentColor;
    }

    /// <summary>
    /// Makes the Shield power-up text partially transparent.
    /// </summary>
    public void HideShieldText () {
        UnityEngine.Color currentColor = m_shieldText.color;
        currentColor.a = m_minAlpha;
        m_shieldText.color = currentColor;
    }

    /// <summary>
    /// Makes the Bullet Time power-up text fully visible.
    /// </summary>
    public void ShowBulletTimeText () {
        UnityEngine.Color currentColor = m_bulletTimeText.color;
        currentColor.a = m_maxAlpha;
        m_bulletTimeText.color = currentColor;
    }

    /// <summary>
    /// Makes the Bullet Time power-up text partially transparent.
    /// </summary>
    public void HideBulletTimeText () {
        UnityEngine.Color currentColor = m_bulletTimeText.color;
        currentColor.a = m_minAlpha;
        m_bulletTimeText.color = currentColor;
    }
}