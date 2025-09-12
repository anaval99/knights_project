using R3;
using UnityEngine;
using UnityEngine.UI;

public class FriendActionsUI : MonoBehaviour
{
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    TMPro.TextMeshProUGUI friendNameText;
    [SerializeField]
    Button party1Button;
    [SerializeField]
    Button party2Button;

    public BehaviorSubject<CharAvatar> SelectedFriendObs = new(null);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.SelectedFriendObs
            .Where(avatar => avatar != null)
            .Subscribe(avatar =>
            {
                this.friendNameText.text = string.Empty;
                this.friendNameText.text = avatar.CharacterName;
            })
            .AddTo(this);
        this.party1Button.OnClickAsObservable()
            .Subscribe(_ => this.AddFriendToParty(1))
            .AddTo(this);
        this.party2Button.OnClickAsObservable()
            .Subscribe(_ => this.AddFriendToParty(2))
            .AddTo(this);
    }

    void OnEnable()
    {
        this.SelectedFriendObs.OnNext(null);
    }

    async void AddFriendToParty(int partyNumber)
    {
        var selectedAvatar = this.SelectedFriendObs.Value;
        if (selectedAvatar == null)
        {
            Debug.LogWarning("No friend selected to add to party.");
            return;
        }

        var playerAvatar = charDataCenter.CharAvatarObs.Value;
        if (playerAvatar == null)
        {
            Debug.LogWarning("Player avatar is not loaded.");
            return;
        }

        if (partyNumber == 1 && playerAvatar.Party1UserId != selectedAvatar.UserId)
        {
            playerAvatar.Party1UserId = selectedAvatar.UserId;
            if (playerAvatar.Party2UserId == selectedAvatar.UserId)
            {
                playerAvatar.Party2UserId = null; // Remove from party 2 if already there
            }
        }
        else if (partyNumber == 2 && playerAvatar.Party2UserId != selectedAvatar.UserId)
        {
            playerAvatar.Party2UserId = selectedAvatar.UserId;
            if (playerAvatar.Party1UserId == selectedAvatar.UserId)
            {
                playerAvatar.Party1UserId = null; // Remove from party 1 if already there
            }
        }
        else
        {
            Debug.LogWarning($"Avatar {selectedAvatar.CharacterName} is already in party {partyNumber}.");
            return;
        }

        await charDataCenter.SaveAvatar(playerAvatar);
        Debug.Log($"Added {selectedAvatar.CharacterName} to party {partyNumber}.");
    }
}
