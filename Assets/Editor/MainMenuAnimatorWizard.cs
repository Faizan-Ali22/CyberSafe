#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.IO;

// MainMenuAnimatorWizard
// Creates / refines AnimatorController & animation clips (IntroOpen, Open, Close, Closed) with polished easing curves.
// Usage: Tools > UI > Build Main Menu Animator ... then select the MainMenu root GameObject.
// It will place generated assets under Assets/GeneratedScripts/UIAnimations/ by default.

namespace CyberSafe.UI.Editor
{
    public class MainMenuAnimatorWizard : EditorWindow
    {
        private GameObject targetRoot;
        private string outputFolder = "Assets/GeneratedScripts/UIAnimations";
        private float introDuration = 1.2f;
        private float openDuration = 0.35f;
        private float closeDuration = 0.30f;
        private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0,0,1,1);
        private AnimationCurve scaleInCurve = new AnimationCurve(new Keyframe(0,0.88f,0,2.2f), new Keyframe(0.6f,1.02f,0,0), new Keyframe(1f,1f,0,0));
        private AnimationCurve scaleOutCurve = new AnimationCurve(new Keyframe(0,1f,0,0), new Keyframe(0.75f,0.95f,0,0), new Keyframe(1f,0.88f,0,0));

        [MenuItem("Tools/UI/Build Main Menu Animator...")] public static void Open() => GetWindow<MainMenuAnimatorWizard>(true, "Main Menu Animator Wizard");

        private void OnGUI()
        {
            GUILayout.Label("Main Menu Animator Generator", EditorStyles.boldLabel);
            targetRoot = (GameObject)EditorGUILayout.ObjectField("Main Menu Root", targetRoot, typeof(GameObject), true);
            outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
            introDuration = EditorGUILayout.FloatField("Intro Duration", introDuration);
            openDuration = EditorGUILayout.FloatField("Open Duration", openDuration);
            closeDuration = EditorGUILayout.FloatField("Close Duration", closeDuration);
            fadeCurve = EditorGUILayout.CurveField("Fade Curve", fadeCurve);
            scaleInCurve = EditorGUILayout.CurveField("Scale In Curve", scaleInCurve);
            scaleOutCurve = EditorGUILayout.CurveField("Scale Out Curve", scaleOutCurve);

            EditorGUILayout.Space();
            if (GUILayout.Button("Generate / Update"))
            {
                if (!Validate()) return;
                Generate();
            }
        }

        private bool Validate()
        {
            if (!targetRoot)
            {
                ShowNotification(new GUIContent("Assign Main Menu Root."));
                return false;
            }
            if (!targetRoot.GetComponent<CanvasGroup>())
            {
                ShowNotification(new GUIContent("Root needs CanvasGroup."));
                return false;
            }
            return true;
        }

