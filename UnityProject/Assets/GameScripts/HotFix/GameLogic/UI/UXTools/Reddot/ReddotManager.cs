using System;
using System.Collections.Generic;

namespace GameLogic.UI.UXTools.Reddot
{
    public sealed class ReddotManager
    {
        private const char PathSeparator = '/';

        private readonly Dictionary<string, ReddotNode> _nodes = new Dictionary<string, ReddotNode>(StringComparer.Ordinal);

        public ReddotManager()
        {
            _nodes[string.Empty] = new ReddotNode(string.Empty, null);
        }

        public void SetCount(string path, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be non-negative.");
            }

            var node = GetOrCreateNode(path);
            node.SetSelfCount(count);
        }

        public int GetCount(string path)
        {
            var node = GetNode(path);
            if (node == null)
            {
                return 0;
            }

            return node.TotalCount;
        }

        public IDisposable Subscribe(string path, Action<int> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var node = GetOrCreateNode(path);
            node.CountChanged += handler;
            handler(node.TotalCount);
            return new Subscription(() => node.CountChanged -= handler);
        }

        private ReddotNode GetNode(string path)
        {
            if (path == null)
            {
                return null;
            }

            _nodes.TryGetValue(path, out var node);
            return node;
        }

        private ReddotNode GetOrCreateNode(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (_nodes.TryGetValue(path, out var existing))
            {
                return existing;
            }

            var parts = path.Split(PathSeparator);
            var currentPath = string.Empty;
            var parent = _nodes[string.Empty];

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();
                if (string.IsNullOrEmpty(part))
                {
                    continue;
                }

                currentPath = string.IsNullOrEmpty(currentPath) ? part : currentPath + PathSeparator + part;
                if (!_nodes.TryGetValue(currentPath, out var node))
                {
                    node = new ReddotNode(currentPath, parent);
                    _nodes[currentPath] = node;
                    parent.AddChild(node);
                }

                parent = node;
            }

            return parent;
        }

        private sealed class ReddotNode
        {
            private readonly List<ReddotNode> _children = new List<ReddotNode>();
            private int _selfCount;
            private int _totalCount;

            public ReddotNode(string path, ReddotNode parent)
            {
                Path = path;
                Parent = parent;
            }

            public string Path { get; }
            public ReddotNode Parent { get; }
            public int TotalCount => _totalCount;

            public event Action<int> CountChanged;

            public void AddChild(ReddotNode child)
            {
                _children.Add(child);
            }

            public void SetSelfCount(int count)
            {
                if (_selfCount == count)
                {
                    return;
                }

                _selfCount = count;
                UpdateTotal();
            }

            private void UpdateTotal()
            {
                var total = _selfCount;
                for (int i = 0; i < _children.Count; i++)
                {
                    total += _children[i]._totalCount;
                }

                if (total == _totalCount)
                {
                    return;
                }

                _totalCount = total;
                CountChanged?.Invoke(_totalCount);
                Parent?.OnChildChanged();
            }

            private void OnChildChanged()
            {
                UpdateTotal();
            }
        }

        private sealed class Subscription : IDisposable
        {
            private Action _dispose;

            public Subscription(Action dispose)
            {
                _dispose = dispose ?? throw new ArgumentNullException(nameof(dispose));
            }

            public void Dispose()
            {
                _dispose?.Invoke();
                _dispose = null;
            }
        }
    }
}
