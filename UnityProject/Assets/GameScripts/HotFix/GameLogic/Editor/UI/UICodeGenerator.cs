using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameLogic.UI.CodeGen;
using GameLogic.UI.MVVM;

namespace GameLogic.Editor.UI
{
    public enum UIGenerationTarget
    {
        Window = 0,
        Widget = 1,
    }

    public sealed class UICodeGenerationOptions
    {
        public UICodeGenerationOptions(string outputDirectory, UIGenerationTarget target, bool generateViewModel)
        {
            if (string.IsNullOrWhiteSpace(outputDirectory))
            {
                throw new ArgumentException("Output directory is required.", nameof(outputDirectory));
            }

            OutputDirectory = outputDirectory;
            Target = target;
            GenerateViewModel = generateViewModel;
        }

        public string OutputDirectory { get; }
        public UIGenerationTarget Target { get; }
        public bool GenerateViewModel { get; }
    }

    public sealed class UICodeGenerationResult
    {
        public UICodeGenerationResult(string path, string content)
        {
            Path = path;
            Content = content;
        }

        public string Path { get; }
        public string Content { get; }
    }

    public static class UICodeGenerator
    {
        private const string WindowBaseType = "GameLogic.UI.UIWindow";
        private const string WidgetBaseType = "GameLogic.UI.UIWidget";

        public static IReadOnlyList<UICodeGenerationResult> Generate(UIControlData data, UICodeGenerationOptions options)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            EnsureOutputDirectory(options.OutputDirectory);

            var results = new List<UICodeGenerationResult>();
            var isWindow = options.Target == UIGenerationTarget.Window;

            var mainCode = isWindow ? GenerateWindowCode(data) : GenerateWidgetCode(data);
            results.Add(new UICodeGenerationResult(BuildPath(options.OutputDirectory, data.ClassName), mainCode));

            if (options.GenerateViewModel)
            {
                var viewModelCode = GenerateViewModelCode(data);
                results.Add(new UICodeGenerationResult(BuildPath(options.OutputDirectory, data.ViewModelClassName), viewModelCode));
            }

            return results;
        }

