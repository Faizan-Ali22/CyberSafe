# Futuristic Pod Menu Animation Setup Guide

This guide explains how to set up the futuristic pod opening animation for the CyberSafe main menu.

## Overview

The pod menu animation creates a sci-fi style interface where:
1. Two side panels (pods) start closed, covering the menu
2. The pods slide/stretch open to reveal the main menu
3. Menu content fades in as the pods open
4. The effect plays on first launch as an intro, then as a quick transition on subsequent launches

## Assets Required

The following PNG sprites should be in `Assets/Scenes/Intro Scenes/`:
- **LeftPod Menu.png** - Left side pod graphic
- **RightPod Menu.png** - Right side pod graphic  
- **Main Menu.png** - Full menu with content
- **Empty Menu.png** - Background/empty menu

## Setup Instructions

### 1. Create the Menu Canvas Structure

In your Main Menu scene, create a UI hierarchy like this:

```
Canvas
├── PodMenuRoot (GameObject with PodMenuAnimator component)
│   ├── Background (Image - Empty Menu.png)
│   ├── LeftPod (Image - LeftPod Menu.png)
│   ├── RightPod (Image - RightPod Menu.png)
│   └── MenuContent (GameObject with CanvasGroup)
│       ├── Title
│       ├── Buttons
│       └── Other menu elements
```

### 2. Configure the Images

**Left Pod:**
- Add Image component with LeftPod Menu.png
- Set RectTransform:
  - Anchor: Left-Middle
  - Position: Adjust so it covers left side of menu when closed
  - Size: Match sprite dimensions

**Right Pod:**
- Add Image component with RightPod Menu.png
- Set RectTransform:
  - Anchor: Right-Middle
  - Position: Adjust so it covers right side of menu when closed
  - Size: Match sprite dimensions

**Background:**
- Add Image component with Empty Menu.png or Main Menu.png
- Stretch to fill canvas

### 3. Setup PodMenuAnimator Component

1. Add `PodMenuAnimator` script to PodMenuRoot GameObject
2. Assign references in Inspector:
   - **Left Pod**: Drag LeftPod GameObject
   - **Right Pod**: Drag RightPod GameObject
   - **Menu Content**: Drag MenuContent GameObject (must have CanvasGroup)
   - **Background Image**: (Optional) Drag Background GameObject

3. Configure animation settings:
   - **Open Duration**: 1.5s (adjust to taste)
   - **Close Duration**: 1.0s
   - **Pod Slide Distance**: 500 pixels (how far pods move)
   - **Pod Stretch Scale**: 1.2 (stretch effect multiplier)

4. Intro settings:
   - **Play Intro On Start**: Checked
   - **Intro Delay**: 0.3s
   - **Intro Played Key**: "PodMenuIntroPlayed"

### 4. Add CanvasGroup to MenuContent

The MenuContent GameObject needs a CanvasGroup component for fading:
1. Select MenuContent GameObject
2. Add Component > Canvas Group
3. Initial settings:
   - Alpha: 0
   - Interactable: Off
   - Block Raycasts: Off

(The script will manage these values during animation)

### 5. Optional: Integration with MainMenuAnimator

If you're using the existing MainMenuAnimator system, you can coordinate both:

```csharp
// In your menu initialization:
public PodMenuAnimator podAnimator;
public MainMenuAnimator menuAnimator;

private IEnumerator Start()
{
    // Play pod animation first
    yield return podAnimator.PlayIntroAnimation();
    
    // Then enable main menu interactions
    menuAnimator.OpenMenu();
}
```

## Animation Curve Customization

The `openCurve` field allows you to customize the easing:
- Default: EaseInOut for smooth start and end
- Linear: Constant speed throughout
- EaseOut: Fast start, slow end
- Custom: Draw your own curve in Inspector

## Testing

### Test First Launch
1. Play scene in Unity Editor
2. Should see intro animation with pods opening
3. Menu fades in and becomes interactive

