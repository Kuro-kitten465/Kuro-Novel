using UnityEngine;
using Newtonsoft.Json;

namespace KuroNovel
{
    public sealed class VNSettings : MonoBehaviour
    {
        private static readonly VNSettings m_Instance = new VNSettings();
        public static VNSettings Instance => m_Instance;

        static VNSettings()
        {

        }

        public VNSettings()
        {

        }
    }
}
