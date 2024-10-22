using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[ExecuteInEditMode]
public class FloorGeneratorTool : MonoBehaviour
{
    [SerializeField] private Transform  Parent;
    [SerializeField] private List<Tile> Tiles;
    [SerializeField] private Vector2Int GridSize;
    [SerializeField] private Vector2Int TileSize;
    [SerializeField] private bool       RotateTiles;


    public void Generate()
    {
        PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);
        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                int randomIndex = Random.Range(0, Tiles.Count);

                GameObject tileGo = (GameObject)PrefabUtility.InstantiatePrefab(Tiles[randomIndex].gameObject, Parent);

                float positionX = transform.position.x + TileSize.x * (x + 0.5f);
                float positionY = transform.position.y;
                float positionZ = transform.position.z + TileSize.y * (y + 0.5f);
                Vector3 position = new Vector3(positionX, positionY, positionZ);
                tileGo.transform.position = position;

                float rotationY = Random.Range(1, 5) * (RotateTiles ? 90f : 180f);
                Vector3 rotationEuler = new Vector3(0f, rotationY, 0f);
                tileGo.transform.rotation = Quaternion.Euler(rotationEuler);

                tileGo.name = $"Tile ({x},{y})";
            }
        }
        PrefabUtility.RecordPrefabInstancePropertyModifications(this.gameObject);

        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }
}
