using System;
using System.Collections.Generic;
using GameLogic.UI.UXTools.Color;
using GameLogic.UI.UXTools.Localization;
using GameLogic.UI.UXTools.Reddot;
using GameLogic.UI.UXTools.Widget;

namespace GameLogic.UI.UXTools
{
    public sealed class UXToolsOptionsInput
    {
        public ILocalizationProvider LocalizationProvider { get; set; }
        public UIColorManager ColorManager { get; set; }
        public ReddotManager ReddotManager { get; set; }
        public WidgetRepository WidgetRepository { get; set; }
        public IReadOnlyList<UXComponent> InitialComponents { get; set; }
    }

    public sealed class UXToolsOptions
    {
        public UXToolsOptions(UXToolsOptionsInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            LocalizationProvider = input.LocalizationProvider ?? throw new ArgumentNullException(nameof(input.LocalizationProvider));
            ColorManager = input.ColorManager ?? throw new ArgumentNullException(nameof(input.ColorManager));
            ReddotManager = input.ReddotManager ?? throw new ArgumentNullException(nameof(input.ReddotManager));
            WidgetRepository = input.WidgetRepository ?? throw new ArgumentNullException(nameof(input.WidgetRepository));
            InitialComponents = input.InitialComponents;
        }

        public ILocalizationProvider LocalizationProvider { get; }
        public UIColorManager ColorManager { get; }
        public ReddotManager ReddotManager { get; }
        public WidgetRepository WidgetRepository { get; }
        public IReadOnlyList<UXComponent> InitialComponents { get; }
    }
}
