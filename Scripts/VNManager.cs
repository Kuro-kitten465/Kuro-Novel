using KuroNovel.DataNode;
using Newtonsoft.Json;
using UnityEngine;
using KuroNovel.Utils;
using KuroNovel.StateMachine;
using System.IO;
using UnityEngine.SceneManagement;
using System;

namespace KuroNovel.Manager
{
    public class VNManager : MonoSingleton<VNManager>
    {
        private VNStateMachine stateMachine;
        private VNSequence currentSequence;

        private const string VNPanelSceneName = "Kuro-Novel";

        public override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        /// Starts the VN system by loading a sequence and displaying the VNPanel scene.
        /// </summary>
        /// <param name="sequencePath">The path to the VN sequence JSON file.</param>
        public void StartVN(VNSequence sequence)
        {
            LoadVNPanelScene(() =>
            {
                //currentSequence = LoadSequence(sequence);
                currentSequence = sequence;
                stateMachine = new VNStateMachine(currentSequence);
                stateMachine.Start();
            });
        }

        private VNSequence LoadSequence(string path)
        {
            /*if (!File.Exists(path))
            {
                Debug.LogError($"Sequence file not found at path: {path}");
                return null;
            }*/

            /*
            var s = Resources.Load<TextAsset>(path);
            string json = File.ReadAllText(s.text);
            JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return JsonConvert.DeserializeObject<VNSequence>(json, settings);*/
            return Resources.Load<VNSequence>(path);
        }

        private void LoadVNPanelScene(System.Action onComplete)
        {
            if (SceneManager.GetSceneByName(VNPanelSceneName).isLoaded)
            {
                onComplete?.Invoke();
                return;
            }

            SceneManager.LoadScene(VNPanelSceneName, LoadSceneMode.Additive);

            // Wait until the scene is loaded
            SceneManager.sceneLoaded += OnSceneLoaded;

            void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                if (scene.name == VNPanelSceneName)
                {
                    SceneManager.sceneLoaded -= OnSceneLoaded;
                    onComplete?.Invoke();
                }
            }
        }

        private void Update()
        {
            stateMachine?.Update();
        }
    }
}
