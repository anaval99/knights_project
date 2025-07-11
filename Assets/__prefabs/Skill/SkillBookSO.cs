using UnityEngine;

[CreateAssetMenu(fileName = "SkillBookSO", menuName = "Scriptable Objects/SkillBookSO")]
public class SkillBookSO : ScriptableObject
{
  [SerializeField]
  public int Level;
  [SerializeField]
  public string SkillId;
  [SerializeField]
  public string SkillName;
  [SerializeField]
  public int ActionPoints;
  [SerializeField]
  public string Description;
  [SerializeField]
  public Sprite SkillIcon;
  [SerializeField]
  public WeaponClass WeaponClass;
}
