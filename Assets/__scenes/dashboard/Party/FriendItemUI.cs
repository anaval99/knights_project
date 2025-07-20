using R3;
using UnityEngine;

public class FriendItemUI : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI friendNameText;
    [SerializeField]
    TMPro.TextMeshProUGUI partyStatusText;
    [SerializeField]
    UnityEngine.UI.Image backgroundImage;
    [SerializeField]
    FriendActionsUI friendActionsUI;
    [SerializeField]
    CharDataCenter charDataCenter;

    private CharAvatar friendAvatar;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var button = this.GetComponent<UnityEngine.UI.Button>();
        button.OnClickAsObservable()
            .Subscribe(_ =>
            {
                this.friendActionsUI.SelectedFriendObs.OnNext(friendAvatar);
            })
            .AddTo(this);
        this.friendActionsUI.SelectedFriendObs
            .Subscribe(this.SetFocusStatus)
            .AddTo(this);
        this.charDataCenter.CharAvatarObs
            .Subscribe(avatar => this.SetPartyStatus(avatar))
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetFocusStatus(CharAvatar selectedAvatar)
    {
        var color = Color.black;
        this.backgroundImage.color = Color.black; // Reset color
        if (selectedAvatar != null && this.friendAvatar != null && selectedAvatar.AvatarId == this.friendAvatar.AvatarId)
        {
            // Highlight the selected friend
            color = Color.yellow; // Example highlight color
        }
        else if (this.friendAvatar != null)
        {
            // Reset if no friend is selected or if avatar is null
            color = Color.white; // Default color
        }
        this.backgroundImage.color = color;
    }

    void SetPartyStatus(CharAvatar playerAvatar)
    {
        string statusText = string.Empty;
        if (this.friendAvatar != null && playerAvatar != null)
        {
            if (playerAvatar.Party1AvatarId == this.friendAvatar.AvatarId)
            {
                statusText = "P1";
            }
            else if (playerAvatar.Party2AvatarId == this.friendAvatar.AvatarId)
            {
                statusText = "P2";
            }
        }
        this.partyStatusText.text = statusText;
    }

    public void Render(CharAvatar avatar)
    {
        this.friendAvatar = avatar;
        var color = Color.black;
        string friendName = string.Empty;
        if (avatar != null)
        {
            color = Color.white;
            friendName = avatar.CharacterName;
        }
        this.friendNameText.text = friendName;
        this.backgroundImage.color = color;
        this.SetPartyStatus(this.charDataCenter.CharAvatarObs.Value);
        this.SetFocusStatus(this.friendActionsUI.SelectedFriendObs.Value);
    }
}
