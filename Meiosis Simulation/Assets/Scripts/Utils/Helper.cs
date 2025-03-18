using UnityEngine;

public static class Helper 
{
    public static void ChangeColorRecursively(Transform parent, Color color)
    {
        if (parent == null) return;

        // Change the color of the current object if there is a Renderer
        Renderer renderer = parent.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
        // Recursively go through all children
        foreach (Transform child in parent)
        {
            ChangeColorRecursively(child, color);
        }
    }
}
