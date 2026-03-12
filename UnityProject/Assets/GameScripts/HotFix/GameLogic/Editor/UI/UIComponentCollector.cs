using System;
using System.Collections.Generic;
using GameLogic.UI.CodeGen;
using GameLogic.UI.UXTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.Editor.UI
{
    public static class UIComponentCollector
    {
        private static readonly Type[] SupportedUGUI =
        {
            typeof(Button),
            typeof(Image),
            typeof(RawImage),
            typeof(Text),
            typeof(Toggle),
            typeof(InputField),
            typeof(Dropdown),
            typeof(Slider),
            typeof(ScrollRect),
        };

        public static List<UIComponentData> Collect(GameObject root)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            var components = root.GetComponentsInChildren<Component>(true);
            var results = new List<UIComponentData>();
            var existing = new HashSet<string>(StringComparer.Ordinal);

            foreach (var component in components)
            {
                if (component == null)
                {
                    continue;
                }

                if (!IsSupported(component))
                {
                    continue;
                }

                var fieldName = UIFieldNameGenerator.GenerateFieldName(
                    component.gameObject.name,
                    component.GetType().Name,
                    existing);

                results.Add(new UIComponentData
                {
                    ObjectPath = TransformPathUtility.GetPath(root.transform, component.transform),
                    ObjectName = component.gameObject.name,
                    ComponentType = component.GetType().FullName,
                    FieldName = fieldName,
                    IsUXComponent = component is UXComponent,
                });
            }

            return results;
        }

        private static bool IsSupported(Component component)
        {
            if (component is UXComponent)
            {
                return true;
            }

            var componentType = component.GetType();
            for (int i = 0; i < SupportedUGUI.Length; i++)
            {
                if (SupportedUGUI[i].IsAssignableFrom(componentType))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static class TransformPathUtility
    {
        public static string GetPath(Transform root, Transform target)
        {
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (target == root)
            {
                return root.name;
            }

            var stack = new Stack<string>();
            var current = target;
            while (current != null && current != root)
            {
                stack.Push(current.name);
                current = current.parent;
            }

            if (current == null)
            {
                throw new InvalidOperationException("Target is not a child of root.");
            }

            stack.Push(root.name);
            return string.Join("/", stack.ToArray());
        }
    }
}
