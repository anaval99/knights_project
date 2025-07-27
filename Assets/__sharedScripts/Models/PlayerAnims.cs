using Unity.VisualScripting;
using UnityEngine;

public static class PlayerAnims
{
    public static string Attack01 = "Attack01";
    public static string Attack02 = "Attack02";
    public static string Attack03_Maintain = "Attack03_Maintain";
    public static string Attack03_Start = "Attack03_Start";
    public static string Attack04 = "Attack04";
    public static string Attack04_Spinning = "Attack04_Spinning";
    public static string Attack04_Start = "Attack04_Start";
    public static string Challenging = "Challenging";
    public static string Dance = "Dance";
    public static string Defend = "Defend";
    public static string DefendHit = "DefendHit";
    public static string Die01 = "Die01";
    public static string Die01Stay = "Die01Stay";
    public static string Die02 = "Die02";
    public static string Dizzy = "Dizzy";
    public static string GetHit01 = "GetHit01";
    public static string GetHit02 = "GetHit02";
    public static string GetUp = "GetUp";
    public static string Idle_Battle = "Idle_Battle";
    public static string Idle_Normal = "Idle_Normal";
    public static string LevelUp = "LevelUp";
    public static string SenseSomethingSearching = "SenseSomethingSearching";
    public static string SenseSomethingStart = "SenseSomethingStart";
    public static string Victory = "Victory";
    public static string Combo01_InPlace = "Combo01_InPlace";
    public static string Combo02_InPlace = "Combo02_InPlace";
    public static string Combo03_InPlace = "Combo03_InPlace";
    public static string Combo04_InPlace = "Combo04_InPlace";
    public static string Combo05_InPlace = "Combo05_InPlace";
    public static string Combo05_InPlaceWithRMHeight = "Combo05_InPlaceWithRMHeight";
    public static string DashBWD_Battle_InPlace = "DashBWD_Battle_InPlace";
    public static string DashFWD_Battle_InPlace = "DashFWD_Battle_InPlace";
    public static string DashLFT_Battle_InPlace = "DashLFT_Battle_InPlace";
    public static string DashRGT_Battle_InPlace = "DashRGT_Battle_InPlace";
    public static string JumpAir_InPlace = "JumpAir_InPlace";
    public static string JumpAirDoubleJump_InPlace = "JumpAirDoubleJump_InPlace";
    public static string JumpAirSpin_InPlace = "JumpAirSpin_InPlace";
    public static string JumpEnd_InPlace = "JumpEnd_InPlace";
    public static string JumpFull_InPlace = "JumpFull_InPlace";
    public static string JumpFullSpin_InPlace = "JumpFullSpin_InPlace";
    public static string JumpStart_InPlace = "JumpStart_InPlace";
    public static string MoveBWD_Battle_InPlace = "MoveBWD_Battle_InPlace";
    public static string MoveFWD_Battle_InPlace = "MoveFWD_Battle_InPlace";
    public static string MoveFWD_Normal_InPlace = "MoveFWD_Normal_InPlace";
    public static string MoveLFT_Battle_InPlace = "MoveLFT_Battle_InPlace";
    public static string MoveRGT_Battle_InPlace = "MoveRGT_Battle_InPlace";
    public static string RollBWD_Battle_InPlace = "RollBWD_Battle_InPlace";
    public static string RollFWD_Battle_InPlace = "RollFWD_Battle_InPlace";
    public static string RollLFT_Battle_InPlace = "RollLFT_Battle_InPlace";
    public static string RollRGT_Battle_InPlace = "RollRGT_Battle_InPlace";
    public static string SprintFWD_Battle_InPlace = "SprintFWD_Battle_InPlace";

    public static string WithWeapon(this string animName, (ItemSO, GameObject) WeaponSOGO)
    {
        return animName.WithWeapon(WeaponSOGO.Item1.WeaponClass);
    }

    public static string WithWeapon(this string animName, WeaponClass weaponClass)
    {
        string suffix = weaponClass switch
        {
            WeaponClass.Sword => "THS",
            WeaponClass.Bow => "BowAndArrow",
            WeaponClass.Wand => "MagicWand",
            _ => null
        };
        return suffix != null ? $"{animName}_{suffix}" : animName;
    }
}
