using UnityEngine;

public class BodyPartRenderer : MonoBehaviour
{
    [SerializeField]
    GameObjectList bodyPartList;
    [SerializeField]
    HairColorList hairColorList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetBodyPart(CharAvatar avatar)
    {
        // turn off all body parts
        foreach (GameObject bodyPart in bodyPartList.gameObjects)
        {
            if (bodyPart != null)
            {
                bodyPart.SetActive(false);
            }
        }

        // turn on the body parts specified in the avatar
        var bodyPartDict = bodyPartList.GetGameObjectDictionary();
        if (bodyPartDict.TryGetValue(avatar.Hair, out GameObject hair))
        {
            hair.SetActive(true);
            var hairRenderer = hair.GetComponent<MeshRenderer>();
            var hairMaterial = hairColorList.HairColorDictionary[avatar.HairColor];
            if (hairRenderer != null && hairMaterial != null)
            {
                hairRenderer.material = hairMaterial;
            }
            else
            {
                Debug.LogError("Hair renderer or material not found for: " + avatar.Hair);
            }
        }
        if (bodyPartDict.TryGetValue(avatar.Eye, out GameObject eye))
        {
            eye.SetActive(true);
        }
        if (bodyPartDict.TryGetValue(avatar.Mouth, out GameObject mouth))
        {
            mouth.SetActive(true);
        }
    }
}
