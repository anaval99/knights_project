using UnityEngine;

public class Recolor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

  void OnEnable()
  {
            var renderer = GetComponent<SkinnedMeshRenderer>();
        var material = renderer.material;
        Debug.Log("Material: " + material.name);
  }
}
