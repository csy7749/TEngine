using NUnit.Framework;
using GameLogic.UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace GameLogic.Tests
{
    public sealed class UIManagerTests
    {
        private sealed class TestWindow : UIWindow
        {
            public bool Created { get; private set; }
            public bool Shown { get; private set; }
            public bool Hidden { get; private set; }
            public bool Destroyed { get; private set; }

            protected override void OnCreate() => Created = true;
            protected override void OnShow() => Shown = true;
            protected override void OnHide() => Hidden = true;
            protected override void OnDestroy() => Destroyed = true;
        }

        private sealed class TestResourceProvider : IUIResourceProvider
        {
            public GameObject LoadInstance(UIResourceRequest request)
            {
                return CreateInstance(request);
            }

            public UniTask<GameObject> LoadInstanceAsync(UIResourceRequest request)
            {
                return UniTask.FromResult(CreateInstance(request));
            }

            public void ReleaseInstance(GameObject instance)
            {
                if (instance != null)
                {
                    UnityEngine.Object.DestroyImmediate(instance);
                }
            }

            private static GameObject CreateInstance(UIResourceRequest request)
            {
                var go = new GameObject("TestWindow");
                var canvas = go.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                go.AddComponent<GraphicRaycaster>();
                return go;
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (UIManager.IsInitialized)
            {
                UIManager.Shutdown();
            }
        }

        [Test]
        public void OpenAndCloseWindow_LifecycleWorks()
        {
            var options = new UIManagerOptions(new UIManagerOptionsInput
            {
                ResourceProvider = new TestResourceProvider(),
                WindowFactory = new ActivatorUIWindowFactory(),
            });

            UIManager.Initialize(options);

            var openOptions = new UIWindowOpenOptions(new UIWindowOpenOptionsInput
            {
                PrefabPath = "Test/Window",
                Layer = UILayer.UI,
                CacheMode = UIWindowCacheMode.None,
            });

            var window = UIManager.Instance.OpenWindow<TestWindow>(openOptions);
            Assert.IsTrue(window.Created);
            Assert.IsTrue(window.Shown);

            UIManager.Instance.CloseWindow<TestWindow>();
            Assert.IsTrue(window.Hidden);
            Assert.IsTrue(window.Destroyed);
        }
    }
}

