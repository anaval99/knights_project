using Animancer;
using R3;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private AnimationList animationList;
    [SerializeField]
    private AnimancerComponent animancerComponent;
    [SerializeField]
    private CharDataCenter charDataCenter;
    [SerializeField]
    private EquipmentList equipmentList;
    [SerializeField]
    private RxStateMachine rxStateMachine;

    private (ItemSO, GameObject) weaponSOGO;
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

        var idleState = new IdleState(
            this.animancerComponent,
            weaponSO,
            this.animationList
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
