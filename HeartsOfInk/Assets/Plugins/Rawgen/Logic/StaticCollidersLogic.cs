using Rawgen.Unity.Editor.Models;
using Rawgen.Unity.Editor.Utils;
using Rawgen_DataAccess;
using Rawgen_Studio_Models;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Rawgen.Unity.Editor.Logic
{
    /// <summary>
    /// Script para el manejo de coliders.
    /// 
    /// Nota: No funciona en Android debido al uso de streamingAssets.
    /// </summary>
    public class StaticCollidersLogic
    {
        public GameObject gameObject { get; set; }
        private PolygonCollider2D polygonCollider;

        public void LoadGameObject(GameObject newGameObject, StaticCollidersOptionsModel options)
        {
            if (gameObject == null || !gameObject.Equals(newGameObject))
            {
                gameObject = newGameObject;
                polygonCollider = gameObject.GetComponent<PolygonCollider2D>();
                LoadPolygonCollider();

                if (options.loadAutomatically)
                {
                    AutoSearchCollider(gameObject.name, options);
                }
            }
        }

        private void LoadPolygonCollider()
        {
            if (polygonCollider == null)
            {
                polygonCollider = (PolygonCollider2D)gameObject.AddComponent(typeof(PolygonCollider2D));
            }
        }

        private void AutoSearchCollider(string spriteName, StaticCollidersOptionsModel options)
        {
            if (File.Exists(Application.streamingAssetsPath + "/Colliders/" + spriteName + ".rwcol"))
            {
                options.colliderPath = "/Colliders/" + spriteName + ".rwcol";
                LoadCollider(options);
            }
        }

        public void LoadCollider(StaticCollidersOptionsModel options)
        {
            JsonFileDAC jsonDac = new JsonFileDAC();
            ColliderFileModel colliderFileModel;
            string path;
            List<Vector2> edges;
            Rawgen.Math.Objects.Vectors.Vector2Int resolution;
            float scale = options.scale;

            LoadPolygonCollider();
            path = FoldersUtils.AddStreamingAssetsPath(options.colliderPath);
            colliderFileModel = jsonDac.LoadFile<ColliderFileModel>(path);
            edges = new List<Vector2>();
            resolution = colliderFileModel.Resolution;

            if (options.overwriteOptions)
            {
                options.offset = new Vector2(0 - (resolution.X / 2 / scale), 0 - (resolution.Y / 2 / scale));
            }
            
            polygonCollider.offset = options.offset;

            for (int index = 0; index < colliderFileModel.Edges.Count; index++)
            {
                Rawgen.Math.Objects.Vectors.Vector2Int vector = colliderFileModel.Edges[index];
                edges.Add(new Vector2(vector.X / scale, vector.Y / scale));
            }

            polygonCollider.SetPath(0, edges.ToArray());
        }

        public void SaveCollider(StaticCollidersOptionsModel options)
        {
            JsonFileDAC jsonDac = new JsonFileDAC();
            ColliderFileModel colliderFileModel;
            string fullPath;

            fullPath = FoldersUtils.AddStreamingAssetsPath(options.colliderPath);
            colliderFileModel = new ColliderFileModel();

            for (int pathIndex = 0; pathIndex < polygonCollider.pathCount; pathIndex++)
            {
                Vector2[] vectors = polygonCollider.GetPath(pathIndex);

                for (int edgeIndex = 0; edgeIndex < vectors.Length; edgeIndex++)
                {
                    Vector2 unityEdge = vectors[edgeIndex];
                    Rawgen.Math.Objects.Vectors.Vector2Int edge = new Math.Objects.Vectors.Vector2Int();

                    edge.X = Convert.ToInt32(unityEdge.x * options.scale);
                    edge.Y = Convert.ToInt32(unityEdge.y * options.scale);

                    colliderFileModel.Edges.Add(edge);
                }
            }

            jsonDac.SaveFile(colliderFileModel, fullPath);
        }
    }
}
