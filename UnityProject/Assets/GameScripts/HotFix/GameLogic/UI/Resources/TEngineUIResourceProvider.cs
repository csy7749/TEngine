using System;
using Cysharp.Threading.Tasks;
using TEngine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameLogic.UI
{
    public sealed class TEngineUIResourceProvider : IUIResourceProvider
    {
        private readonly IResourceModule _resourceModule;

        public TEngineUIResourceProvider(IResourceModule resourceModule)
        {
            _resourceModule = resourceModule ?? throw new ArgumentNullException(nameof(resourceModule));
        }

        public GameObject LoadInstance(UIResourceRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return _resourceModule.LoadGameObject(request.Location, request.Parent, request.PackageName);
        }

        public UniTask<GameObject> LoadInstanceAsync(UIResourceRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return _resourceModule.LoadGameObjectAsync(
                request.Location,
                request.Parent,
                request.CancellationToken,
                request.PackageName);
        }

        public void ReleaseInstance(GameObject instance)
        {
            if (instance == null)
            {
                return;
            }

            Object.Destroy(instance);
        }
    }
}
