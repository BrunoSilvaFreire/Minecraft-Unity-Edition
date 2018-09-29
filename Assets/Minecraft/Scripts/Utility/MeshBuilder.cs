using System.Collections.Generic;
using UnityEngine;

namespace Minecraft.Scripts.Utility {
    public class MeshBuilder {
        public List<Vector3> Vertices {
            get;
        } = new List<Vector3>();

        public List<int> Indices {
            get;
        } = new List<int>();

        public List<Vector2> Uvs {
            get;
        } = new List<Vector2>();

        public Mesh Build() {
            var mesh = new Mesh {
                vertices = Vertices.ToArray(),
                triangles = Indices.ToArray(),
                uv = Uvs.ToArray()
            };
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            return mesh;
        }

        public void AddFace(Vector3 corner, Vector3 up, Vector3 right, bool reversed) {
            var index = Vertices.Count;

            Vertices.Add(corner);
            Vertices.Add(corner + up);
            Vertices.Add(corner + up + right);
            Vertices.Add(corner + right);

            var uvWidth = new Vector2(1, 1);
            var uvCorner = new Vector2(0, 0);


            Uvs.Add(uvCorner);
            Uvs.Add(new Vector2(uvCorner.x, uvCorner.y + uvWidth.y));
            Uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y + uvWidth.y));
            Uvs.Add(new Vector2(uvCorner.x + uvWidth.x, uvCorner.y));

            if (reversed) {
                Indices.Add(index + 0);
                Indices.Add(index + 1);
                Indices.Add(index + 2);
                Indices.Add(index + 2);
                Indices.Add(index + 3);
                Indices.Add(index + 0);
            } else {
                Indices.Add(index + 1);
                Indices.Add(index + 0);
                Indices.Add(index + 2);
                Indices.Add(index + 3);
                Indices.Add(index + 2);
                Indices.Add(index + 0);
            }
        }
    }
}