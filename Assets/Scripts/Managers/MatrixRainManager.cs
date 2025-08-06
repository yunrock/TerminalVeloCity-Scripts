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
/// Manages the Matrix rain effect by instantiating and animating columns of text characters.
/// </summary>
public class MatrixRainManager : MonoBehaviour {

    [SerializeField] private GameObject m_matrixTextPrefab;
    [SerializeField] private GameObject m_matrixParentTextPrefab;
    [SerializeField] private int m_numberOfRows = 18;
    [SerializeField] private int m_numberOfColumns = 33;
    [SerializeField] private int m_xDisplacement = 30;
    [SerializeField] private int m_yDispalcement = -30;

    void Start () {
        Setup();
    }

    void Update () {
        MatrixRain();
    }

    /// <summary>
    /// Controls the activation and updating of text characters to simulate the falling matrix effect.
    /// </summary>
    private void MatrixRain () {
        foreach (Transform col in transform) {
            for (int i = 0; i < col.childCount; i++) {
                Transform row = col.GetChild(i);
                if ((i > 0) && !row.gameObject.activeSelf) {
                    if (i < (col.childCount - 5)) {
                        if (col.GetChild(i + 4).gameObject.activeSelf) {
                            continue;
                        }
                    }
                    if (col.GetChild(i - 1).GetComponent<TextMeshProUGUI>().color.a < 0.8f) {
                        if (Random.value > 0.6f) {
                            row.GetComponent<TextMeshProUGUI>().text = GetRandomCharacter();
                            row.gameObject.SetActive(true);
                        }
                    }
                    break;
                } else if ((i == 0) && !row.gameObject.activeSelf) {
                    if (!col.GetChild(i + 2).gameObject.activeSelf) {
                        if (Random.value > 0.1f) {
                            row.GetComponent<TextMeshProUGUI>().text = GetRandomCharacter();
                            row.gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Instantiates the matrix rain structure: columns and their child rows.
    /// </summary>
    private void Setup () {
        for (int col = 0; col < m_numberOfColumns; col++) {
            GameObject temp = Instantiate(m_matrixParentTextPrefab, transform);
            temp.transform.localPosition = 
                new Vector3(m_matrixParentTextPrefab.transform.position.x + (m_xDisplacement * col), m_matrixParentTextPrefab.transform.position.y, 0);
            for (int row = 0; row < m_numberOfRows; row++) {
                GameObject matrixText = Instantiate(m_matrixTextPrefab, temp.transform);
                Vector3 pos = matrixText.transform.localPosition;
                pos.y = m_matrixTextPrefab.transform.localPosition.y + (m_yDispalcement * row);
                matrixText.transform.localPosition = pos;
                if ((row == 0) && (Random.value > 0.1f)) {
                    matrixText.GetComponent<TextMeshProUGUI>().text = GetRandomCharacter(); 
                    matrixText.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Returns a randomly selected character from a string containing Latin, Katakana, and Hangul characters.
    /// </summary>
    /// <returns>A random character as a string.</returns>
    private string GetRandomCharacter() {
        const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890アイウエオカキクケコサシスセソタチツテトナヌヒフヘホマムメモヤユヨラリルレロワヰヱヲㄶㅊㅎㅖㅘㆄㅽㆇㆌㆆㅝㅠㄲㄴ";
        int randomIndex = Random.Range(0, characters.Length);
        return characters[randomIndex].ToString();
    }
}