        private void Generate()
        {
            if (!Directory.Exists(outputFolder)) Directory.CreateDirectory(outputFolder);
            string controllerPath = Path.Combine(outputFolder, "MainMenuAnimator.controller");

            // Controller or load existing
            AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            if (!controller)
            {
                controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
            }

            var layer = controller.layers[0];
            layer.name = "Base";

            // Ensure parameters
            AddTrigger(controller, "trIntro");
            AddTrigger(controller, "trOpen");
            AddTrigger(controller, "trClose");

            // Clips
            AnimationClip clipClosed = CreateClosedClip();
            AnimationClip clipOpen = CreateOpenClip(openDuration);
            AnimationClip clipIntro = CreateIntroClip(introDuration);
            AnimationClip clipClose = CreateCloseClip(closeDuration);

            SaveClip(clipClosed, Path.Combine(outputFolder, "Closed.anim"));
            SaveClip(clipOpen, Path.Combine(outputFolder, "Open.anim"));
            SaveClip(clipIntro, Path.Combine(outputFolder, "IntroOpen.anim"));
            SaveClip(clipClose, Path.Combine(outputFolder, "Close.anim"));

            // States (reuse if exist)
            AnimatorStateMachine sm = layer.stateMachine;
            AnimatorState stClosed = FindOrCreateState(sm, "Closed", clipClosed);
            AnimatorState stOpen = FindOrCreateState(sm, "Open", clipOpen);
            AnimatorState stIntro = FindOrCreateState(sm, "IntroOpen", clipIntro);
            AnimatorState stClose = FindOrCreateState(sm, "Close", clipClose);
            sm.defaultState = stClosed;

            // Clear existing transitions for clarity
            ClearTransitions(stClosed, stOpen, stIntro, stClose);

            // Transitions
            AddTransitionAny(sm, stIntro, "trIntro");
            AddTransitionAny(sm, stOpen, "trOpen");
            AddTransitionAny(sm, stClose, "trClose");

            // Auto-return from IntroOpen to Open
            var introToOpen = stIntro.AddTransition(stOpen);
            introToOpen.hasExitTime = true;
            introToOpen.exitTime = 0.98f;
            introToOpen.duration = 0.05f;

            // Close goes to Closed
            var closeToClosed = stClose.AddTransition(stClosed);
            closeToClosed.hasExitTime = true;
            closeToClosed.exitTime = 0.99f;
            closeToClosed.duration = 0.05f;

            EditorUtility.SetDirty(controller);

            // Assign animator
            var animComp = targetRoot.GetComponent<Animator>();
            if (!animComp) animComp = targetRoot.AddComponent<Animator>();
            animComp.runtimeAnimatorController = controller;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ShowNotification(new GUIContent("Main Menu Animator generated."));
        }

        private void AddTrigger(AnimatorController c, string name)
        {
            if (!c.parametersExists(name))
                c.AddParameter(name, AnimatorControllerParameterType.Trigger);
        }

        private AnimationClip CreateClosedClip()
        {
            var clip = new AnimationClip { name = "Closed" };
            var cgBinding = EditorCurveBinding.FloatCurve(string.Empty, typeof(CanvasGroup), "m_Alpha");
            AnimationUtility.SetEditorCurve(clip, cgBinding, new AnimationCurve(new Keyframe(0, 0)));
            var scaleBinding = EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.x");
            // We'll record uniform scale using x,y,z separately for reliability
            AnimationUtility.SetEditorCurve(clip, scaleBinding, new AnimationCurve(new Keyframe(0, 0.88f)));
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.y"), new AnimationCurve(new Keyframe(0, 0.88f)));
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.z"), new AnimationCurve(new Keyframe(0, 0.88f)));
            return clip;
        }

        private AnimationClip CreateOpenClip(float duration)
        {
            var clip = new AnimationClip { name = "Open" };
            clip.frameRate = 60;
            var cg = EditorCurveBinding.FloatCurve(string.Empty, typeof(CanvasGroup), "m_Alpha");
            AnimationUtility.SetEditorCurve(clip, cg, RemapCurve(fadeCurve, 0, duration));
            // Scale in using provided curve
            var sx = EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.x");
            var sy = EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.y");
            var sz = EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.z");
            AnimationUtility.SetEditorCurve(clip, sx, RemapCurve(scaleInCurve, 0, duration));
            AnimationUtility.SetEditorCurve(clip, sy, RemapCurve(scaleInCurve, 0, duration));
            AnimationUtility.SetEditorCurve(clip, sz, RemapCurve(scaleInCurve, 0, duration));
            return clip;
        }

