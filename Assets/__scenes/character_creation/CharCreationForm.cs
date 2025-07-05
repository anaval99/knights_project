using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharCreationForm : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    BodyPartRenderer bodyPartRenderer;
    CharAvatar avatar = new CharAvatar();
    [SerializeField]
    TMPro.TMP_InputField CharacterNameInputField;
    [SerializeField]
    UnityEngine.UI.Button SubmitBtn;
    [SerializeField]
    Alerts alerts;
    void Start()
    {
        this.bodyPartRenderer.SetBodyPart(avatar);
        this.SubmitBtn.onClick.AddListener(Submit);
    }

    void OnDestroy()
    {
        this.SubmitBtn.onClick.RemoveListener(Submit);
    }

    public void SetHair(string hair)
    {
        avatar.Hair = hair;
        this.bodyPartRenderer.SetBodyPart(avatar);
    }

    public void SetFace(string faceNum)
    {
        // 01-12
        string[] validFaces = { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
        if (!validFaces.Contains(faceNum))
        {
            Debug.LogError("Invalid face number: " + faceNum);
            return;
        }

        avatar.Eye = "Eye" + faceNum;
        avatar.Mouth = "Mouth" + faceNum;
        this.bodyPartRenderer.SetBodyPart(avatar);
    }

    public void SetHairColor(string hairColor)
    {
        avatar.HairColor = hairColor;
        this.bodyPartRenderer.SetBodyPart(avatar);
    }

    public async void Submit()
    {
        var avatarName = this.CharacterNameInputField.text;
        if (string.IsNullOrEmpty(avatarName))
        {
            Debug.LogError("Character name cannot be empty.");
            this.alerts.Error("Character name cannot be empty.");
            return;
        }
        this.SubmitBtn.interactable = false; // Disable the button to prevent multiple submissions
        this.avatar.CharacterName = this.CharacterNameInputField.text;
        await FirebaseService.Instance.SaveSingle<CharAvatar>(FirebasePaths.Avatars, this.avatar);
        Debug.Log($"Avatar submitted: {this.avatar.CharacterName} with Hair: {this.avatar.Hair}, Eye: {this.avatar.Eye}, Mouth: {this.avatar.Mouth}");
        // Proceed to the next screen or game state
        SceneManager.LoadScene("__scenes/dashboard/dashboard");
    }
}
