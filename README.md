# 🟩 Terminal Velo.City – Code-Oriented README

**Terminal Velo.City** is a Matrix-themed, infinite runner game developed in Unity 6 using C#. This project serves as a technical showcase for advanced programming techniques, game systems architecture, and backend connectivity—all implemented by **Yunuen Vladimir Sánchez Garrido** as part of a 10-mini-game portfolio.

> 📁 GitHub Repository: [View Source Code](https://github.com/yunrock/TerminalVeloCity-Scripts)
> 🌐 Playable Build: *[(Coming soon on Itch.io)](https://yunuenvladimir.itch.io/terminal-velo-city)*

---

## 🧠 Purpose of the Project

This project was created to demonstrate:

* Clean and modular C# scripting practices
* Usage of Unity’s component-based architecture
* Real-world application of design patterns
* Web integration (HTTP + PHP + MySQL)
* Custom animation and audio management systems

While the game is minimal in gameplay complexity, its underlying systems reflect a professional-level approach to game architecture and extensibility.

---

## 🧩 Architecture & Design Patterns

### 🔄 Component-Based Design

Unity’s component model is fully leveraged. All scripts are modular and decoupled, allowing for easy expansion and reuse. Key examples include:

* `PlayerController.cs`
* `GameManager.cs`
* `GUIManager.cs`
* `UIManager.cs`
* `RoundManager.cs`

### 🧱 Singleton Pattern

Core managers like `GameManager`, `AudioManager`, and `RoundManager` use the **Singleton** pattern for global accessibility and centralized control.

```csharp
public static GameManager instance { get; private set; }
```

### 🛰 Event-Driven Programming

The project uses **C# Events** for communication between systems, such as velocity changes affecting animation speed:

```csharp
RoundManager.instance.OnVelocityChanged += SetSpeedMultiplier;
```

### 🌐 Web Integration

A custom backend using PHP and MySQL allows leaderboard data to be:

* Submitted by players who achieve top scores
* Retrieved and displayed in-game

Communication uses Unity’s `UnityWebRequest` class to POST/GET data, promoting asynchronous, non-blocking gameplay.

```csharp
UnityWebRequest www = UnityWebRequest.Post(url, form);
yield return www.SendWebRequest();
```

---

## 🕹 Core Game Mechanics (Code-Relevant Summary)

* **Lane-based movement** using keyboard or gamepad (Input System)
* **Obstacle spawning system** with progressive speed scaling
* **Power-ups**: handled via modular interaction tags (Shield, Gun, BulletTime)
* **Bullet firing system** tied to input + pooling logic (planned for optimization)
* **Score system** with real-time point tracking and backend sync
* **Game states** controlled via a central GameManager FSM (finite state machine)

---

## 📦 File Structure (Scripts Only)

```
Assets/Scripts/
├── DataBase/
│   ├── Top10LoaderUpdater.cs
├── Items/
│   ├── BulletBehaviour.cs
│   ├── ObstacleBehaviour.cs
│   ├── PowerUpBehaviour.cs
│   ├── SymbolMatrixRainBehaviour.cs
├── Managers/
│   ├── AudioManager.cs
│   ├── GameManager.cs
│   ├── GameOverManager.cs
│   ├── GUIManager.cs
│   ├── LogoSceneManager.cs
│   ├── MatrixRainManager.cs
│   ├── ObstacleSpawner.cs
│   ├── PowerUpSpawner.cs
│   ├── RankingsManager.cs
│   ├── RoundManager.cs
│   ├── UIManager.cs
├── Player/
│   ├── PlayerAnimator.cs
│   ├── PlayerController.cs
├── UI/
│   ├── GUIManager.cs
│   └── MenuManager.cs
```

---

## 📡 Database & PHP Integration

* PHP scripts handle POST and GET requests
* MySQL stores top 10 scores
* Player name and score are validated before saving
* Backend hosted on external server

This setup demonstrates real-world client-server communication, including basic security principles like:

* Parameter validation
* Separation of concerns
* Rate-limited updates

---

## 🛠 Tech Stack

| Tool         | Role                          |
| ------------ | ----------------------------- |
| Unity 6      | Game engine                   |
| C#           | Game logic and architecture   |
| PHP          | Backend logic for leaderboard |
| MySQL        | Score data storage            |
| Git + GitHub | Version control               |

---

## 👨‍💻 About the Developer

**Yunuen Vladimir** – Mexico City 🇲🇽
Computer Scientist, AI Specialist, AI Researcher, and Game Developer.
This project is part of a self-driven journey into the professional video game industry.

🕹 Portfolio: [https://yunuenvladimir.itch.io](https://yunuenvladimir.itch.io)

---

## 📄 License

This repository is licensed under the **GNU GPLv3**. You are free to use, study, modify, and share it under the same license.

👉 [Read Full License](https://www.gnu.org/licenses/gpl-3.0.html)
