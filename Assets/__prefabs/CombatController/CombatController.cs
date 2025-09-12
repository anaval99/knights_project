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

public class OnHealEvent
{
    public int HP;
    public int MP;
    public SkillBookSO SkillBookSO;
    public CombatParticipant Target;
}

public class CombatController : MonoBehaviour
{
    [SerializeField]
    public PlayerRig playerRig;
    [SerializeField]
    private PlayerRig party1Rig;
    [SerializeField]
    private PlayerRig party2Rig;

    public BehaviorSubject<CombatState> CombatStateObs = new(new CombatState());
    public Subject<OnHitEvent> OnHitObs = new();
    public Subject<OnDeathEvent> OnDeathObs = new();
    public Subject<OnHealEvent> OnHealObs = new();
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
        this.ProcessSideEffects();
    }

    public void SetCombatState(CombatState state)
    {
        this.randomizer.Next(0, 100); // Ensure randomizer is initialized
        this.CombatStateObs.OnNext(state);
        // if (state.Phase == CombatPhase.BattleStart)
        // {
        //     InitBattleStart(state);
        //     Debug.Log("Combat phase set to BattleStart with " + string.Join(", ", state.ShuffledParticipants.Select(p => p.name)) + " participants.");
        // }
        // else if (state.Phase == CombatPhase.Start)
        // {
        //     Debug.Log("Combat phase set to Start.");
        // }
        // else if (state.Phase == CombatPhase.TurnEnd)
        // {
        //     if (state.EnemyParticipants.All(x => x.IsDead))
        //     {
        //         this.InitBattleEnd(state);
        //     }
        //     else
        //     {
        //         StartTurn(state);
        //     }
        // }
        // else
        // {
        //     Debug.Log("Combat phase set to " + state.Phase);
        // }
    }

    void ProcessSideEffects()
    {
        this.CombatStateObs
            .Debounce(TimeSpan.FromSeconds(0.1))
            .Subscribe(state =>
            {
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
                    else
                    {
                        StartTurn(state);
                    }
                }
                else
                {
                    Debug.Log("Combat phase set to " + state.Phase);
                }
            })
            .AddTo(this);
    }

    public void TriggerOnHit(OnHitEvent ev)
    {
        Debug.Log("TriggerOnHit");
        this.OnHitObs.OnNext(ev);
    }

    public void TriggerOnDeath(OnDeathEvent ev)
    {
        Debug.Log("TriggerOnDeath");
        this.OnDeathObs.OnNext(ev);
    }

    public void TriggerOnHeal(OnHealEvent ev)
    {
        Debug.Log("TriggerOnHeal");
        this.OnHealObs.OnNext(ev);
    }

    void InitBattleStart(CombatState currState)
    {
        var state = currState.Clone();
        // concat player and enemy participants
        state.ShuffledParticipants = state.PlayerParticipants.Concat(state.EnemyParticipants).ToList();
        // now shuffle the participants
        state.ShuffledParticipants = state.ShuffledParticipants.OrderBy(_ => randomizer.Next(0, 100)).ToList();
        // state.ShuffledParticipants = state.ShuffledParticipants.OrderBy(cp => cp.name.Contains("Player")).ToList();
        // state.ShuffledParticipants = new List<CombatParticipant>(
        //     state.EnemyParticipants.Concat(state.PlayerParticipants.Take(1))
        // );
        state.TurnIndex = -1; // reset turn index
        Observable.Timer(TimeSpan.FromSeconds(1)).Take(1).Subscribe(_ =>
        {
            // Start the first turn
            StartTurn(state);
        });
    }

    void InitBattleEnd(CombatState currState)
    {
        var state = currState.Clone();
        state.Phase = state.isFinalZone ? CombatPhase.FinalBattleEnd : CombatPhase.Patrolling;
        state.EnemyParticipants = new();
        state.TurnIndex = -1;
        state.SkillTargets = new();
        state.SelectedSkillBook = null;
        state.ShuffledParticipants = new();
        this.SetCombatState(state);
    }

    void StartTurn(CombatState currState)
    {
        var state = currState.Clone();
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
        string party1UserId = playerAvatar.Party1UserId;
        string party2UserId = playerAvatar.Party2UserId;
        // load party avatars
        await LoadPartyAvatarsAsync(party1UserId, party2UserId);
        var state = this.CombatStateObs.Value.Clone();
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

    async Task LoadParty1AvatarAsync(string party1UserId, List<CharAvatar> partyAvatars)
    {
        if (!string.IsNullOrEmpty(party1UserId) && partyAvatars.Count > 0)
        {
            var party1Avatar = partyAvatars.Find(a => a.UserId == party1UserId);
            this.party1Rig.gameObject.SetActive(true);
            await this.party1Rig.CharDataCenter.LoadCharDataAsync(party1Avatar.UserId);
        }
    }

    async Task LoadParty2AvatarAsync(string party2UserId, List<CharAvatar> partyAvatars)
    {
        if (!string.IsNullOrEmpty(party2UserId) && partyAvatars.Count > 0)
        {
            var party2Avatar = partyAvatars.Find(a => a.UserId == party2UserId);
            this.party2Rig.gameObject.SetActive(true);
            await this.party2Rig.CharDataCenter.LoadCharDataAsync(party2Avatar.UserId);
        }
    }

    async Task LoadPartyAvatarsAsync(string party1UserId, string party2UserId)
    {
        var partyAvatars = await FirebaseService.Instance.QueryMany<CharAvatar>(FirebasePaths.Avatars, q => q.WhereIn("UserId", new[] { party1UserId, party2UserId }));

        var loadParty1Task = LoadParty1AvatarAsync(party1UserId, partyAvatars);
        var loadParty2Task = LoadParty2AvatarAsync(party2UserId, partyAvatars);

        await Task.WhenAll(loadParty1Task, loadParty2Task);
    }

    public Observable<CombatState> GetMyState(CombatParticipant combatParticipant, CombatPhase combatPhase)
    {
        return this.CombatStateObs.Where(state =>
            state != null
            && state.Phase == combatPhase
            && state.ShuffledParticipants[state.TurnIndex] == combatParticipant);
    }

    public async void UseConsumableSkillBook(string consumableSOId, int qtyToUse)
    {
        var skillBooks = this.playerRig.CharDataCenter.CharSkillBooksObs.Value;
        var skillBook = skillBooks.SkillBooks.FirstOrDefault(sb => sb.SkillBookSOId == consumableSOId);
        if (skillBook != null)
        {
            skillBook.Quantity -= qtyToUse;
            await this.playerRig.CharDataCenter.SaveSkillBooks(skillBooks);
        }
    }

    public async Task ClaimLoots(List<LootedItem> lootedItems)
    {
        foreach (var lootedItem in lootedItems)
        {
            if (lootedItem.ItemSO != null)
            {
                var inventory = this.playerRig.CharDataCenter.CharInventoryObs.Value;
                var existingItem = inventory.Items.FirstOrDefault(x => x.ItemSOId == lootedItem.ItemSO.name);
                if (lootedItem.ItemSO.IsStackable && existingItem != null)
                {
                    existingItem.Quantity += lootedItem.Qty;
                }
                else
                {
                    inventory.Items.Add(new()
                    {
                        Quantity = lootedItem.Qty,
                        ItemSOId = lootedItem.ItemSO.name
                    });
                }
                await this.playerRig.CharDataCenter.SaveInventory(inventory);
            }
            else if (lootedItem.SkillBookSO != null)
            {
                var skillbooks = this.playerRig.CharDataCenter.CharSkillBooksObs.Value;
                var existingItem = skillbooks.SkillBooks.FirstOrDefault(x => x.SkillBookSOId == lootedItem.SkillBookSO.name);
                if (existingItem != null)
                {
                    existingItem.Quantity += lootedItem.Qty;
                }
                else
                {
                    skillbooks.SkillBooks.Add(new()
                    {
                        Quantity = lootedItem.Qty,
                        SkillBookSOId = lootedItem.SkillBookSO.name
                    });
                }
                await this.playerRig.CharDataCenter.SaveSkillBooks(skillbooks);
            }
        }
    }

    public async Task ClaimLootsForTeammates(List<LootedItem> lootedItems)
    {
        var playerAvatar = this.playerRig.CharDataCenter.CharAvatarObs.Value;
        var partyUserIds = new List<string>();
        if (!string.IsNullOrEmpty(playerAvatar.Party1UserId))
        {
            partyUserIds.Add(playerAvatar.Party1UserId);
        }
        if (!string.IsNullOrEmpty(playerAvatar.Party2UserId))
        {
            partyUserIds.Add(playerAvatar.Party2UserId);
        }
        if (partyUserIds.Count == 0) return;

        var partyAvatars = await FirebaseService.Instance.QueryMany<CharAvatar>(FirebasePaths.Avatars, q => q.WhereIn("UserId", partyUserIds.ToArray()));

        foreach (var avatar in partyAvatars)
        {
            var mail = new CharMail
            {
                UserId = avatar.UserId,
                Subject = "Loot from Battle",
                Message = "Your share of the loot from the recent battle.",
                Attachments = new List<CharMailAttachment>()
            };

            foreach (var lootedItem in lootedItems)
            {
                var attachment = new CharMailAttachment
                {
                    Qty = lootedItem.Qty
                };

                if (lootedItem.ItemSO != null)
                {
                    attachment.ItemSOId = lootedItem.ItemSO.name;
                }
                else if (lootedItem.SkillBookSO != null)
                {
                    attachment.SkillBookSOId = lootedItem.SkillBookSO.name;
                }

                mail.Attachments.Add(attachment);
            }

            await FirebaseService.Instance.SaveSingleToList(FirebasePaths.Mails, mail);
        }
    }
}
