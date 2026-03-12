using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.UI
{
    public sealed class UIManager
    {
        private const int WindowSortingStep = 10;
        private const int RootSortingOrder = 0;

        private static UIManager _instance;

        private readonly UIManagerOptions _options;
        private readonly Dictionary<Type, UIWindowEntry> _windowCache = new Dictionary<Type, UIWindowEntry>();
        private readonly Dictionary<UILayer, List<UIWindowEntry>> _layerWindows = new Dictionary<UILayer, List<UIWindowEntry>>();
        private readonly Dictionary<UILayer, UILayerContext> _layers = new Dictionary<UILayer, UILayerContext>();

        private Transform _root;
        private Camera _uiCamera;
        private int _uiLayerId;

        private UIManager(UIManagerOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public static UIManager Instance => _instance ?? throw new InvalidOperationException("UIManager is not initialized.");
        public static bool IsInitialized => _instance != null;

        public static UIManager Initialize(UIManagerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (_instance != null)
            {
                throw new InvalidOperationException("UIManager is already initialized.");
            }

            var manager = new UIManager(options);
            manager.BuildRoot();
            _instance = manager;
            return manager;
        }

        public static void Shutdown()
        {
            if (_instance == null)
            {
                return;
            }

            _instance.Dispose();
            _instance = null;
        }

        public async UniTask<T> OpenWindowAsync<T>(UIWindowOpenOptions options) where T : UIWindow
        {
            var window = await OpenWindowAsync(typeof(T), options);
            return (T)window;
        }

        public T OpenWindow<T>(UIWindowOpenOptions options) where T : UIWindow
        {
            return (T)OpenWindow(typeof(T), options);
        }

        public void CloseWindow<T>() where T : UIWindow
        {
            CloseWindow(typeof(T));
        }

        public void CloseWindow(Type windowType)
        {
            if (windowType == null)
            {
                throw new ArgumentNullException(nameof(windowType));
            }

            if (!_windowCache.TryGetValue(windowType, out var entry))
            {
                return;
            }

            HideWindowEntry(entry);
            RemoveFromLayer(entry);

            if (entry.Options.CacheMode == UIWindowCacheMode.Cache)
            {
                return;
            }

            DestroyWindowEntry(entry);
            _windowCache.Remove(windowType);
        }

        private void Dispose()
        {
            foreach (var entry in _windowCache.Values)
            {
                DestroyWindowEntry(entry);
            }

            _windowCache.Clear();
            _layerWindows.Clear();
            _layers.Clear();

            if (_root != null)
            {
                UnityEngine.Object.Destroy(_root.gameObject);
                _root = null;
            }

            if (_uiCamera != null)
            {
                UnityEngine.Object.Destroy(_uiCamera.gameObject);
                _uiCamera = null;
            }
        }

        private async UniTask<UIWindow> OpenWindowAsync(Type windowType, UIWindowOpenOptions options)
        {
            var entry = GetOrCreateCachedEntry(windowType, options);
            if (entry != null)
            {
                BringToFront(entry);
                ShowWindowEntry(entry);
                return entry.Window;
            }

            var created = await CreateWindowEntryAsync(windowType, options);
            AddWindowEntry(created);
            ShowWindowEntry(created);
            return created.Window;
        }

        private UIWindow OpenWindow(Type windowType, UIWindowOpenOptions options)
        {
            var entry = GetOrCreateCachedEntry(windowType, options);
            if (entry != null)
            {
                BringToFront(entry);
                ShowWindowEntry(entry);
                return entry.Window;
            }

            var created = CreateWindowEntry(windowType, options);
            AddWindowEntry(created);
            ShowWindowEntry(created);
            return created.Window;
        }

        private UIWindowEntry GetOrCreateCachedEntry(Type windowType, UIWindowOpenOptions options)
        {
            ValidateOpenOptions(windowType, options);

            if (!_windowCache.TryGetValue(windowType, out var cached))
            {
                return null;
            }

            if (cached.Options.CacheMode != UIWindowCacheMode.Cache)
            {
                return null;
            }

            return cached;
        }

        private void ValidateOpenOptions(Type windowType, UIWindowOpenOptions options)
        {
            if (windowType == null)
            {
                throw new ArgumentNullException(nameof(windowType));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (!_layers.ContainsKey(options.Layer))
            {
                throw new InvalidOperationException($"Layer {options.Layer} is not configured.");
            }
        }

        private async UniTask<UIWindowEntry> CreateWindowEntryAsync(Type windowType, UIWindowOpenOptions options)
        {
            var window = _options.WindowFactory.Create(windowType);
            var instance = await _options.ResourceProvider.LoadInstanceAsync(CreateRequest(options));
            return BuildWindowEntry(window, instance, options);
        }

        private UIWindowEntry CreateWindowEntry(Type windowType, UIWindowOpenOptions options)
        {
            var window = _options.WindowFactory.Create(windowType);
            var instance = _options.ResourceProvider.LoadInstance(CreateRequest(options));
            return BuildWindowEntry(window, instance, options);
        }

        private UIResourceRequest CreateRequest(UIWindowOpenOptions options)
        {
            return new UIResourceRequest(new UIResourceRequestInput
            {
                Location = options.PrefabPath,
                Parent = _layers[options.Layer].Root,
                PackageName = options.PackageName,
            });
        }

        private UIWindowEntry BuildWindowEntry(UIWindow window, GameObject instance, UIWindowOpenOptions options)
        {
            if (window == null)
            {
                throw new InvalidOperationException("Window factory returned null.");
            }

            if (instance == null)
            {
                throw new InvalidOperationException("Resource provider returned null instance.");
            }

            SetLayerRecursively(instance, _uiLayerId);
            instance.transform.SetParent(_layers[options.Layer].Root, false);

            var context = new UIWindowContext(new UIWindowContextInput
            {
                WindowId = options.WindowId,
                Layer = options.Layer,
                Instance = instance,
                LayerRoot = _layers[options.Layer].Root,
            });

            window.Initialize(context);
            return new UIWindowEntry(window, instance, options);
        }
        private void AddWindowEntry(UIWindowEntry entry)
        {
            _windowCache[entry.WindowType] = entry;
            AddToLayer(entry);
        }

        private void AddToLayer(UIWindowEntry entry)
        {
            var layerList = GetLayerList(entry.Layer);
            layerList.Add(entry);
            UpdateLayerSorting(entry.Layer);
        }

        private void BringToFront(UIWindowEntry entry)
        {
            var layerList = GetLayerList(entry.Layer);
            if (layerList.Remove(entry))
            {
                layerList.Add(entry);
                UpdateLayerSorting(entry.Layer);
                return;
            }

            layerList.Add(entry);
            UpdateLayerSorting(entry.Layer);
        }

        private void RemoveFromLayer(UIWindowEntry entry)
        {
            var layerList = GetLayerList(entry.Layer);
            layerList.Remove(entry);
            UpdateLayerSorting(entry.Layer);
        }

        private List<UIWindowEntry> GetLayerList(UILayer layer)
        {
            if (_layerWindows.TryGetValue(layer, out var list))
            {
                return list;
            }

            list = new List<UIWindowEntry>();
            _layerWindows[layer] = list;
            return list;
        }

        private void ShowWindowEntry(UIWindowEntry entry)
        {
            entry.Instance.SetActive(true);
            entry.Window.ShowInternal();
        }

        private void HideWindowEntry(UIWindowEntry entry)
        {
            entry.Window.HideInternal();
            entry.Instance.SetActive(false);
        }

        private void DestroyWindowEntry(UIWindowEntry entry)
        {
            entry.Window.DestroyInternal();
            _options.ResourceProvider.ReleaseInstance(entry.Instance);
        }

        private void UpdateLayerSorting(UILayer layer)
        {
            var layerList = GetLayerList(layer);
            var baseOrder = _layers[layer].Definition.SortingOrder;

            for (int i = 0; i < layerList.Count; i++)
            {
                var entry = layerList[i];
                var sortingOrder = baseOrder + (i * WindowSortingStep) + entry.Options.SortingOrderOffset;
                ApplySortingOrder(entry.Window, sortingOrder);
            }
        }

        private void ApplySortingOrder(UIWindow window, int sortingOrder)
        {
            var canvas = window.Canvas;
            if (canvas == null)
            {
                return;
            }

            canvas.overrideSorting = true;
            canvas.sortingOrder = sortingOrder;
        }

        private void BuildRoot()
        {
            _uiLayerId = ResolveLayerId(_options.LayerName);
            _root = CreateRootGameObject(_options.RootName, _options.RootSettings, _uiLayerId);
            _uiCamera = CreateCamera(_options.CameraSettings, _uiLayerId, _root);
            ConfigureRootCanvas(_root, _uiCamera, _options.RootSettings);
            CreateLayerHierarchy(_root, _options.LayerSettings, _uiLayerId);
        }

        private static int ResolveLayerId(string layerName)
        {
            var layerId = LayerMask.NameToLayer(layerName);
            if (layerId < 0)
            {
                throw new InvalidOperationException($"Unity layer {layerName} not found.");
            }

            return layerId;
        }

        private Transform CreateRootGameObject(string name, UIRootSettings settings, int layerId)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var root = new GameObject(name);
            root.layer = layerId;
            UnityEngine.Object.DontDestroyOnLoad(root);
            return root.transform;
        }

        private Camera CreateCamera(UICameraSettings settings, int layerId, Transform parent)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            var cameraObject = new GameObject(settings.Name);
            cameraObject.layer = layerId;
            cameraObject.transform.SetParent(parent, false);

            var camera = cameraObject.AddComponent<Camera>();
            camera.clearFlags = settings.ClearFlags;
            camera.backgroundColor = settings.BackgroundColor;
            camera.orthographic = true;
            camera.orthographicSize = settings.OrthographicSize;
            camera.nearClipPlane = settings.NearClipPlane;
            camera.farClipPlane = settings.FarClipPlane;
            camera.depth = settings.Depth;
            camera.cullingMask = 1 << layerId;
            return camera;
        }

        private void ConfigureRootCanvas(Transform root, Camera camera, UIRootSettings settings)
        {
            var canvas = root.gameObject.AddComponent<Canvas>();
            canvas.renderMode = settings.RenderMode;
            canvas.worldCamera = camera;
            canvas.sortingOrder = RootSortingOrder;

            var scaler = root.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = settings.ReferenceResolution;
            scaler.matchWidthOrHeight = settings.MatchWidthOrHeight;

            root.gameObject.AddComponent<GraphicRaycaster>();
        }

        private void CreateLayerHierarchy(Transform root, UILayerSettings settings, int layerId)
        {
            foreach (var definition in settings.Layers)
            {
                var layerRoot = new GameObject(definition.Name);
                layerRoot.layer = layerId;
                layerRoot.transform.SetParent(root, false);

                var canvas = layerRoot.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = definition.SortingOrder;

                layerRoot.AddComponent<GraphicRaycaster>();

                _layers[definition.Layer] = new UILayerContext(definition, layerRoot.transform);
            }
        }

        private static void SetLayerRecursively(GameObject root, int layerId)
        {
            if (root == null)
            {
                return;
            }

            var stack = new Stack<Transform>();
            stack.Push(root.transform);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                current.gameObject.layer = layerId;

                for (int i = 0; i < current.childCount; i++)
                {
                    stack.Push(current.GetChild(i));
                }
            }
        }

        private sealed class UILayerContext
        {
            public UILayerContext(UILayerDefinition definition, Transform root)
            {
                Definition = definition ?? throw new ArgumentNullException(nameof(definition));
                Root = root ?? throw new ArgumentNullException(nameof(root));
            }

            public UILayerDefinition Definition { get; }
            public Transform Root { get; }
        }

        private sealed class UIWindowEntry
        {
            public UIWindowEntry(UIWindow window, GameObject instance, UIWindowOpenOptions options)
            {
                Window = window ?? throw new ArgumentNullException(nameof(window));
                Instance = instance ?? throw new ArgumentNullException(nameof(instance));
                Options = options ?? throw new ArgumentNullException(nameof(options));
                WindowType = window.GetType();
                Layer = options.Layer;
            }

            public UIWindow Window { get; }
            public GameObject Instance { get; }
            public UIWindowOpenOptions Options { get; }
            public Type WindowType { get; }
            public UILayer Layer { get; }
        }
    }
}







