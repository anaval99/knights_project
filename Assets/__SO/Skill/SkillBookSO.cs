using UnityEngine;

[CreateAssetMenu(fileName = "SkillBookSO", menuName = "Scriptable Objects/SkillBookSO")]
public class SkillBookSO : ScriptableObject
{
  [SerializeField]
  public int Level;
  [SerializeField]
  public string SkillName;
  [SerializeField]
  public int ManaCost = 0;
  [TextArea]
  [SerializeField]
  public string Description;
  [SerializeField]
  public Sprite SkillIcon;
  [SerializeField]
  public WeaponClass WeaponClass;
  [SerializeField]
  public SkillType SkillType;
  [SerializeField]
  public TargetType TargetType = TargetType.None;
  [SerializeField]
  public int DamagePercent = 0;
  [SerializeField]
  public int HealAmount = 0;
}
