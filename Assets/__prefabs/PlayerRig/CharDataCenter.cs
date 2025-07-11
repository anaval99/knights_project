using System;
using System.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Events;

public class CharDataCenter : MonoBehaviour
{
    #region Events
    public BehaviorSubject<CharEquipment> CharEquipmentObs = new(null);
    public BehaviorSubject<CharInventory> CharInventoryObs = new(null);
    public BehaviorSubject<CharSkillBooks> CharSkillBooksObs = new(null);
    public BehaviorSubject<CharSkillBar> CharSkillBarObs = new(null);
    #endregion Events
    [SerializeField]
    EquipmentRenderer equipmentRenderer;
    [SerializeField]
    BodyPartRenderer bodyPartRenderer;

    public async Task LoadCharDataAsync(string firebaseUid)
    {
        // get the character data from FirebaseService.Instance
        var charAvatar = await FirebaseService.Instance.GetSingle<CharAvatar>(FirebasePaths.Avatars, firebaseUid);
        var charInventory = await this.GetInventoryAsync(firebaseUid);
        var charSkillBooks = await this.GetSkillBooksAsync(firebaseUid);
        var charSkillBar = await this.GetSkillBarAsync(firebaseUid);
        var charEquipment = await this.GetEquipmentAsync(firebaseUid, charInventory);
        // set avatar
        this.bodyPartRenderer.SetBodyPart(charAvatar);
        // set equipment
        this.equipmentRenderer.SetEquipment(charEquipment);
        // emit the loaded data
        this.CharEquipmentObs.OnNext(charEquipment);
        this.CharInventoryObs.OnNext(charInventory);
        this.CharSkillBooksObs.OnNext(charSkillBooks);
        this.CharSkillBarObs.OnNext(charSkillBar);
    }

    public async Task SaveEquipment(CharEquipment equipment)
    {
        this.equipmentRenderer.SetEquipment(equipment);
        await FirebaseService.Instance.SaveSingle(FirebasePaths.Equipments, equipment);
        this.CharEquipmentObs.OnNext(equipment);
    }

    public async Task<CharInventory> GetInventoryAsync(string firebaseUid)
    {
        var charInventory = await FirebaseService.Instance.GetSingle<CharInventory>(FirebasePaths.Inventories, firebaseUid);
        if (charInventory == null)
        {
            charInventory = new CharInventory() { UserId = firebaseUid };
            await FirebaseService.Instance.SaveSingle(FirebasePaths.Inventories, charInventory);
        }
        return charInventory;
    }
    
    public async Task<CharEquipment> GetEquipmentAsync(string firebaseUid, CharInventory charInventory)
    {
        var charEquipment = await FirebaseService.Instance.GetSingle<CharEquipment>(FirebasePaths.Equipments, firebaseUid);
        if (charEquipment == null)
        {
            charEquipment = new CharEquipment()
            {
                UserId = firebaseUid,
                Weapon = charInventory.Items.Find(x => x.ItemSOId == "beginner_sword"),
                Armor = charInventory.Items.Find(x => x.ItemSOId == "beginner_armor")
            };
            await FirebaseService.Instance.SaveSingle(FirebasePaths.Equipments, charEquipment);
        }
        return charEquipment;
    }

    public async Task<CharSkillBooks> GetSkillBooksAsync(string firebaseUid)
    {
        var charSkillBooks = await FirebaseService.Instance.GetSingle<CharSkillBooks>(FirebasePaths.SkillBooks, firebaseUid);
        if (charSkillBooks == null)
        {
            charSkillBooks = new CharSkillBooks() { UserId = firebaseUid };
            await FirebaseService.Instance.SaveSingle(FirebasePaths.SkillBooks, charSkillBooks);
        }
        return charSkillBooks;
    }

    public async Task<CharSkillBar> GetSkillBarAsync(string firebaseUid)
    {
        var charSkillBar = await FirebaseService.Instance.GetSingle<CharSkillBar>(FirebasePaths.SkillBars, firebaseUid);
        if (charSkillBar == null)
        {
            charSkillBar = new CharSkillBar() { UserId = firebaseUid };
            await FirebaseService.Instance.SaveSingle(FirebasePaths.SkillBars, charSkillBar);
        }
        return charSkillBar;
    }
}
