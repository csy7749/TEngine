using System;
using UnityEngine;

namespace GameLogic.UI
{
    public sealed class UIWindowContextInput
    {
        public string WindowId { get; set; }
        public UILayer Layer { get; set; }
        public GameObject Instance { get; set; }
        public Transform LayerRoot { get; set; }
    }

    public sealed class UIWindowContext
    {
        public UIWindowContext(UIWindowContextInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (input.Instance == null)
            {
                throw new ArgumentNullException(nameof(input.Instance));
            }

            WindowId = input.WindowId;
            Layer = input.Layer;
            Instance = input.Instance;
            LayerRoot = input.LayerRoot;
        }

        public string WindowId { get; }
        public UILayer Layer { get; }
        public GameObject Instance { get; }
        public Transform LayerRoot { get; }
    }
}
