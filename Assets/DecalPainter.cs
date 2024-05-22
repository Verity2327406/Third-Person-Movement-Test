using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DecalPainter : MonoBehaviour
{
    [SerializeField]
    private DecalTextureData[] decalData;

    [SerializeField]
    private GameObject decalProjectorPrefab;

    [SerializeField]
    private int selectedDecalIndex;

    [SerializeField]
    private Image decalImage;

    Material[] decalMaterials;

    private void Start()
    {
        decalMaterials = new Material[decalData.Length];
        selectedDecalIndex = 0;
        foreach (Image image in FindObjectsByType<Image>(FindObjectsSortMode.None))
        {
            if (image.CompareTag("Decal"))
            {
                decalImage = image;
                break;
            }
        }
        decalImage.sprite = decalData[selectedDecalIndex].sprite;
    }

    public void PaintDecal(Vector3 point, Vector3 normal, Collider collider)
    {
        GameObject decal = Instantiate(decalProjectorPrefab, point, Quaternion.identity);
        DecalProjector projector = decal.GetComponent<DecalProjector>();
        if (decalMaterials[selectedDecalIndex] == null)
        {
            decalMaterials[selectedDecalIndex] = new Material(projector.material);
        }
        projector.material = decalMaterials[selectedDecalIndex];
        projector.material.SetTexture("Base_Map", decalData[selectedDecalIndex].sprite.texture);
        projector.size = decalData[selectedDecalIndex].size;
        decal.transform.forward = -normal;
    }
}

[Serializable]
public class DecalTextureData
{
    public Sprite sprite;
    public Vector3 size;
}
