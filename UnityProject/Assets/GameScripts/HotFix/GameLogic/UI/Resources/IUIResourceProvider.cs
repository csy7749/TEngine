using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameLogic.UI
{
    public sealed class UIResourceRequestInput
    {
        public string Location { get; set; }
        public Transform Parent { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public string PackageName { get; set; }
    }

    public sealed class UIResourceRequest
    {
        public UIResourceRequest(UIResourceRequestInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (string.IsNullOrWhiteSpace(input.Location))
            {
                throw new ArgumentException("Location is required.", nameof(input.Location));
            }

            Location = input.Location;
            Parent = input.Parent;
            CancellationToken = input.CancellationToken;
            PackageName = input.PackageName ?? string.Empty;
        }

        public string Location { get; }
        public Transform Parent { get; }
        public CancellationToken CancellationToken { get; }
        public string PackageName { get; }
    }

    public interface IUIResourceProvider
    {
        GameObject LoadInstance(UIResourceRequest request);
        UniTask<GameObject> LoadInstanceAsync(UIResourceRequest request);
        void ReleaseInstance(GameObject instance);
    }
}
