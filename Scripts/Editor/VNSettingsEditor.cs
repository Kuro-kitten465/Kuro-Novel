using UnityEditor;

namespace KuroNovelEdior
{
    public class VNSettingsEditor : EditorWindow
    {
        [MenuItem("Kuro Novel/VN Settings")]
        public static void ShowWindow()
        {
            GetWindow<VNSettingsEditor>("VN Settings");
        }

        private void OnEnable()
        {
            
        }
    }

    public class VNPreferencesEditor : EditorWindow
    {
        [MenuItem("Kuro Novel/VN Preferences")]
        public static void ShowWindow()
        {
            GetWindow<VNPreferencesEditor>("VN Preferences");
        }

        private void OnEnable()
        {
            
        }
    }
}