using R3;
using UnityEngine;
using UnityEngine.UI;

public class FriendSearcher : MonoBehaviour
{
    [SerializeField]
    CharDataCenter charDataCenter;
    [SerializeField]
    private TMPro.TMP_InputField avatarIdInput;
    [SerializeField]
    private Button addFriendButton;
    [SerializeField]
    Alerts alerts;

    private bool isSearching = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.addFriendButton.OnClickAsObservable()
            .Subscribe(this.AddFriend)
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    async void AddFriend(Unit unit)
    {
        if (isSearching) return;
        string avatarId = avatarIdInput.text;
        var playerAvatar = charDataCenter.CharAvatarObs.Value;
        // dont add self avatar
        if (playerAvatar.AvatarId == avatarId)
        {
            Debug.LogWarning("Cannot add self as friend.");
            alerts.Error("Cannot add self as friend.");
            return;
        }
        if (playerAvatar == null)
        {
            Debug.LogWarning("Player avatar is not loaded.");
            return;
        }
        // Check will be done after finding the avatar

        isSearching = true;
        if (string.IsNullOrEmpty(avatarId))
        {
            Debug.LogWarning("Avatar ID is empty.");
            alerts.Error("Avatar ID cannot be empty.");
            isSearching = false;
            return;
        }

        var avatars = await FirebaseService.Instance.QueryMany<CharAvatar>(FirebasePaths.Avatars, q => q.WhereEqualTo("AvatarId", avatarId));
        if (avatars.Count == 0)
        {
            Debug.LogWarning($"No avatar found with ID: {avatarId}");
            alerts.Error($"No avatar found with ID: {avatarId}");
            isSearching = false;
            return;
        }
        CharAvatar avatar = avatars[0];
        if (playerAvatar.FriendUserIds.Contains(avatar.UserId))
        {
            Debug.LogWarning("This user is already in your friend list.");
            alerts.Error("This user is already in your friend list.");
            isSearching = false;
            return;
        }
        playerAvatar.FriendUserIds.Add(avatar.UserId);
        await charDataCenter.SaveAvatar(playerAvatar);

        isSearching = false;
    }
}
