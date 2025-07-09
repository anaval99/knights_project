using R3;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Button closeButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.closeButton.OnClickAsObservable()
            .Subscribe(_ => this.gameObject.SetActive(false))
            .AddTo(this);
    }
}
