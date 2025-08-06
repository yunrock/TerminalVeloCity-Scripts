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

/// <summary>
/// Manages the logo scene.
/// </summary>
public class LogoSceneManager : MonoBehaviour {
    /// <summary>
    /// Changes the scene to the main scene, called by a event in logo animation.
    /// </summary>
    public void ChangeToMainScene () {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }
}
