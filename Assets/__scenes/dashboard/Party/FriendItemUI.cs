using R3;
using UnityEngine;

public class FriendItemUI : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI friendNameText;
    [SerializeField]
    UnityEngine.UI.Image backgroundImage;
    [SerializeField]
    FriendActionsUI friendActionsUI;

    private CharAvatar avatar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var button = this.GetComponent<UnityEngine.UI.Button>();
        button.OnClickAsObservable()
            .Subscribe(_ =>
            {
                this.friendActionsUI.SelectedFriendObs.OnNext(avatar);
            })
            .AddTo(this);
        this.friendActionsUI.SelectedFriendObs
            .Subscribe(selectedAvatar =>
            {
                var color = Color.black;
                this.backgroundImage.color = Color.black; // Reset color
                if (selectedAvatar != null && this.avatar != null && selectedAvatar.AvatarId == this.avatar.AvatarId)
                {
                    // Highlight the selected friend
                    color = Color.yellow; // Example highlight color
                }
                else if (this.avatar != null)
                {
                    // Reset if no friend is selected or if avatar is null
                    color = Color.white; // Default color
                }
                this.backgroundImage.color = color;
            })
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Render(CharAvatar avatar)
    {
        this.avatar = avatar;
        var color = Color.black;
        string friendName = string.Empty;
        if (avatar != null)
        {
            color = Color.white;
            friendName = avatar.CharacterName;
        }
        this.friendNameText.text = friendName;
        this.backgroundImage.color = color;
    }
}
