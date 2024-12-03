using KuroNovel.DataNode;
using KuroNovel.Manager;
using TMPro;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [SerializeField] private VNSequence sequence;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    public void ONNN() => VNManager.Instance.StartVN(sequence);
}
