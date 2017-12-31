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

            if (Input.GetMouseButtonUp(0))
            {
                _isDragging = false;
            }

            if (!_isDragging) return;


            var newPosition = mousePos - Offset;

            Screen.transform.position = newPosition;
        }

        private bool _isDragging = false;

        // Use GetComponent<KScreen>() instead?
        public KScreen Screen;

        public Vector3 Offset = new Vector3();

        public static void Attach(KScreen screen)
        {
            var draggable = screen.FindOrAddUnityComponent<DraggablePanel>();

            draggable.Screen = screen;
        }
    }
}
