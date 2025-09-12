using System.Collections.Generic;
using UnityEngine;
using R3;
using System;

public class FriendList : ListContainer
{
    [SerializeField]
    List<FriendItemUI> friendItemUIs;
    [SerializeField]
    CharDataCenter charDataCenter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.PopulateListItems(this.friendItemUIs, 18);
        this.charDataCenter.CharAvatarObs
            .Subscribe(this.FetchAndRenderFriends)
            .AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void FetchAndRenderFriends(CharAvatar avatar)
    {
        if (avatar == null || avatar.FriendUserIds == null || avatar.FriendUserIds.Count == 0)
        {
            Debug.LogWarning("No friends to render.");
            this.friendItemUIs.ForEach(itemUI => itemUI.Render(null));
            return;
        }

        var friendAvatars = await FirebaseService.Instance.QueryMany<CharAvatar>(
            FirebasePaths.Avatars,
            q => q.WhereIn("UserId", avatar.FriendUserIds)
        );

        this.RenderItems(this.friendItemUIs, friendAvatars, 0, (itemUI, friendAvatar) =>
        {
            itemUI.Render(friendAvatar);
        });   
    }
}
