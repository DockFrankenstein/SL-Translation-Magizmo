using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Utility.UI
{
    [ExecuteAlways]
    public class DeformUI : MonoBehaviour
    {
        [Label("Corners")]
        public Vector3 TL;
        public Vector3 TR;
        public Vector3 BL;
        public Vector3 BR;

        [Label("Rendering")]
        public Material material;
        public Vector2Int size = new Vector2Int(1920, 1080);

        [Label("Components")]
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] MeshFilter meshFilter;
        [SerializeField] Camera cam;

        Mesh mesh;
        RenderTexture rt;
        Material _mat = null;

        bool _changeMeshNextUpdate;

        private void Reset()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
        }

        private void Update()
        {
            if (_changeMeshNextUpdate && meshFilter != null)
            {
                _changeMeshNextUpdate = false;
                meshFilter.mesh = mesh;
            }
        }

        private void OnValidate()
        {
            _mat = null;
            rt = new RenderTexture(size.x, size.y, 4);
            cam.targetTexture = rt;

            UpdateMesh();
        }

        private void OnEnable()
        {
            _mat = null;
            rt = new RenderTexture(size.x, size.y, 4);
            cam.targetTexture = rt;

            UpdateMesh();
        }

        public void UpdateMesh()
        {
            if (_mat == null)
            {
                if (material == null) return;
                _mat = new Material(material);
                _mat.mainTexture = rt;
                meshRenderer.sharedMaterial = _mat;
            }

            mesh = new Mesh();

            Vector3[] verticies = new Vector3[]
            {
                TL,
                TR,
                BL,
                BR,
            };

            mesh.vertices = verticies;

            int[] tris = new int[]
            {
                1, 2, 0,
                1, 3, 2,
            };

            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
            };

            mesh.uv = uv;
            _changeMeshNextUpdate = true;
        }

        private void OnDisable()
        {

        }

        private void OnDrawGizmosSelected()
        {
            const float radius = 0.01f;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + TL, radius);
            Gizmos.DrawSphere(transform.position + TR, radius);
            Gizmos.DrawSphere(transform.position + BL, radius);
            Gizmos.DrawSphere(transform.position + BR, radius);
        }
    }
}