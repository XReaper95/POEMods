using JetBrains.Annotations;
using UnityEngine;
using UnityModManagerNet;

namespace HidePanelsWithHotkey
{
    internal static class Entrypoint
    {
        private static readonly KeyBinding LKey = new KeyBinding() { keyCode = KeyCode.L };
        private static readonly KeyBinding SemiColonKey = new KeyBinding() { keyCode = KeyCode.Semicolon };

        [CanBeNull]
        private static UIHUDMinimizeButton _consoleMinimizeButton;
        [CanBeNull]
        private static UIHUDMinimizeButton ConsoleMinimizeButton
        {
            get
            {
                if (_consoleMinimizeButton == null)
                {
                    _consoleMinimizeButton = GetMinimizeButton("ConsoleMinimize");
                }

                return _consoleMinimizeButton;
            }
        }

        [CanBeNull]
        private static UIHUDMinimizeButton _actionBarMinimizeButton;
        [CanBeNull]
        private static UIHUDMinimizeButton ActionBarMinimizeButton
        {
            get
            {
                if (_actionBarMinimizeButton == null)
                {
                    _actionBarMinimizeButton = GetMinimizeButton("ActionBarMinimize");
                }

                return _actionBarMinimizeButton;
            }
        }

        // ReSharper disable once UnusedMember.Local
        private static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnUpdate = OnUpdate;
            return true;
        }

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if (LKey.Down())
            {
                if (ConsoleMinimizeButton != null)
                {
                    ButtonHiderOnClick(ConsoleMinimizeButton);
                }
            }

            // ReSharper disable once InvertIf
            if (SemiColonKey.Down())
            {
                if (ActionBarMinimizeButton != null)
                {
                    ButtonHiderOnClick(ActionBarMinimizeButton);
                }
            }
        }

        private static UIHUDMinimizeButton GetMinimizeButton(string gameObjectName)
        {
            GameObject gameObject = GameObject.Find(gameObjectName);
            return gameObject.GetComponent<UIHUDMinimizeButton>();
        }

        private static void ButtonHiderOnClick(UIHUDMinimizeButton button)
        {
            button.MinimizedState = !button.MinimizedState;
            foreach (GameObject gameObject in button.Hide)
            {
                UIPanel component = gameObject.GetComponent<UIPanel>();
                if ((bool) (Object) component)
                    component.alpha = button.MinimizedState ? 0.0f : 1f;
                else
                    gameObject.SetActive(!button.MinimizedState);
            }
            foreach (UITweener uiTweener in button.HideTween)
                uiTweener.Play(button.MinimizedState);
            ButtonHiderUpdateAnchor(button);
        }

        private static void ButtonHiderUpdateAnchor(UIHUDMinimizeButton button)
        {
            if ((bool) (Object) button.ActiveAnchor)
                button.ActiveAnchor.enabled = !button.MinimizedState;
            if ((bool) (Object) button.HiddenAnchor)
                button.HiddenAnchor.enabled = button.MinimizedState;
            if ((bool) (Object) button.ActiveAnchor && button.ActiveAnchor.enabled)
                button.ActiveAnchor.Update();
            if (!(bool) (Object) button.HiddenAnchor || !button.HiddenAnchor.enabled)
                return;
            button.HiddenAnchor.Update();
        }
    }
}
