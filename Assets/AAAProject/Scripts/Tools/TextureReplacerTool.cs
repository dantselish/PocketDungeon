using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class TextureReplacerTool : MonoBehaviour
{
    [SerializeField] private Material OldMaterial;
    [SerializeField] private Material NewMaterial;


    public void ReplaceWithNewMaterial()
    {
        var objects = GetComponentsInChildren<MeshRenderer>().Where(x => x.sharedMaterial == OldMaterial);
        foreach (MeshRenderer meshRenderer in objects)
        {
            meshRenderer.material = NewMaterial;
        }
        OldMaterial = NewMaterial;
    }
}