        private AnimationClip CreateIntroClip(float duration)
        {
            var clip = new AnimationClip { name = "IntroOpen" };
            clip.frameRate = 60;
            // Slight overshoot fade using two keypoints
            var introFade = new AnimationCurve(new Keyframe(0,0,0,2f), new Keyframe(duration*0.8f,1.05f,0,0), new Keyframe(duration,1f,0,0));
            var cg = EditorCurveBinding.FloatCurve(string.Empty, typeof(CanvasGroup), "m_Alpha");
            AnimationUtility.SetEditorCurve(clip, cg, introFade);
            // Scale with stronger overshoot
            var introScale = new AnimationCurve(new Keyframe(0,0.7f,0,3f), new Keyframe(duration*0.55f,1.08f,0,0), new Keyframe(duration,1f,0,0));
            var sx = EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.x");
            var sy = EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.y");
            var sz = EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.z");
            AnimationUtility.SetEditorCurve(clip, sx, introScale);
            AnimationUtility.SetEditorCurve(clip, sy, introScale);
            AnimationUtility.SetEditorCurve(clip, sz, introScale);
            return clip;
        }

        private AnimationClip CreateCloseClip(float duration)
        {
            var clip = new AnimationClip { name = "Close" };
            clip.frameRate = 60;
            var fadeOut = new AnimationCurve(new Keyframe(0,1f,0,0), new Keyframe(duration*0.5f,0.4f,0,0), new Keyframe(duration,0f,0,0));
            var cg = EditorCurveBinding.FloatCurve(string.Empty, typeof(CanvasGroup), "m_Alpha");
            AnimationUtility.SetEditorCurve(clip, cg, fadeOut);
            var sx = EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.x");
            var sy = EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.y");
            var sz = EditorCurveBinding.FloatCurve(string.Empty, typeof(Transform), "m_LocalScale.z");
            AnimationUtility.SetEditorCurve(clip, sx, RemapCurve(scaleOutCurve, 0, duration));
            AnimationUtility.SetEditorCurve(clip, sy, RemapCurve(scaleOutCurve, 0, duration));
            AnimationUtility.SetEditorCurve(clip, sz, RemapCurve(scaleOutCurve, 0, duration));
            return clip;
        }

        private void SaveClip(AnimationClip clip, string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (!existing)
            {
                AssetDatabase.CreateAsset(clip, path);
            }
            else
            {
                // Update curves in existing clip
                var newBindings = AnimationUtility.GetCurveBindings(clip);
                foreach (var b in newBindings)
                {
                    var curve = AnimationUtility.GetEditorCurve(clip, b);
                    AnimationUtility.SetEditorCurve(existing, b, curve);
                }
                EditorUtility.SetDirty(existing);
            }
        }

        private AnimationCurve RemapCurve(AnimationCurve source, float t0, float t1)
        {
            var remap = new AnimationCurve();
            foreach (var k in source.keys)
            {
                float nt = Mathf.Lerp(t0, t1, k.time);
                var nk = new Keyframe(nt, k.value, k.inTangent, k.outTangent);
                remap.AddKey(nk);
            }
            return remap;
        }

        private AnimatorState FindOrCreateState(AnimatorStateMachine sm, string name, AnimationClip clip)
        {
            foreach (var s in sm.states)
            {
                if (s.state.name == name)
                {
                    s.state.motion = clip;
                    return s.state;
                }
            }
            var st = sm.AddState(name);
            st.motion = clip;
            return st;
        }

        private void ClearTransitions(params AnimatorState[] states)
        {
            foreach (var st in states)
            {
                // removing transitions directly isn't straightforward; we'll just remove them from the state
                st.transitions = new AnimatorStateTransition[0];
            }
        }

        private void AddTransitionAny(AnimatorStateMachine sm, AnimatorState toState, string trigger)
        {
            var t = sm.AddAnyStateTransition(toState);
            t.duration = 0.05f;
            t.hasExitTime = false;
            t.AddCondition(AnimatorConditionMode.If, 0, trigger);
        }
    }

    internal static class AnimatorControllerExtensions
    {
        public static bool parametersExists(this AnimatorController c, string name)
        {
            foreach (var p in c.parameters)
            {
                if (p.name == name) return true;
            }
            return false;
        }
    }
}
#endif
