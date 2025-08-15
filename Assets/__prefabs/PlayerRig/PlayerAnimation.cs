using Animancer;
using R3;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    public AnimationList animationList;
    [SerializeField]
    public AnimancerComponent animancerComponent;
    [SerializeField]
    public CharDataCenter charDataCenter;
    [SerializeField]
    public EquipmentList equipmentList;
    [SerializeField]
    public RxStateMachine rxStateMachine;

    public (ItemSO, GameObject) weaponSOGO;
    private void Start()
    {
        this.charDataCenter.CharEquipmentObs
            .Where(e => e != null)
            .Select(e => e.Weapon)
            .DistinctUntilChanged()
            .Select(weapon =>
            {
                var (so, go) = this.equipmentList.GetSOGO(weapon);
                this.weaponSOGO = (so, go);
                return so;
            })
            .Subscribe(this.onChangeWeapon)
            .AddTo(this);
    }

    void onChangeWeapon(ItemSO weaponSO)
    {
        if (weaponSO == null)
        {
            Debug.LogWarning("Weapon is null, cannot change animation.");
            return;
        }

        this.PlayIdleAnimation();
    }

    public void PlayIdleAnimation()
    {
        var clip = this.animationList.GetClip(PlayerAnims.Idle_Battle.WithWeapon(this.weaponSOGO));
        var idleState = new IdleState(
            this.animancerComponent,
            clip
        );
        this.rxStateMachine.SetState(idleState);
    }

    public void PlayPatrollingAnimation()
    {
        var patrolState = new PatrolState(
             this.animancerComponent,
             this.weaponSOGO.Item1,
             this.animationList
         );
        this.rxStateMachine.SetState(patrolState);
    }
}
