using R3;
using UnityEngine;

public class DashboardInterface : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Button openInventoryButton;
    [SerializeField]
    GameObject inventoryUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.openInventoryButton.OnClickAsObservable()
            .Subscribe(_ => this.inventoryUI.SetActive(true))
            .AddTo(this);
    }
}
