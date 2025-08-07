using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Firestore;
using R3;
using UnityEngine;

public class OnDeathEvent
{
    public string TaskID = Guid.NewGuid().ToString();
    public CombatParticipant DeadParticipant;
}

public class OnHitEvent
{
    public string TaskID = Guid.NewGuid().ToString();
    public CombatParticipant Source;
    public CombatParticipant Target;
    public SkillBookSO SkillBookSO;
    public int Damage;
}

public class CombatController : MonoBehaviour
{
    [SerializeField]
    private PlayerRig playerRig;
    [SerializeField]
    private PlayerRig party1Rig;
    [SerializeField]
    private PlayerRig party2Rig;

    public BehaviorSubject<CombatState> CombatStateObs = new(new CombatState());
    public Subject<OnHitEvent> OnHitObs = new();
    public Subject<OnDeathEvent> OnDeathObs = new();
    public CombatParticipant CurrentParticipant
    {
        get => this.CombatStateObs.Value.ShuffledParticipants[this.CombatStateObs.Value.TurnIndex];
    }
    public CombatState State => this.CombatStateObs.Value;

    private System.Random randomizer = new();
    void Start()
    {
        // Initialize combat setup
        InitializeCombatAsync();
    }

    public void SetCombatState(CombatState state)
    {
        this.randomizer.Next(0, 100); // Ensure randomizer is initialized
        this.CombatStateObs.OnNext(state);
        if (state.Phase == CombatPhase.BattleStart)
        {
            InitBattleStart(state);
            Debug.Log("Combat phase set to BattleStart with " + string.Join(", ", state.ShuffledParticipants.Select(p => p.name)) + " participants.");
        }
        else if (state.Phase == CombatPhase.Start)
        {
            Debug.Log("Combat phase set to Start.");
        }
        else if (state.Phase == CombatPhase.TurnEnd)
        {
            if (state.EnemyParticipants.All(x => x.IsDead))
            {
                this.InitBattleEnd(state);
            }
            StartTurn(state);
        }
        else
        {
            Debug.Log("Combat phase set to " + state.Phase);
        }
    }

    public void TriggerOnHit(OnHitEvent ev)
    {
        Debug.Log("TriggerOnHit");
        this.OnHitObs.OnNext(ev);
    }

    public void TriggerOnDeath(OnDeathEvent ev)
    {
        Debug.Log("TriggerOnHit");
        this.OnDeathObs.OnNext(ev);
    }

    void InitBattleStart(CombatState state)
    {
        // concat player and enemy participants
        state.ShuffledParticipants = state.PlayerParticipants.Concat(state.EnemyParticipants).ToList();
        // now shuffle the participants
        state.ShuffledParticipants = state.ShuffledParticipants.OrderBy(_ => randomizer.Next(0, 100)).ToList();
        state.TurnIndex = -1; // reset turn index
        Observable.Timer(TimeSpan.FromSeconds(1)).Take(1).Subscribe(_ =>
        {
            // Start the first turn
            StartTurn(state);
        });
    }

    void InitBattleEnd(CombatState state)
    {
        state.Phase = state.isFinalZone ? CombatPhase.FinalBattleEnd : CombatPhase.Patrolling;
        state.EnemyParticipants = new();
        state.TurnIndex = -1;
        state.SkillTargets = new();
        state.SelectedSkillBook = null;
        state.ShuffledParticipants = new();
        this.SetCombatState(state);
    }

    void StartTurn(CombatState state)
    {
        state.TurnIndex = (state.TurnIndex + 1) % state.ShuffledParticipants.Count; // increment turn index and wrap around
        while (state.ShuffledParticipants[state.TurnIndex].CurrentHealth == 0) // if dead
        {
            state.TurnIndex = (state.TurnIndex + 1) % state.ShuffledParticipants.Count; // increment turn index and wrap around
        }
        var currentParticipant = state.ShuffledParticipants[state.TurnIndex];
        Debug.Log($"Starting turn for {currentParticipant.name}.");
        // Notify the participant to take their turn
        Debug.Log($"Current participant: {currentParticipant.name}, Turn Index: {state.TurnIndex}");
        // Set the phase to TurnStart
        state.Phase = CombatPhase.TurnStart;
        state.SelectedSkillBook = null; // reset selected skill book
        state.SkillTargets = new();
        this.SetCombatState(state);
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
        state.PlayerParticipants.AddRange(allRigs.Select(r => r.PlayerCombatParticipant));
        this.SetCombatState(state);
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
