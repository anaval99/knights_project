using System.Threading.Tasks;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField]
    private PlayerRig playerRig;
    [SerializeField]
    private PlayerRig party1Rig;
    [SerializeField]
    private PlayerRig party2Rig;
    void Start()
    {
        // Initialize combat setup
        InitializeCombatAsync();
    }

    // Update is called once per frame
    void Update()
    {

    }

    async void InitializeCombatAsync()
    {
        // disable all rigs at the start
        playerRig.gameObject.SetActive(true); // enable player rig
        party1Rig.gameObject.SetActive(false);
        party2Rig.gameObject.SetActive(false);
        // load player rig data
        string playerId = FirebaseService.Instance.GetUserId();
        await playerRig.charDataCenter.LoadCharDataAsync(playerId);
        var playerAvatar = playerRig.charDataCenter.CharAvatarObs.Value;
        string party1AvatarId = playerAvatar.Party1AvatarId;
        string party2AvatarId = playerAvatar.Party2AvatarId;
        // load party avatars
        await LoadPartyAvatarsAsync(party1AvatarId, party2AvatarId);
    }

    async Task LoadPartyAvatarsAsync(string party1AvatarId, string party2AvatarId)
    {
        var partyAvatarIds = new[] { party1AvatarId, party2AvatarId };
        var partyAvatars = await FirebaseService.Instance.QueryMany<CharAvatar>(FirebasePaths.Avatars, q => q.WhereIn("AvatarId", new[] { party1AvatarId, party2AvatarId }));
        if (party1AvatarId != null && partyAvatars.Count > 0)
        {
            var party1Avatar = partyAvatars.Find(a => a.AvatarId == party1AvatarId);
            this.party1Rig.gameObject.SetActive(true);
            await this.party1Rig.charDataCenter.LoadCharDataAsync(party1Avatar.UserId);
        }
        if (party2AvatarId != null && partyAvatars.Count > 0)
        {
            var party2Avatar = partyAvatars.Find(a => a.AvatarId == party2AvatarId);
            this.party2Rig.gameObject.SetActive(true);
            await this.party2Rig.charDataCenter.LoadCharDataAsync(party2Avatar.UserId);
        }
    }
}
