using UnityEngine;

public class FaceSelectBtn : MonoBehaviour
{
    [SerializeField]
    string faceNum;
    [SerializeField]
    CharCreationForm charCreationForm;
    [SerializeField]
    UnityEngine.UI.Button SelectFaceBtn;
    [SerializeField]
    TMPro.TextMeshProUGUI FaceNumText;


    void Start()
    {
        this.SelectFaceBtn.onClick.AddListener(OnClickSelectFaceBtn);
        // Set the face number text
        this.FaceNumText.text = this.faceNum;
    }

    void OnClickSelectFaceBtn()
    {
        this.charCreationForm.SetFace(this.faceNum);
    }

    void OnDestroy()
    {
        this.SelectFaceBtn.onClick.RemoveListener(OnClickSelectFaceBtn);   
    }
}
