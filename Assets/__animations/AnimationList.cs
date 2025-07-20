using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "AnimationList", menuName = "Scriptable Objects/AnimationList")]
public class AnimationList : ScriptableObject
{
  [SerializeField]
  private List<Object> animationFBXFiles = new List<Object>(); // Change to Object to accept FBX files
  [SerializeField]
  public List<AnimationClip> AnimationClips = new List<AnimationClip>();

  private Dictionary<string, AnimationClip> animationDictionary;
  public Dictionary<string, AnimationClip> AnimationDictionary
  {
    get
    {
      if (animationDictionary == null)
      {
        animationDictionary = this.AnimationClips.ToDictionary(
          clip => clip.name,
          clip => clip);
      }
      return animationDictionary;
    }
  }

  public AnimationClip GetClip(string name)
  {
    this.AnimationDictionary.TryGetValue(name, out var clip);
    if (clip == null)
    {
      Debug.LogWarning($"Animation clip '{name}' not found in AnimationList.");
    }
    return clip;
  }

  void OnValidate()
  {
    if (animationFBXFiles == null || animationFBXFiles.Count == 0)
    {
      Debug.LogWarning("Animation list is empty.");
      return;
    }

    this.animationFBXFiles = this.animationFBXFiles.DistinctBy(a => a.name).ToList();
    this.AnimationClips = this.animationFBXFiles.Select(fbx =>
    {
      string assetPath = AssetDatabase.GetAssetPath(fbx);
      Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
      var clip = assets.OfType<AnimationClip>().FirstOrDefault(clip => clip.name == fbx.name);
      return clip;
    }).ToList();
  }
}
