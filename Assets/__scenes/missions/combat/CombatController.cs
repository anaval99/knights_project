using System.Collections.Generic;
using System.Threading.Tasks;
using R3;
using UnityEngine;

public class CombatController : MonoBehaviour
{
    [SerializeField]
    private PlayerRig playerRig;
    [SerializeField]
    private PlayerRig party1Rig;
    [SerializeField]
    private PlayerRig party2Rig;

    public BehaviorSubject<CombatState> CombatStateObs = new(new CombatState());
    void Start()
    {
        // Initialize combat setup
        InitializeCombatAsync();
    }

    public void SetCombatState(CombatState state)
    {
        this.CombatStateObs.OnNext(state);
    }

    async void InitializeCombatAsync()
    {
        // disable all rigs at the start
        playerRig.gameObject.SetActive(true); // enable player rig
        party1Rig.gameObject.SetActive(false);
        party2Rig.gameObject.SetActive(false);
        // load player rig data
        string playerId = FirebaseService.Instance.GetUserId();
        await playerRig.CharDataCenter.LoadCharDataAsync(playerId);
        var playerAvatar = playerRig.CharDataCenter.CharAvatarObs.Value;
        string party1AvatarId = playerAvatar.Party1AvatarId;
        string party2AvatarId = playerAvatar.Party2AvatarId;
        // load party avatars
        await LoadPartyAvatarsAsync(party1AvatarId, party2AvatarId);
        var state = this.CombatStateObs.Value;
        state.Phase = CombatPhase.Start; // set initial combat phase
        this.SetCombatState(state);
        var allRigs = new List<PlayerRig> { playerRig };
        if (party1Rig.gameObject.activeSelf)
        {
            allRigs.Add(party1Rig);
        }
        if (party2Rig.gameObject.activeSelf)
        {
            allRigs.Add(party2Rig);
        }
        // Initialize combat handlers for all rigs
        foreach (var rig in allRigs)
        {
            rig.PlayerCombatHandler.InitializeCombatHandler(this.CombatStateObs);
        }
    }

    async Task LoadParty1AvatarAsync(string party1AvatarId, List<CharAvatar> partyAvatars)
    {
        if (party1AvatarId != null && partyAvatars.Count > 0)
        {
            var party1Avatar = partyAvatars.Find(a => a.AvatarId == party1AvatarId);
            this.party1Rig.gameObject.SetActive(true);
            await this.party1Rig.CharDataCenter.LoadCharDataAsync(party1Avatar.UserId);
        }
    }

    async Task LoadParty2AvatarAsync(string party2AvatarId, List<CharAvatar> partyAvatars)
    {
        if (party2AvatarId != null && partyAvatars.Count > 0)
        {
            var party2Avatar = partyAvatars.Find(a => a.AvatarId == party2AvatarId);
            this.party2Rig.gameObject.SetActive(true);
            await this.party2Rig.CharDataCenter.LoadCharDataAsync(party2Avatar.UserId);
        }
    }

    async Task LoadPartyAvatarsAsync(string party1AvatarId, string party2AvatarId)
    {
        var partyAvatars = await FirebaseService.Instance.QueryMany<CharAvatar>(FirebasePaths.Avatars, q => q.WhereIn("AvatarId", new[] { party1AvatarId, party2AvatarId }));

        var loadParty1Task = LoadParty1AvatarAsync(party1AvatarId, partyAvatars);
        var loadParty2Task = LoadParty2AvatarAsync(party2AvatarId, partyAvatars);

        await Task.WhenAll(loadParty1Task, loadParty2Task);
    }
}
