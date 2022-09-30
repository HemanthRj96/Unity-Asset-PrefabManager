using System.Linq;
using UnityEngine;


namespace Lacobus.Prefabs
{
    public class PrefabManager : ScriptableObject
    {
        // Fields

        [SerializeField] private PrefabContainer[] _prefabContainer;


        // Public methods

        /// <summary>
        /// Returns a prefab based on the prefab tag
        /// </summary>
        /// <param name="prefabTag">Tag given to the prefab</param>
        public GameObject GetPrefab(string prefabTag)
        {
            var container = _prefabContainer.ToList().Find(a => a.prefabTag == prefabTag);

            if (container != null)
                return container.prefab;
            return null;
        }

        /// <summary>
        /// Returns a prefab based on the prefab id
        /// </summary>
        /// <param name="prefabId">Id given to the prefab</param>
        public GameObject GetPrefab(int prefabId)
        {
            var container = _prefabContainer.ToList().Find(a => a.prefabId == prefabId);

            if (container != null)
                return container.prefab;
            return null;
        }


        // Nested types

        [System.Serializable]
        public class PrefabContainer
        {
            public string prefabTag;
            public int prefabId;
            public GameObject prefab;
        }
    }
}