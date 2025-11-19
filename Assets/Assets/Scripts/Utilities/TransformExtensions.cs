using System.Collections;
using UnityEngine;

public static class TransformExtensions
{
    /// <summary>
    /// Smoothly interpolates the scale of a Transform over a given duration.
    /// </summary>
    /// <param name="transform">The transform to scale.</param>
    /// <param name="targetScale">The target scale.</param>
    /// <param name="duration">The duration of the interpolation.</param>
    /// <returns>IEnumerator for coroutine usage.</returns>
    public static IEnumerator LerpScaleOverTime(this Transform transform, Vector3 targetScale, float duration)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = transform.localScale;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
}