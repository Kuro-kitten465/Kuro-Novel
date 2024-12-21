using UnityEngine;

namespace KuroNovel
{
    public sealed class VNSettings : ScriptableObject
    {
        public string CharactersFolder = "Assets/Resources/VNAssets/Characters/";
        public string BackgroundsFolder = "Assets/Resources/VNAssets/Backgrounds/";
        public string SpritesFolder = "Assets/Resources/VNAssets/Sprites/";
        public string SequencesFolder = "Assets/Kuro-Novel/Resources/VNAssets/Sequence/";

        public enum AssetHandlerMode { Resources }

        public AssetHandlerMode AssetHandler = AssetHandlerMode.Resources;

        //Default Value
        public readonly string res_CharactersPath = "Assets/Resources/VNAssets/Characters/";
        public readonly string res_BackgroundsPath = "Assets/Resources/VNAssets/Backgrounds/";
        public readonly string res_SpritesPath = "Assets/Resources/VNAssets/Sprites/";
    }

    public static class VNSettingsManager
    {
        private static VNSettings cachedSettings;

        public static VNSettings GetSettings()
        {
            if (cachedSettings == null)
            {
                cachedSettings = Resources.Load<VNSettings>("VNSettings");
                if (cachedSettings == null)
                {
                    Debug.LogError("VNSettings asset not found in Resources folder.");
                }
            }
            return cachedSettings;
        }
    }
}
