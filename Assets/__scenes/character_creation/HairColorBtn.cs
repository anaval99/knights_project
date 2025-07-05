using UnityEngine;

public class HairColorBtn : MonoBehaviour
{
    [SerializeField]
    string hairColor;
    [SerializeField]
    CharCreationForm charCreationForm;

    UnityEngine.UI.Button selectHairColorBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.selectHairColorBtn = this.GetComponent<UnityEngine.UI.Button>();
        this.selectHairColorBtn.onClick.AddListener(SelectHairColorBtn_OnClick);
    }

    void SelectHairColorBtn_OnClick()
    {
        this.charCreationForm.SetHairColor(this.hairColor);
    }

    void OnDestroy()
    {
        this.selectHairColorBtn.onClick.RemoveListener(SelectHairColorBtn_OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
