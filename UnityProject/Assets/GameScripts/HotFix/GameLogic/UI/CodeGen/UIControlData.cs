using System;
using System.Collections.Generic;
using GameLogic.UI.MVVM;
using UnityEngine;

namespace GameLogic.UI.CodeGen
{
    public enum UIBindingKind
    {
        Property = 0,
        Command = 1,
    }

    [Serializable]
    public sealed class UIComponentData
    {
        public string ObjectPath;
        public string ObjectName;
        public string ComponentType;
        public string FieldName;
        public bool IsUXComponent;
    }

    [Serializable]
    public sealed class UIBindingRule
    {
        public UIBindingKind Kind;
        public string ComponentFieldName;
        public string ViewModelMemberName;
        public string TargetMemberName;
        public string SourceValueTypeName;
        public string TargetValueTypeName;
        public BindingMode BindingMode;
        public string CommandParameter;
    }

    public sealed class UIControlData : MonoBehaviour
    {
        [SerializeField] private string _namespaceName;
        [SerializeField] private string _className;
        [SerializeField] private string _viewModelClassName;
        [SerializeField] private bool _isWindow = true;
        [SerializeField] private string _prefabPath;
        [SerializeField] private List<UIComponentData> _components = new List<UIComponentData>();
        [SerializeField] private List<UIBindingRule> _bindingRules = new List<UIBindingRule>();

        public string NamespaceName => _namespaceName;
        public string ClassName => _className;
        public string ViewModelClassName => _viewModelClassName;
        public bool IsWindow => _isWindow;
        public string PrefabPath => _prefabPath;
        public IReadOnlyList<UIComponentData> Components => _components;
        public IReadOnlyList<UIBindingRule> BindingRules => _bindingRules;

        public void ApplyComponents(IReadOnlyList<UIComponentData> components)
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            _components = new List<UIComponentData>(components);
        }

        public void ApplyBindingRules(IReadOnlyList<UIBindingRule> rules)
        {
            if (rules == null)
            {
                throw new ArgumentNullException(nameof(rules));
            }

            _bindingRules = new List<UIBindingRule>(rules);
        }
    }
}
