using UnityEngine;

public static class TransformExtensions
{
    public static void ResetTransform(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void ResetForce(this Transform transform)
    {
        ResetTransform(transform);

        Rigidbody rb = transform.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

public static class GameObjectExtensions
{
    // Recursive : Àç±ÍÀû
    public static void SetActiveRecursive(this GameObject gameObject, bool isActive)
    {
        gameObject.SetActive(isActive);

        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActiveRecursive(isActive);
        }
    }
}