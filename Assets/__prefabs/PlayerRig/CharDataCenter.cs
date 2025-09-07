using System;
using System.Linq;
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
    public BehaviorSubject<CharAvatar> CharAvatarObs = new(null);
    #endregion Events
    [SerializeField]
    EquipmentRenderer equipmentRenderer;
    [SerializeField]
    BodyPartRenderer bodyPartRenderer;

    public async Task LoadCharDataAsync(string firebaseUid)
    {
        // get the character data from FirebaseService.Instance
        var charAvatarTask = FirebaseService.Instance.GetSingle<CharAvatar>(FirebasePaths.Avatars, firebaseUid);
        var charInventoryTask = this.GetInventoryAsync(firebaseUid);
        var charSkillBooksTask = this.GetSkillBooksAsync(firebaseUid);
        var charSkillBarTask = this.GetSkillBarAsync(firebaseUid);
        await Task.WhenAll(charAvatarTask, charInventoryTask, charSkillBooksTask, charSkillBarTask);
        var charAvatar = await charAvatarTask;
        var charInventory = await charInventoryTask;
        var charSkillBooks = await charSkillBooksTask;
        var charSkillBar = await charSkillBarTask;
        // load equipment last to ensure all items are available
        var charEquipment = await this.GetEquipmentAsync(firebaseUid, charInventory);
        // set avatar
        this.bodyPartRenderer.SetBodyPart(charAvatar);
        // set equipment
        this.equipmentRenderer.SetEquipment(charEquipment);
        // emit the loaded data
        this.CharEquipmentObs.OnNext(charEquipment);
        this.CharInventoryObs.OnNext(charInventory);
        // var fakeSkillBook = new CharSkillBooks();
        // var fakelist = charSkillBooks.SkillBooks;
        // fakeSkillBook.SkillBooks = fakelist.Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist)
        // .Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist)
        // .Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist)
        // .Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist)
        // .Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist)
        // .Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist)
        // .Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).Concat(fakelist).ToList();
        this.CharSkillBooksObs.OnNext(charSkillBooks);
        this.CharSkillBarObs.OnNext(charSkillBar);
        this.CharAvatarObs.OnNext(charAvatar);
    }

    public async Task SaveAvatar(CharAvatar avatar)
    {
        await FirebaseService.Instance.SaveSingle(FirebasePaths.Avatars, avatar);
        this.CharAvatarObs.OnNext(avatar);
    }

    public async Task SaveEquipment(CharEquipment equipment)
    {
        this.equipmentRenderer.SetEquipment(equipment);
        await FirebaseService.Instance.SaveSingle(FirebasePaths.Equipments, equipment);
        this.CharEquipmentObs.OnNext(equipment);
    }

    public async Task SaveSkillBar(CharSkillBar skillBar)
    {
        await FirebaseService.Instance.SaveSingle(FirebasePaths.SkillBars, skillBar);
        this.CharSkillBarObs.OnNext(skillBar);
    }

    public async Task SaveSkillBooks(CharSkillBooks charSkillBooks)
    {
        await FirebaseService.Instance.SaveSingle(FirebasePaths.SkillBooks, charSkillBooks);
        this.CharSkillBooksObs.OnNext(charSkillBooks);
    }

    public async Task SaveInventory(CharInventory charInventory)
    {
        await FirebaseService.Instance.SaveSingle(FirebasePaths.SkillBooks, charInventory);
        this.CharInventoryObs.OnNext(charInventory);
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
