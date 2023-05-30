﻿using UnityEditor;
using UnityEngine;

namespace NAK.AASEmulator.Editor
{
    public static class EditorExtensions
    {
        public static void HandlePopupScroll(ref int newIndex, int minIndex, int maxIndex)
        {
            if (Event.current.type == EventType.ScrollWheel && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                if (Event.current.delta.y < 0)
                {
                    newIndex = Mathf.Clamp(newIndex + 1, minIndex, maxIndex);
                    Event.current.Use();
                }
                else if (Event.current.delta.y > 0)
                {
                    newIndex = Mathf.Clamp(newIndex - 1, minIndex, maxIndex);
                    Event.current.Use();
                }
            }
        }

        public static float joystickClickTime;
        public static Vector2 Joystick2DField(Rect position, Vector2 value, bool shouldNormalize = false)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Event currentEvent = Event.current;

            // Joystick area
            Rect joystickArea = new Rect(position.x, position.y, position.width, position.height);

            // Draw the background
            EditorGUI.DrawRect(joystickArea, Color.grey);

            // Draw the handle
            Vector2 handlePosition = new Vector2(value.x * (joystickArea.width / 2), -value.y * (joystickArea.height / 2));
            Vector2 screenHandlePosition = joystickArea.center + handlePosition;
            Handles.color = Color.white;
            Handles.DrawSolidDisc(screenHandlePosition, Vector3.forward, 6);

            // Handle input
            if (currentEvent.type == EventType.MouseDown && joystickArea.Contains(currentEvent.mousePosition))
            {
                GUIUtility.hotControl = controlID;
                currentEvent.Use();

                // Double-click reset
                if ((Time.realtimeSinceStartup - joystickClickTime) < 0.5f)
                {
                    value = Vector2.zero;
                }
                joystickClickTime = Time.realtimeSinceStartup;
            }
            else if (currentEvent.type == EventType.MouseUp && GUIUtility.hotControl == controlID)
            {
                GUIUtility.hotControl = 0;
                currentEvent.Use();
            }
            else if (currentEvent.type == EventType.MouseDrag && GUIUtility.hotControl == controlID)
            {
                Vector2 newJoystickPosition = currentEvent.mousePosition - joystickArea.center;
                newJoystickPosition.x /= joystickArea.width / 2;
                newJoystickPosition.y /= -joystickArea.height / 2;
                value = new Vector2(Mathf.Clamp(newJoystickPosition.x, -1, 1), Mathf.Clamp(newJoystickPosition.y, -1, 1));
                currentEvent.Use();
            }

            if (shouldNormalize)
            {
                if (value.magnitude > value.normalized.magnitude)
                {
                    value.Normalize();
                }
            }

            return value;
        }
    }
}
