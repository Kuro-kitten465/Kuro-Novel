using KuroNovel.DataNode;
using KuroNovel.Manager;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] private VNSequence sequence;

    private void OnGUI()
    {
        if (GUILayout.Button("Test"))
        {
            VNManager.Instance.StartVN(sequence);
        }
    }
}