### Test Subsequent Launches
1. Stop and play scene again
2. Should see quick open animation (no intro)

### Reset Intro Flag
To test the intro again:
- Select PodMenuAnimator in Hierarchy
- In Inspector, click "Reset Intro Flag" (if exposed)
- Or delete PlayerPrefs: `PlayerPrefs.DeleteKey("PodMenuIntroPlayed")`

## Troubleshooting

### Pods don't move
- Check that RectTransform anchors are set correctly
- Verify podSlideDistance is not 0
- Ensure pods are child of a Canvas

### Menu doesn't fade
- Verify MenuContent has CanvasGroup component
- Check that menuContent reference is assigned
- Ensure CanvasGroup alpha starts at 0

### Animation too fast/slow
- Adjust openDuration and closeDuration
- Modify the openCurve for different easing

### Pods positioned incorrectly
- Initial positions are cached in Awake()
- If you move pods in editor, stop/start play mode to recache
- Or manually set positions in closed state before playing

## Performance Tips

- Use sprite atlasing for the pod images
- Keep pod textures reasonably sized (1024x1024 or less)
- Disable mipmaps on UI sprites
- Use Rect Mask 2D instead of Mask for better performance

## Customization Ideas

### Add sound effects:
```csharp
public AudioClip podOpenSound;
private AudioSource audioSource;

// In OpenPods():
if (audioSource != null && podOpenSound != null)
{
    audioSource.PlayOneShot(podOpenSound);
}
```

### Add particles:
```csharp
public ParticleSystem openingParticles;

// At start of OpenPods():
if (openingParticles != null)
{
    openingParticles.Play();
}
```

### Add glow effect:
- Add Outline or Shadow component to pod images
- Animate the effect color alpha during opening

## Reference

Based on futuristic UI tutorial: https://www.youtube.com/watch?v=CE9VOZivb3I

The animation simulates mechanical pods opening to reveal interface elements, common in sci-fi games and movies like:
- Halo (UNSC terminals)
- Mass Effect (Omni-tool interfaces)
- Cyberpunk 2077 (Relic interfaces)

## API Reference

### Public Methods

```csharp
// Open the pods manually
podAnimator.Open();

// Close the pods manually
podAnimator.Close();

// Play full intro sequence
StartCoroutine(podAnimator.PlayIntroAnimation());

// Reset intro flag for testing
podAnimator.ResetIntroFlag();
```

### Public Coroutines

```csharp
// Wait for pods to open
yield return podAnimator.OpenPods();

// Wait for pods to close
yield return podAnimator.ClosePods();
```

## Integration Example

Complete example of menu flow with pod animation:

```csharp
using UnityEngine;
using CyberSafe.UI;

public class MenuController : MonoBehaviour
{
    public PodMenuAnimator podAnimator;
    public MainMenu mainMenu;
    
    private IEnumerator Start()
    {
        // Disable main menu initially
        mainMenu.enabled = false;
        
        // Play pod animation
        yield return podAnimator.PlayIntroAnimation();
        
        // Enable main menu functionality
        mainMenu.enabled = true;
        
        Debug.Log("Menu ready!");
    }
    
    public void OnStartGameClicked()
    {
        StartCoroutine(LoadGameWithTransition());
    }
    
    private IEnumerator LoadGameWithTransition()
    {
        // Close pods
        yield return podAnimator.ClosePods();
        
        // Load game scene
        SceneManager.LoadScene("GameScene");
    }
}
```

## Notes

- The script uses PlayerPrefs to track if intro has played
- First launch shows full intro, subsequent launches are quicker
- Animation uses RectTransform for UI positioning
- CanvasGroup controls fading and interaction blocking
- All references are validated in OnValidate() (Editor only)

---

**Created for CyberSafe Project**  
**Version 1.0**  
**Last Updated: November 2025**
