# 🥪 Sandwich Puzzle Game
### Gameplay Video ###
![alt text](https://github.com/MahmoudAbousetta6/Sandwich-Game/blob/main/Images/Gameplay.gif)


A fun and engaging Unity-based puzzle game where players assemble sandwiches by dragging ingredients across a grid. The goal is to collect all ingredients and arrange them in the correct order to create a complete sandwich.

## 🎮 Game Overview

Sandwich Puzzle Game is a 2D grid-based puzzle game that challenges players to think strategically about ingredient placement and movement. Players must drag ingredients from one cell to another, ensuring they end up with a properly structured sandwich (bread on top and bottom) containing all the required ingredients.

## ✨ Features

### Core Gameplay
- **Grid-based Movement**: Drag ingredients between adjacent cells on a customizable grid
- **Ingredient Collection**: Gather various sandwich ingredients (bread, cheese, ham, bacon, etc.)
- **Sandwich Assembly**: Arrange ingredients to create a complete sandwich structure
- **Win Conditions**: Complete the sandwich with bread on top and bottom, containing all ingredients

### Game Mechanics
- **Adjacent Movement**: Ingredients can only move to neighboring cells
- **Stacking System**: Multiple ingredients can be stacked in a single cell
- **Flipping Animation**: Smooth 3D flipping animations when moving ingredients
- **Audio Feedback**: Sound effects for ingredient movements and win conditions

### Level System
- **Difficulty Levels**: Easy and Hard modes with different grid sizes and ingredient counts
- **Random Generation**: Procedurally generated levels with connected ingredient paths
- **Level Configuration**: ScriptableObject-based level design system
- **Reset Functionality**: Reset current level or generate new levels

### User Interface
- **Score System**: Time-based scoring with faster completion yielding higher scores
- **Camera Control**: Dynamic camera focusing on winning sandwich
- **UI Panels**: In-game and win screen interfaces
- **Button Controls**: Reset, generate new level, and next level options

## 🛠️ Technical Features

### Architecture
- **Clean Code Structure**: Well-organized scripts with comprehensive commenting
- **Modular Design**: Separated concerns for grid management, game state, and input handling
- **ScriptableObjects**: Data-driven design for ingredients and level configuration
- **Event System**: Decoupled communication between game systems

### Unity Integration
- **Cinemachine**: Professional camera system for dynamic camera control
- **DOTween**: Smooth animations and transitions
- **TextMesh Pro**: High-quality text rendering
- **Audio System**: Integrated sound effects and music

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or later
- Basic knowledge of Unity Editor

### Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/yourusername/sandwich-puzzle-game.git
   cd sandwich-puzzle-game
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Open" and select the cloned project folder
   - Wait for Unity to import all assets

3. **Open the Main Scene**
   - Navigate to `Assets/Scenes/Main Scene.unity`
   - Double-click to open the scene

4. **Run the Game**
   - Click the Play button in Unity Editor
   - Or build the project for your target platform

## 🎯 How to Play

### Basic Controls
- **Mouse Click and Drag**: Click on a cell with ingredients and drag to an adjacent empty cell
- **Valid Moves**: Only adjacent cells (horizontally or vertically) are valid destinations
- **Ingredient Stacking**: Multiple ingredients can be moved together

### Game Rules
1. **Objective**: Collect all ingredients and arrange them as a complete sandwich
2. **Sandwich Structure**: Must have bread as the bottom and top layers
3. **Movement**: Ingredients can only move to adjacent cells
4. **Completion**: All ingredients must be in a single cell with proper sandwich structure

### Scoring System
- **Time-based**: Faster completion yields higher scores
- **Formula**: `Score = (1 / completion_time) * 100 + 10`
- **Display**: Score shown on win screen with 2 decimal places

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Game/
│   │   ├── GameManager.cs          # Main game controller
│   │   └── LevelConfig.cs          # Level configuration system
│   ├── Cell/
│   │   ├── CellInfo.cs             # Grid cell positioning
│   │   └── CellPrefabHandler.cs    # Individual cell behavior
│   └── Ingredients/
│       ├── IngredientHandler.cs    # Ingredient behavior and animations
│       └── IngredientsData.cs      # Ingredient data structure
├── Prefabs/
│   ├── Cell Prefab.prefab          # Grid cell template
│   ├── Ingredients/                # Ingredient prefabs
│   └── Tiles/                      # Tile prefabs
├── Scenes/
│   └── Main Scene.unity            # Main game scene
├── Data/
│   ├── SO/                         # ScriptableObject assets
│   └── Audio/                      # Music and sound effects
└── Art/
    ├── Models/                     # 3D models for ingredients and tiles
    ├── Materials/                  # Materials and shaders
    └── Textures/                   # Texture assets
```

## 🎨 Customization

### Adding New Ingredients
1. Create a 3D model for the new ingredient
2. Add it to `Assets/Art/Models/ingredients/`
3. Create a prefab in `Assets/Prefabs/Ingredients/`
4. Add the prefab to the `IngredientsData` ScriptableObject
5. Set the `isBread` flag appropriately

### Modifying Level Difficulty
1. Open the `LevelConfig` ScriptableObject
2. Adjust grid dimensions (`GridX`, `GridZ`)
3. Modify spacing between cells
4. Change difficulty level (Easy/Hard)

### Customizing Animations
- Modify `flippingSpeed` in `CellPrefabHandler`
- Adjust animation curves in `IngredientHandler.FlipTo()`
- Update audio timing in `CellPrefabHandler.TranslateFrom()`

## 🐛 Troubleshooting

### Common Issues

**Game doesn't start**
- Ensure all required prefabs are assigned in the GameManager
- Check that the LevelConfig ScriptableObject is properly configured

**Ingredients not moving**
- Verify that cells have proper colliders
- Check that the camera is set up correctly for raycasting

**Audio not playing**
- Ensure AudioSource components are attached to cell prefabs
- Check audio file imports and settings

**Performance issues**
- Reduce grid size for better performance
- Optimize ingredient models and textures
- Consider using object pooling for large grids

## 🙏 Acknowledgments

- **Unity Technologies** for the amazing game engine
- **DOTween** for smooth animations
- **Cinemachine** for professional camera control
- **TextMesh Pro** for high-quality text rendering

## 📞 Support

If you encounter any issues or have questions:

- Create an issue in the GitHub repository
- Check the troubleshooting section above
- Review the code comments for implementation details

---

**Enjoy building delicious sandwiches! 🥪✨** 
