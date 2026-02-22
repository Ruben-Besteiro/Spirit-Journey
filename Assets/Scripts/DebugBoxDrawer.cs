using UnityEngine;

public class DebugBoxDrawer : MonoBehaviour
{
    private float lifetime;
    private float timer;

    private Vector3 startScale;
    [SerializeField] private MeshRenderer meshRenderer;

    /// <summary>
    /// Dibuja una caja con posición, tamaño y rotación
    /// </summary>
    public static void DrawBox(Vector3 center, Vector3 size, Quaternion rotation, Color color, float duration = 1f)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "DEBUG_BOX";

        Object.Destroy(cube.GetComponent<Collider>());

        cube.transform.position = center;
        cube.transform.rotation = rotation;
        cube.transform.localScale = size;

        var drawer = cube.AddComponent<DebugBoxDrawer>();
        drawer.Initialize(color, duration);
    }

    /// <summary>
    /// Dibuja desde Bounds (sin rotación)
    /// </summary>
    public static void DrawBox(Bounds bounds, Color color, float duration = 1f)
    {
        DrawBox(bounds.center, bounds.size, Quaternion.identity, color, duration);
    }

    /// <summary>
    /// Dibuja desde Collider (sin rotación)
    /// </summary>
    public static void DrawBox(Collider col, Color color, float duration = 1f)
    {
        DrawBox(col.bounds, color, duration);
        Debug.Log("BoxDraw");
    }

    private void Initialize(Color color, float duration)
    {
        lifetime = duration;
        timer = duration;
        startScale = transform.localScale;

        meshRenderer = GetComponent<MeshRenderer>();

        // material no iluminado simple
        meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        meshRenderer.material.color = color;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        float t = Mathf.Clamp01(timer / lifetime);

        // Escala decreciente
        transform.localScale = startScale * t;

        // Fade opcional (suave)
        Color c = meshRenderer.material.color;
        c.a = t;
        meshRenderer.material.color = c;

        if (timer <= 0f)
            Destroy(gameObject);
    }
}