using UnityEngine;

public class HairSelectBtn : MonoBehaviour
{
    [SerializeField]
    string hairNum;
    [SerializeField]
    CharCreationForm charCreationForm;
    [SerializeField]
    UnityEngine.UI.Button hairSelectBtn;
    [SerializeField]
    TMPro.TextMeshProUGUI hairNumText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.hairNum = ParserService.GetSeriesNumberFromGameObjectName(this.gameObject);
        this.hairNumText.text = this.hairNum;
        this.hairSelectBtn.onClick.AddListener(OnClickSelectHairBtn);
    }

    void OnClickSelectHairBtn()
    {
        this.charCreationForm.SetHair("Hair" + this.hairNum);
    }

    // Update is called once per frame
    void OnDestroy()
    {
        this.hairSelectBtn.onClick.RemoveListener(OnClickSelectHairBtn);
    }
}
