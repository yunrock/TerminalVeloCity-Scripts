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
/// Controls the behavior of a single Matrix-style symbol that fades out over time,
/// mimicking the "digital rain" effect.
/// </summary>
public class SymbolMatrixRainBehaviour : MonoBehaviour {
    /// <summary>
    /// The minimum duration for the symbol's fade-out effect.
    /// </summary>
    [SerializeField] private float m_fadeDurationMin = 0.3f;
    /// <summary>
    /// The maximum duration for the symbol's fade-out effect.
    /// </summary> 
    [SerializeField] private float m_fadeDurationMax = 0.5f; 

    private TextMeshProUGUI m_textMeshPro;
    private float m_fadeSpeed; 
    
    void Start () {
        SymbolSetup();
    }

    void OnEnable () {
        SymbolReset();
    }

    void Update () {
        SymbolBehaviour();
    }

    /// <summary>
    /// Sets up the symbol by retrieving its TextMeshProUGUI component and assigning a random fade speed.
    /// </summary>
    private void SymbolSetup () {
        m_textMeshPro = GetComponent<TextMeshProUGUI>();
        m_fadeSpeed = Random.Range(m_fadeDurationMin, m_fadeDurationMax); 
    }

    /// <summary>
    /// Resets the symbol's alpha value to fully visible (1).
    /// </summary>
    private void SymbolReset () {
        m_textMeshPro = GetComponent<TextMeshProUGUI>();
        Color color = m_textMeshPro.color;
        m_textMeshPro.color = new Color(color.r, color.g, color.b, 1f);
    }

    /// <summary>
    /// Gradually fades the symbol out. Deactivates the GameObject when fully transparent.
    /// </summary>
    private void SymbolBehaviour () {
        Color color = m_textMeshPro.color;
        float alpha = m_textMeshPro.color.a;
        if (alpha > 0) {
            alpha -= Time.deltaTime * m_fadeSpeed;
            m_textMeshPro.color = new Color(color.r, color.g, color.b, alpha);
        } else {
            gameObject.SetActive(false);
        }
    }
}
