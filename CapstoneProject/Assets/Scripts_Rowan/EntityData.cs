using UnityEngine;

namespace lilGuysNamespace
{

    [CreateAssetMenu(fileName = "EntityData", menuName = "Scriptable Objects/EntityData")]
    public class EntityData : ScriptableObject
    {
        [SerializeField] private string entityName;         
        [SerializeField] private ElementType defaultElement;      // Will never change
        public ElementType taggedElement;                         // default and tagged will be the same if default != Normal
        public float health;



        public enum ElementType
        {
            Normal,
            Zoom,
            Boom,
            Gloom
        }
    }

}