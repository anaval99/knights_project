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
        var charEquipment = await this.GetEquipmentAsync(firebaseUid, charInventory);
        // set avatar
        this.bodyPartRenderer.SetBodyPart(charAvatar);
        // set equipment
        this.equipmentRenderer.SetEquipment(charEquipment);
        this.CharEquipmentObs.OnNext(charEquipment);
        this.CharInventoryObs.OnNext(charInventory);
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
}
