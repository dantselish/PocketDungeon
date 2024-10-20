using System.Collections.Generic;
using UnityEngine;

public class GridMesh : MonoBehaviour {
        public MeshFilter filter;
        public Vector2Int gridSize;
        public float gridSpacing;

        Mesh mesh;
        List<Vector3> verticies;
        List<int> indices;

        private void Start() {
            mesh = new Mesh();
            MeshRenderer meshRenderer = filter.GetComponent<MeshRenderer>();

            Rebuild();
        }

        private void Update() {
            Rebuild();
        }

        private void Rebuild() {
            verticies = new List<Vector3>();
            indices = new List<int>();

            float xMin = gridSpacing * gridSize.x / 2f;
            float zMin = gridSpacing * gridSize.y / 2f;

            for (int i = 0; i <= gridSize.x; i++) {
                for (int j = 0; j <= gridSize.y; j++) {
                    float x1 = i * gridSpacing - xMin;
                    float x2 = (i + 1) * gridSpacing - xMin;
                    float z1 = j * gridSpacing - zMin;
                    float z2 = (j + 1) * gridSpacing - zMin;

                    if (i != gridSize.x) {
                        verticies.Add(new Vector3(x1, 0, z1));
                        verticies.Add(new Vector3(x2, 0, z1));
                    }

                    if (j != gridSize.y) {
                        verticies.Add(new Vector3(x1, 0, z1));
                        verticies.Add(new Vector3(x1, 0, z2));
                    }
                }
            }

            int indicesCount = verticies.Count;
            for (int i = 0; i < indicesCount; i++) {
                indices.Add(i);
            }

            mesh.vertices = verticies.ToArray();
            mesh.SetIndices(indices.ToArray(), MeshTopology.Lines, 0);
            filter.mesh = mesh;
        }
}
