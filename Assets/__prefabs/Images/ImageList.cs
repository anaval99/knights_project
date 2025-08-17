using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ImageList", menuName = "Scriptable Objects/ImageList")]
public class ImageList : ScriptableObject
{
  [SerializeField]
  Sprite[] images;

  private Dictionary<string, Sprite> imageDictionary;
  public Dictionary<string, Sprite> ImageDictionary
  {
    get
    {
      if (imageDictionary == null)
      {
        imageDictionary = new Dictionary<string, Sprite>();
        foreach (var image in images)
        {
          if (image != null && !string.IsNullOrEmpty(image.name))
          {
            imageDictionary[image.name] = image;
          }
        }
      }
      return imageDictionary;
    }
  }

  void OnValidate()
  {
    if (this.images != null && this.images?.Length > 0)
    {
      this.images = this.images.DistinctBy(image => image.name).ToArray();
    }
  }

  public Sprite GetRarityFrame(Rarity rarity)
  {
    string prefix = null;
    switch (rarity)
    {
      case Rarity.Common:
        prefix = "ItemFrame_02_Bg_Single_Lime";
        break;
      case Rarity.Rare:
        prefix = "ItemFrame_02_Bg_Single_Blue";
        break;
      case Rarity.Epic:
        prefix = "ItemFrame_02_Bg_Single_Yellow";
        break;
      case Rarity.Legendary:
        prefix = "ItemFrame_02_Bg_Single_Pink";
        break;
    }
    if (string.IsNullOrEmpty(prefix) || !this.ImageDictionary.ContainsKey(prefix))
    {
      return this.ImageDictionary["ItemFrame_02_Bg_Single_Brown"]; // brown if rarity not found
    }
    return this.ImageDictionary[prefix];
  }
}