        private static void EnsureOutputDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new InvalidOperationException($"Output directory does not exist: {directory}.");
            }
        }

        private static string BuildPath(string directory, string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new InvalidOperationException("Class name is required for code generation.");
            }

            return Path.Combine(directory, className + ".cs");
        }

        private static string GenerateWindowCode(UIControlData data)
        {
            ValidateData(data);

            var writer = new CodeWriter();
            writer.AppendLine("using GameLogic.UI;");
            writer.AppendLine("using GameLogic.UI.MVVM;");
            writer.AppendLine("using UnityEngine;");
            writer.AppendLine();
            writer.AppendLine($"namespace {data.NamespaceName}");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine($"public partial class {data.ClassName} : {WindowBaseType}");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine($"private {data.ViewModelClassName} _viewModel;");
            writer.AppendLine();
            AppendComponentFields(writer, data.Components);
            writer.AppendLine();
            writer.AppendLine("protected override void OnCreate()");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine("InitializeComponents();");
            writer.AppendLine("InitializeViewModel();");
            writer.AppendLine("BindViewModel();");
            writer.Unindent();
            writer.AppendLine("}");
            writer.AppendLine();
            AppendInitializeComponents(writer, data);
            writer.AppendLine();
            AppendInitializeViewModel(writer, data);
            writer.AppendLine();
            AppendBindViewModel(writer, data);
            writer.Unindent();
            writer.AppendLine("}");
            writer.Unindent();
            writer.AppendLine("}");
            return writer.ToString();
        }

        private static string GenerateWidgetCode(UIControlData data)
        {
            ValidateData(data);

            var writer = new CodeWriter();
            writer.AppendLine("using GameLogic.UI;");
            writer.AppendLine("using UnityEngine;");
            writer.AppendLine();
            writer.AppendLine($"namespace {data.NamespaceName}");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine($"public partial class {data.ClassName} : {WidgetBaseType}");
            writer.AppendLine("{");
            writer.Indent();
            AppendComponentFields(writer, data.Components);
            writer.AppendLine();
            writer.AppendLine("protected override void OnInitialize()");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine("InitializeComponents();");
            writer.Unindent();
            writer.AppendLine("}");
            writer.AppendLine();
            writer.AppendLine("public override void Refresh()");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine("base.Refresh();");
            writer.Unindent();
            writer.AppendLine("}");
            writer.AppendLine();
            AppendInitializeComponents(writer, data);
            writer.Unindent();
            writer.AppendLine("}");
            writer.Unindent();
            writer.AppendLine("}");
            return writer.ToString();
        }

        private static string GenerateViewModelCode(UIControlData data)
        {
            if (string.IsNullOrWhiteSpace(data.ViewModelClassName))
            {
                throw new InvalidOperationException("ViewModel class name is required.");
            }

            var writer = new CodeWriter();
            writer.AppendLine("using GameLogic.UI.MVVM;");
            writer.AppendLine();
            writer.AppendLine($"namespace {data.NamespaceName}");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine($"public sealed class {data.ViewModelClassName} : ViewModelBase");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine("// TODO: Add properties and commands.");
            writer.Unindent();
            writer.AppendLine("}");
            writer.Unindent();
            writer.AppendLine("}");
            return writer.ToString();
        }

        private static void ValidateData(UIControlData data)
        {
            if (string.IsNullOrWhiteSpace(data.NamespaceName))
            {
                throw new InvalidOperationException("Namespace is required for code generation.");
            }

            if (string.IsNullOrWhiteSpace(data.ClassName))
            {
                throw new InvalidOperationException("Class name is required for code generation.");
            }

            if (string.IsNullOrWhiteSpace(data.ViewModelClassName))
            {
                throw new InvalidOperationException("ViewModel class name is required for code generation.");
            }
        }

        private static void AppendComponentFields(CodeWriter writer, IReadOnlyList<UIComponentData> components)
        {
            for (int i = 0; i < components.Count; i++)
            {
                var component = components[i];
                var typeName = TypeNameHelper.GetGlobalTypeName(component.ComponentType);
                writer.AppendLine($"private {typeName} {component.FieldName};");
            }
        }

        private static void AppendInitializeComponents(CodeWriter writer, UIControlData data)
        {
            writer.AppendLine("private void InitializeComponents()");
            writer.AppendLine("{");
            writer.Indent();
            foreach (var component in data.Components)
            {
                var typeName = TypeNameHelper.GetGlobalTypeName(component.ComponentType);
                var isRoot = IsRootPath(data, component.ObjectPath);
                var path = RelativePath(data, component.ObjectPath);
                var targetExpr = isRoot ? "Transform" : $"Transform.Find(\"{path}\")";
                writer.AppendLine($"{component.FieldName} = {targetExpr}.GetComponent<{typeName}>();");
            }
            writer.Unindent();
            writer.AppendLine("}");
        }
        private static void AppendInitializeViewModel(CodeWriter writer, UIControlData data)
        {
            writer.AppendLine("private void InitializeViewModel()");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine($"_viewModel = new {data.ViewModelClassName}();");
            writer.Unindent();
            writer.AppendLine("}");
        }

        private static void AppendBindViewModel(CodeWriter writer, UIControlData data)
        {
            writer.AppendLine("private void BindViewModel()");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine("Bindings.ClearBindings();");

            var components = data.Components.ToDictionary(c => c.FieldName, c => c, StringComparer.Ordinal);
            for (int i = 0; i < data.BindingRules.Count; i++)
            {
                AppendBindingRule(writer, data.BindingRules[i], components, data.ViewModelClassName);
            }

            writer.Unindent();
            writer.AppendLine("}");
        }

        private static void AppendBindingRule(CodeWriter writer, UIBindingRule rule, Dictionary<string, UIComponentData> components, string viewModelClassName)
        {
            if (!components.TryGetValue(rule.ComponentFieldName, out var component))
            {
                throw new InvalidOperationException($"Binding component not found: {rule.ComponentFieldName}.");
            }

            if (rule.Kind == UIBindingKind.Command)
            {
                AppendCommandBinding(writer, rule, component);
                return;
            }

            AppendPropertyBinding(writer, rule, component, viewModelClassName);
        }

        private static void AppendCommandBinding(CodeWriter writer, UIBindingRule rule, UIComponentData component)
        {
            if (!component.ComponentType.IndexOf("UnityEngine.UI.Button", StringComparison.Ordinal) >= 0)
            {
                throw new InvalidOperationException("Command binding requires a Button component.");
            }

            if (string.IsNullOrWhiteSpace(rule.ViewModelMemberName))
            {
                throw new InvalidOperationException("ViewModel command name is required.");
            }

            writer.AppendLine("Bindings.BindCommand(new CommandBindingOptions(new CommandBindingOptionsInput");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine($"Source = new UnityButtonCommandSource({rule.ComponentFieldName}),");
            writer.AppendLine($"Command = _viewModel.{rule.ViewModelMemberName},");
            if (!string.IsNullOrWhiteSpace(rule.CommandParameter))
            {
                writer.AppendLine($"Parameter = {rule.CommandParameter},");
            }
            writer.Unindent();
            writer.AppendLine("}));");
        }

        private static void AppendPropertyBinding(CodeWriter writer, UIBindingRule rule, UIComponentData component, string viewModelClassName)
        {
            if (string.IsNullOrWhiteSpace(rule.ViewModelMemberName))
            {
                throw new InvalidOperationException("ViewModel property name is required.");
            }

            if (string.IsNullOrWhiteSpace(rule.TargetMemberName))
            {
                throw new InvalidOperationException("Target member name is required.");
            }

            if (string.IsNullOrWhiteSpace(rule.SourceValueTypeName) || string.IsNullOrWhiteSpace(rule.TargetValueTypeName))
            {
                throw new InvalidOperationException("Source and target type names are required.");
            }

            var targetType = TypeNameHelper.GetGlobalTypeName(component.ComponentType);
            var sourceValueType = TypeNameHelper.GetGlobalTypeName(rule.SourceValueTypeName);
            var targetValueType = TypeNameHelper.GetGlobalTypeName(rule.TargetValueTypeName);

            writer.AppendLine($"Bindings.Bind(new PropertyBindingOptions<{viewModelClassName}, {targetType}>(new PropertyBindingOptionsInput<{viewModelClassName}, {targetType}>");
            writer.AppendLine("{");
            writer.Indent();
            writer.AppendLine("Source = _viewModel,");
            writer.AppendLine($"Target = {rule.ComponentFieldName},");
            writer.AppendLine($"SourcePropertyName = \"{rule.ViewModelMemberName}\",");
            writer.AppendLine($"SourceGetter = vm => vm.{rule.ViewModelMemberName},");
            writer.AppendLine($"SourceSetter = (vm, value) => vm.{rule.ViewModelMemberName} = ({sourceValueType})value,");
            writer.AppendLine($"TargetGetter = target => target.{rule.TargetMemberName},");
            writer.AppendLine($"TargetSetter = (target, value) => target.{rule.TargetMemberName} = ({targetValueType})value,");
            writer.AppendLine($"SourceValueType = typeof({sourceValueType}),");
            writer.AppendLine($"TargetValueType = typeof({targetValueType}),");
            writer.AppendLine($"Mode = BindingMode.{rule.BindingMode},");
            writer.Unindent();
            writer.AppendLine("}));");
        }

        private static bool IsRootPath(UIControlData data, string objectPath)
        {
            return objectPath == data.gameObject.name;
        }

        private static string RelativePath(UIControlData data, string objectPath)
        {
            if (string.IsNullOrWhiteSpace(objectPath))
            {
                throw new InvalidOperationException("Object path is required.");
            }

            if (objectPath == data.gameObject.name)
            {
                return data.gameObject.name;
            }

            var prefix = data.gameObject.name + "/";
            return objectPath.StartsWith(prefix, StringComparison.Ordinal) ? objectPath.Substring(prefix.Length) : objectPath;
        }
    }

    internal sealed class CodeWriter
    {
        private const int IndentSize = 4;
        private readonly StringBuilder _builder = new StringBuilder();
        private int _indent;

        public void Indent() => _indent++;
        public void Unindent() => _indent = Math.Max(0, _indent - 1);

        public void AppendLine(string line = "")
        {
            if (line.Length == 0)
            {
                _builder.AppendLine();
                return;
            }

            _builder.Append(' ', _indent * IndentSize);
            _builder.AppendLine(line);
        }

        public override string ToString() => _builder.ToString();
    }

    internal static class TypeNameHelper
    {
        public static string GetGlobalTypeName(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new InvalidOperationException("Type name is required.");
            }

            return typeName.StartsWith("global::", StringComparison.Ordinal) ? typeName : "global::" + typeName;
        }
    }
}




