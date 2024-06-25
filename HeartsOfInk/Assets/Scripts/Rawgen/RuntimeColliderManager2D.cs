using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rawgen_Studio_Models;
using Rawgen_DataAccess;
using Assets.Scripts.Rawgen.Models;
using System.Linq;

/// <summary>
/// Gestiona colliders dinámicos en 2D.
/// 
/// Nota: No funciona en Android debido al uso de streamingAssets.
/// </summary>
public class RuntimeColliderManager2D : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private string spriteName = string.Empty;
    private PolygonCollider2D polygonCollider;
    private List<CachedCollider2D> cachedColliders;
    private float lastScale = 100;

    // Editable desde editor.
    public float scale = 100;
    public bool cacheEnabled = true;
    public bool adjustOffset = true;
    public string collidersFolder;

    // TODO Properties.
    //public bool negativeAxisY = false;
    //public bool negativeAxisX = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = this.transform.GetComponent<SpriteRenderer>();
        polygonCollider = this.transform.GetComponent<PolygonCollider2D>();
        cachedColliders = new List<CachedCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (spriteRenderer.sprite.name != spriteName ||
            scale != lastScale)
        {
            lastScale = scale;
            spriteName = spriteRenderer.sprite.name;

            if (!LoadCachedCollider(spriteName))
            {
                LoadCollider(spriteName);
            }
        }
    }

    /// <summary>
    /// En caso de que la caché esté habilitada busca si el collider ya ha sido cargado anteriormente y lo usa.
    /// </summary>
    /// <param name="spriteName"> Nombre del srpite. </param>
    /// <returns> True si se ha cargado el collider desde la caché, false en caso contrario. </returns>
    private bool LoadCachedCollider(string spriteName)
    {
        bool loaded = false;

        if (cacheEnabled)
        {
            CachedCollider2D cachedCollider = BinarySearch(spriteName);

            if (cachedCollider != null)
            {
                loaded = true;
                polygonCollider.offset = cachedCollider.offset;
                polygonCollider.SetPath(0, cachedCollider.edges);
            }
        }

        return loaded;
    }

    private CachedCollider2D BinarySearch(string searchedValue)
    {
        int medium;
        int lowRange;
        int highRange;
        CachedCollider2D targetFinded = null;
        CachedCollider2D evaluating;

        if (cachedColliders.Count > 2)
        {
            lowRange = 0;
            highRange = cachedColliders.Count - 1;

            while (targetFinded == null && lowRange <= highRange)
            {
                medium = (lowRange + highRange) / 2;
                evaluating = cachedColliders[medium];

                if (evaluating.spriteName.CompareTo(searchedValue) == 0)
                {
                    targetFinded = evaluating;
                    break;
                }
                else if (evaluating.spriteName.CompareTo(searchedValue) < 0)
                {
                    lowRange = medium + 1;
                }
                else
                {
                    highRange = medium - 1;
                }
            }
        }
        else if (cachedColliders.Count == 1)
        {
            if (cachedColliders[0].spriteName == searchedValue)
            {
                targetFinded = cachedColliders[0];
            }
        }
        else if (cachedColliders.Count == 2)
        {
            if (cachedColliders[0].spriteName == searchedValue)
            {
                targetFinded = cachedColliders[0];
            }
            else if (cachedColliders[1].spriteName == searchedValue)
            {
                targetFinded = cachedColliders[1];
            }
        }

        return targetFinded;
    }

    private void LoadCollider(string spriteName)
    {
        JsonFileDAC jsonDac = new JsonFileDAC();
        ColliderFileModel colliderFileModel;
        string path = Application.streamingAssetsPath + "/Colliders/" + collidersFolder + spriteName + ".rwcol";
        List<Vector2> edges;
        Rawgen.Math.Objects.Vectors.Vector2Int resolution;

        colliderFileModel = jsonDac.LoadFile<ColliderFileModel>(path);
        edges = new List<Vector2>();
        resolution = colliderFileModel.Resolution;

        if (adjustOffset)
        {
            polygonCollider.offset = new Vector2(0 - (resolution.X / 2 / scale), 0 - (resolution.Y / 2 / scale));
        }

        for (int index = 0; index < colliderFileModel.Edges.Count; index++)
        {
            Rawgen.Math.Objects.Vectors.Vector2Int vector = colliderFileModel.Edges[index];
            edges.Add(new Vector2(vector.X / scale, vector.Y / scale));
        }

        polygonCollider.SetPath(0, edges.ToArray());

        if (cacheEnabled)
        {
            CachedCollider2D cachedCollider2D = new CachedCollider2D();
            cachedCollider2D.offset = polygonCollider.offset;
            cachedCollider2D.edges = edges.ToArray();
            cachedCollider2D.spriteName = spriteName;

            cachedColliders.Add(cachedCollider2D);
            cachedColliders.Sort();
        }
    }
}
