using UnityEngine;

namespace Core
{
    public class DraggablePanel : MonoBehaviour
    {
        public void Update()
        {
            if (Screen == null) return;

            var mousePos = Input.mousePosition;

            if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
            {
                if (Screen.GetMouseOver)
                {
                    Offset = Input.mousePosition - Screen.transform.position;

                    _isDragging = true;
                }
            }

            if (_isDragging && Input.GetMouseButtonUp(0))
            {
                _isDragging = false;

                SavePosition(mousePos - Offset);
            }

            if (!_isDragging) return;

            var newPosition = mousePos - Offset;

            SetPosition(newPosition);
        }

        private bool _isDragging;

        // Use GetComponent<KScreen>() instead?
        public KScreen Screen;

        public Vector3 Offset;

        public static void Attach(KScreen screen)
        {
            var panel = screen.FindOrAddUnityComponent<DraggablePanel>();

            if (panel == null) return;

            panel.Screen = screen;
        }

        // TODO: call when position is set by game
        public static void SetPositionFromFile(KScreen screen)
        {
            Vector2 newPosition;

            var panel = screen.FindOrAddUnityComponent<DraggablePanel>();

            if (panel != null && panel.LoadPosition(out newPosition))
            {
                panel.SetPosition(newPosition);
            }
        }

        // TODO: queue save to file
        private void SavePosition(Vector2 position)
        {
            State.UIState.SaveWindowPosition(gameObject, position);
        }

        private bool LoadPosition(out Vector2 position)
        {
            return State.UIState.LoadWindowPosition(gameObject, out position);
        }

        // use offset?
        private void SetPosition(Vector3 newPosition)
        {
            if (Screen == null) return;

            Screen.transform.position = newPosition;
        }
    }
}
