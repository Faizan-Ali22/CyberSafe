# Main Menu Intro + Open/Close

This guide shows how to use your PSD UI (Brackeys tutorial) with a one-time intro animation and normal open/close transitions.

## 1) Import the PSD as UI sprites
1. Window > Package Manager > install "2D PSD Importer".
2. Drag your PSD into `Assets/Sprites/` (or any folder).
3. In Inspector: set Sprite Mode = Multiple and enable "Use Layers".
4. Open Sprite Editor, slice the elements, and set 9-slice borders for the frame/panel.

## 2) Build the Main Menu hierarchy
- Create a Canvas (Screen Space - Overlay) and an EventSystem.
- Under the Canvas, create an empty `MainMenu` GameObject (this will be the root panel).
- Add:
  - Image(s) for background frame and side elements from your PSD.
  - Buttons (TextMeshPro recommended) for Play, Settings, Quit, etc.
  - CanvasGroup (on `MainMenu` root).
  - Animator (on `MainMenu` root).
  - Script `MainMenuAnimator` (already in `Assets/Scripts/UI/MainMenuAnimator.cs`).

## 3) Animator Controller
Create an Animator Controller and assign it to the `MainMenu` Animator with these states on Layer 0:

- `Closed` (default state)
  - CanvasGroup.alpha = 0, scale slightly down or off-screen position.
- `Open`
  - CanvasGroup.alpha = 1, scale 1, in-place.
- `IntroOpen`
  - Longer, more elaborate animation (e.g., elements slide in, glow, etc.).
- `Close` (optional)
  - Reverse of `Open`. If you omit this state, make an Any State -> `Closed` transition for closing.

Parameters (Triggers):
- `trIntro`
- `trOpen`
- `trClose`

Transitions:
- Any State -> `IntroOpen` on `trIntro` (Has Exit Time OFF).
- Any State -> `Open` on `trOpen` (Has Exit Time OFF).
- Any State -> `Close` (or `Closed`) on `trClose`.

Tips:
- In each state clip, animate `CanvasGroup.alpha` and optionally `RectTransform.anchoredPosition`/`localScale`.
- Set keyframes with ease-out curves for snappy UI.

## 4) Script wiring
On `MainMenu` root (the object with Animator):
- Ensure `MainMenuAnimator` has references (Animator, CanvasGroup) or it will auto-fetch on Awake.
- The script plays `IntroOpen` once per install (stored in PlayerPrefs). Afterwards it uses normal `Open`.

Public methods to hook:
- `OpenMenu()` – plays the regular open transition.
- `CloseMenu()` – plays the close transition.
- `CloseAndLoadScene(string sceneName)` – closes then loads a scene (use for Play button).
- `ReplayIntro()` – if you want to replay the intro from an options menu.

## 5) Buttons (Brackeys-style)
- Play: add OnClick -> `MainMenuAnimator.CloseAndLoadScene("GameScene")`.
- Settings: OnClick -> close this menu and open your Settings panel's animator.
- Quit: OnClick -> `Application.Quit()` (and in Editor, `UnityEditor.EditorApplication.isPlaying = false;`).

## 6) Optional polish
- Animate side neon pods, glow pulses, and SFX during `IntroOpen`.
- Add a short delay before enabling interaction; the script already gates input using `CanvasGroup`.

## 7) Troubleshooting
- If clicks go through while animating, ensure `CanvasGroup.blocksRaycasts` is true when open.
- Make sure state names match exactly: `Closed`, `Open`, `IntroOpen`, `Close`.
- If Close sticks, verify the transition on `trClose` goes to either `Close` or directly to `Closed`.
- If intro repeats every time, check PlayerPrefs key `MainMenuIntroPlayed` and delete/reset it during testing.

