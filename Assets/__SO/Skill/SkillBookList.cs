using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillBookList", menuName = "Scriptable Objects/SkillBookList")]
public class SkillBookList : ScriptableObject
{
  [SerializeField]
  List<SkillBookSO> skillBooks = new();

  private Dictionary<string, SkillBookSO> skillBookDictionary;
  public Dictionary<string, SkillBookSO> SkillBookDictionary
  {
    get
    {
      if (skillBookDictionary == null)
      {
        skillBookDictionary = new Dictionary<string, SkillBookSO>();
        foreach (var skillBook in skillBooks)
        {
          if (skillBook != null)
          {
            skillBookDictionary[skillBook.name] = skillBook;
            Debug.Log($"Added SkillBook: {skillBook.name} to dictionary");
          }
        }
      }
      return skillBookDictionary;
    }
  }

  void OnValidate()
  {
    if (skillBooks != null && skillBooks.Count > 0)
    {
      this.skillBooks = this.skillBooks.DistinctBy(x => x.name).ToList();
    }
  }
}
