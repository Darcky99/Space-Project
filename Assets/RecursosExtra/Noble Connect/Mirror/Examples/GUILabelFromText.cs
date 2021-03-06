using UnityEngine;

namespace NobleConnect.Mirror
{
    public class GUILabelFromText : MonoBehaviour
    {
        //public TextAsset textFile;
        public Texture2D textBackground;
        public Vector2 position;
        // text;

        void Start()
        {
            //text = textFile.text;
        }

        void OnGUI()
        {
            if (!NobleServer.active && !NobleClient.active)
            { 
                var style = new GUIStyle("label");
                style.normal.background = textBackground;
                //style.normal.textColor = Color.black;
                style.padding = new RectOffset(10, 10, 10, 10);
                Rect labelRect = GUILayoutUtility.GetRect(new GUIContent("reyo "), style);
                GUI.Label(new Rect(position.x, position.y, labelRect.width, labelRect.height), "Reyo", style);
            }
        }
    }
}