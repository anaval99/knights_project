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
        CharInventory charInventory = null;
        CharEquipment charEquipment = null;
        // get the character data from FirebaseService.Instance
        var charAvatarTask = FirebaseService.Instance.GetSingle<CharAvatar>(FirebasePaths.Avatars, firebaseUid);
        var charInventoryTask = FirebaseService.Instance.GetSingle<CharInventory>(FirebasePaths.Inventories, firebaseUid);
        var charEquipmentTask = FirebaseService.Instance.GetSingle<CharEquipment>(FirebasePaths.Equipments, firebaseUid);
        await Task.WhenAll(charAvatarTask, charEquipmentTask, charInventoryTask);
        var charAvatar = charAvatarTask.Result;
        // init inventory if it does not exist
        if (charInventoryTask.Result == null)
        {
            charInventory = new CharInventory()
            {
                UserId = firebaseUid,
            };
            await FirebaseService.Instance.SaveSingle(FirebasePaths.Inventories, charInventory);
        }
        else
        {
            charInventory = charInventoryTask.Result;
        }
        // init equipment if it does not exist
        if (charEquipmentTask.Result == null)
        {
            charEquipment = new CharEquipment()
            {
                UserId = firebaseUid,
                Weapon = charInventory.Items.Find(x => x.ItemSOId == "beginner_sword"),
                Armor = charInventory.Items.Find(x => x.ItemSOId == "beginner_armor")
            };
            await FirebaseService.Instance.SaveSingle(FirebasePaths.Equipments, charEquipment);
        }
        else 
        {
            charEquipment = charEquipmentTask.Result;
        }
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
}
