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
    private void Start()
    {
        this.charDataCenter.CharEquipmentObs
            .Where(e => e != null)
            .Select(e => e.Weapon)
            .DistinctUntilChanged()
            .Select(weapon =>
            {
                var (so, go) = this.equipmentList.GetSOGO(weapon);
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

        // Modern switch expression (C# 8.0+)
        var clip = weaponSO.WeaponClass switch
        {
            WeaponClass.Sword => this.animationList.GetClip(SwordAnims.Idle_Battle_THS),
            WeaponClass.Bow => this.animationList.GetClip(ArrowAnims.Idle_Battle_BowAndArrow),
            WeaponClass.Wand => this.animationList.GetClip(WandAnims.Idle_Battle_MagicWand),
            _ => null
        };

        if (clip != null)
        {
            this.animancerComponent.Play(clip);
        }
    }
}
