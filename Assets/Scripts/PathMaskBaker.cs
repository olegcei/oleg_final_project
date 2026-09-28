using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class PathMaskBaker : MonoBehaviour
{
    public SplineContainer spline;
    public MeshFilter terrainMeshFilter;
    public float maskRadius = 5f;       // distance at which mask falls to 0
    public float falloffPower = 2f;     // controls softness of the edge
    public int splineSampleCount = 200; // resolution of path sampling

    [ContextMenu("Bake Mask")]
    public void Bake()
    {
        Mesh mesh = terrainMeshFilter.sharedMesh;
        Vector3[] verts = mesh.vertices;
        Color[] colors = new Color[verts.Length];

        // Pre-sample the spline into world-space points once
        Vector3[] splinePoints = new Vector3[splineSampleCount];
        for (int i = 0; i < splineSampleCount; i++)
        {
            float t = i / (float)(splineSampleCount - 1);
            splinePoints[i] = spline.EvaluatePosition(t);
        }

        Transform terrainTransform = terrainMeshFilter.transform;

        for (int v = 0; v < verts.Length; v++)
        {
            Vector3 worldVert = terrainTransform.TransformPoint(verts[v]);
            float minDist = float.MaxValue;

            // Nearest-sample distance (fine for a first pass;
            // see note below on smarter nearest-segment distance)
            for (int i = 0; i < splinePoints.Length; i++)
            {
                float d = Vector3.Distance(worldVert, splinePoints[i]);
                if (d < minDist) minDist = d;
            }

            float mask = 1f - Mathf.Clamp01(minDist / maskRadius);
            mask = Mathf.Pow(mask, falloffPower);

            // Store mask in R channel, leave G/B/A free for other data
            colors[v] = new Color(mask, 0, 0, 1);
        }

        mesh.colors = colors;
        Debug.Log("Path mask baked into vertex colors.");
    }
}