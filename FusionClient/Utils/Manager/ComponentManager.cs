using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FusionClient.Utils.ComponentManager
{
    public static class ComponentManager
    {
        public static T GetOrAddComponent<T>(this GameObject target) where T : Component
        {
            if (target.GetComponent<T>() == null)
            {
                return target.AddComponent<T>();
            }
            return target.GetComponent<T>();
        }
        public static T AddComponentIfNull<T>(this GameObject target) where T : Component
        {
            if (target.GetComponent<T>() == null)
            {
                return target.AddComponent<T>();
            }
            return null;
        }
        public static void RemoveComponentIfFound<T>(this GameObject target) where T : Component
        {
            if (target.GetComponent<T>() != null)
            {
                UnityEngine.Object.Destroy(target.GetComponent<T>());
            }
        }
        public static List<T> FindAllComponentsInGameObject<T>(GameObject gameObject, bool includeInactive = true, bool searchParent = true, bool searchChildren = true) where T : class
        {
            List<T> components = new();
            foreach (T component in gameObject.GetComponents<T>())
            {
                components.Add(component);
            }
            if (searchParent == true)
            {
                foreach (T component in gameObject.GetComponentsInParent<T>(includeInactive))
                {
                    components.Add(component);
                }
            }
            if (searchChildren == true)
            {
                foreach (T component in gameObject.GetComponentsInChildren<T>(includeInactive))
                {
                    components.Add(component);
                }
            }
            return components;
        }
    }
}
