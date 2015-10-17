using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class GameObjectExtensions
    {
        public static bool IsActive(this GameObject gameObject)
        {
            return gameObject.activeSelf;
        }
    }

    public static class TransformExtensions
    {
        public static bool IsGameObjectActive(this Transform transform)
        {
            return transform.gameObject.activeSelf;
        }
    }
}
