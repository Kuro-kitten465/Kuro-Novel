using UnityEditor;
using UnityEngine;
using KuroNovel;

namespace KuroNovelEdior
{
    public class VNSettingsEditor : EditorWindow
    {
        [MenuItem("Kuro Novel/VN Settings")]
        public static void ShowWindow()
        {
            GetWindow<VNSettingsEditor>("VN Settings");
        }

        private VNSettings settings;

        private void OnEnable()
        {
            settings = VNSettingsManager.GetSettings();
        }

        private void OnGUI()
        {
            if (settings == null)
            {
                GUILayout.Label("VNSettings asset not found. Please create one in the Resources folder.", EditorStyles.boldLabel);
                return;
            }

            GUILayout.Label("Default Asset File Path", EditorStyles.boldLabel);
            settings.CharactersFolder = EditorGUILayout.TextField("Characters Folder", settings.CharactersFolder);
            settings.BackgroundsFolder = EditorGUILayout.TextField("Backgrounds Folder", settings.BackgroundsFolder);
            settings.SpritesFolder = EditorGUILayout.TextField("Sprites Folder", settings.SpritesFolder);

            GUILayout.Space(10);
            GUILayout.Label("Backend Data Handle Mode", EditorStyles.boldLabel);
            settings.AssetHandler = (VNSettings.AssetHandlerMode)EditorGUILayout.EnumPopup("Asset Handle", settings.AssetHandler);

            //GUILayout.Space(10);

            GUILayout.Space(20);
            if (GUILayout.Button("Save Settings"))
            {
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                Debug.Log("VNSettings saved!");
            }
        }
    }
}