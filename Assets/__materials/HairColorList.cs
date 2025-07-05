using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "HairColorList", menuName = "Scriptable Objects/HairColorList")]
public class HairColorList : ScriptableObject
{
  [SerializeField]
  List<Material> hairColorMaterials;

  private Dictionary<string, Material> _hairColorDictionary;

  public Dictionary<string, Material> HairColorDictionary 
  {
    get
    {
      if (_hairColorDictionary == null)
      {
        _hairColorDictionary = hairColorMaterials.ToDictionary(
          material => material.name,
          material => material);
      }
      return _hairColorDictionary;
    }
  }
}
