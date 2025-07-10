using R3;
using UnityEngine;

public class DescriptionPopup : MonoBehaviour
{
    [SerializeField]
    UnityEngine.UI.Button OpenButton;
    [SerializeField]
    UnityEngine.UI.Button CloseButton;
    [SerializeField]
    GameObject LayoutPanel;
    void Start()
    {
        this.OpenButton?.OnClickAsObservable()
            .Subscribe(_ =>
            {
                this.LayoutPanel.SetActive(true);
            })
            .AddTo(this);
        this.CloseButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                this.LayoutPanel.SetActive(false);
            })
            .AddTo(this);
    }
}
