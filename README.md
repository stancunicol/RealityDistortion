# Reality Distortion

![Unity](https://img.shields.io/badge/Unity-2022.3+-black?style=flat&logo=unity)
![C#](https://img.shields.io/badge/C%23-Language-239120?style=flat&logo=csharp)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?style=flat&logo=windows)
![License](https://img.shields.io/badge/License-MIT-yellow?style=flat)

A first-person psychological horror game built in Unity where perception is your only weapon. Navigate through floors while identifying anomalies in your environment – choose correctly to progress, or face the consequences of your mistakes.

## 🎮 Overview

**Reality Distortion** is an atmospheric anomaly detection game where players must carefully observe their surroundings across multiple floors. Each level presents a seemingly identical environment, but anomalies may appear – from subtle visual changes to obvious distortions.

### Core Gameplay
- **Observation**: Scan each floor for environmental irregularities
- **Decision-making**: Choose the correct elevator based on what you observed
- **Progressive challenge**: Advance through floors with varying anomaly difficulty
- **Diverse anomaly types**: Visual distortions, audio cues, animated objects, and environmental changes

## 🛠️ Technical Details

### Built With
- **Engine**: Unity (Universal Render Pipeline)
- **Input System**: Unity's New Input System
- **UI**: TextMesh Pro
- **Language**: C#

### Project Structure
```
Assets/
├── Scripts/
│   ├── AnomalyManager.cs         # Core game logic and level progression
│   ├── ElevatorController.cs      # Elevator interaction system
│   ├── ElevatorDoor.cs            # Door animation logic
│   ├── GameAudioManager.cs        # Audio management system
│   ├── MenuController.cs          # Main menu functionality
│   ├── PauseMenu.cs               # In-game pause system
│   ├── CreditsScroller.cs         # Credits scrolling
│   └── Anomalies/                 # Individual anomaly implementations
├── Scenes/
│   ├── MainScene.unity            # Primary gameplay scene
│   ├── MenuScene.unity            # Main menu
│   ├── DescriptionScene.unity     # Game instructions
│   └── CreditsScene.unity         # Credits
├── Prefabs/                       # Reusable game objects
├── Models/                        # 3D assets
├── Audio/                         # Sound effects and music
└── Sprites/                       # 2D textures and UI elements
```

### Key Systems

#### Anomaly System
The game features a modular anomaly system with various types:
- **Visual Anomalies**: Paintings, sculptures, lighting changes
- **Audio Anomalies**: Footsteps, ambient sounds
- **Animated Anomalies**: Moving objects, character appearances
- **Environmental Anomalies**: Door states, exit signs

Each anomaly implements the `IActivatableAnomaly` interface for consistent behavior:
```csharp
public interface IActivatableAnomaly
{
    void ActivateAnomaly();
    bool IsActivated();
}
```

#### Level Progression
- Configurable anomaly count per floor (0-3)
- Dynamic anomaly selection and activation
- Floor-based progression system
- Victory and game over conditions

## 🎯 Features

- ✅ First-person exploration
- ✅ Dynamic anomaly generation
- ✅ Multiple unique anomaly types
- ✅ Progressive difficulty system
- ✅ Atmospheric audio design
- ✅ Pause menu and settings
- ✅ Screenshot functionality
- ✅ Credits system

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 or later
- Required packages (included in project):
  - Universal Render Pipeline
  - TextMesh Pro
  - Input System

### Running the Project
1. Open the project in Unity Hub
2. Load `MenuScene` to start from the main menu
3. Or load `MainScene` for direct gameplay testing
4. Press Play in the Unity Editor

### Building
1. Open **File > Build Settings**
2. Ensure all scenes are added to the build:
   - MenuScene
   - DescriptionScene
   - MainScene
   - CreditsScene
3. Select your target platform
4. Click **Build** or **Build and Run**

## 🎨 Development

### Adding New Anomalies
1. Create a new script in `Assets/Scripts/Anomalies/`
2. Implement the `IActivatableAnomaly` interface
3. Add the anomaly GameObject to the scene
4. Tag it appropriately for the `AnomalyManager` to detect it

Example:
```csharp
public class MyCustomAnomaly : MonoBehaviour, IActivatableAnomaly
{
    private bool isActivated = false;
    
    public void ActivateAnomaly()
    {
        isActivated = true;
        // Your anomaly logic here
    }
    
    public bool IsActivated()
    {
        return isActivated;
    }
}
```

### Configuration
The `AnomalyManager` allows extensive customization:
- Anomaly count per floor
- Victory/Game Over messages and timing
- Floor display settings
- Debug logging options

## 📝 License

This project is licensed under the **MIT License**. Feel free to use, modify, and distribute this project for any purpose.