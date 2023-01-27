using System.ComponentModel;
using System.Linq;
using AvalonDock.Layout;
using RobotEditor.Enums;
using RobotEditor.ViewModel; 

namespace RobotEditor.Classes
{
    [Localizable(false)]
    public sealed class LayoutInitializer : ILayoutUpdateStrategy
    {
        public LayoutInitializer()
        {
            Strategy = this;
        }

        public static ILayoutUpdateStrategy Strategy { get; set; }

        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow,
            ILayoutContainer destinationContainer)
        {
            var destPane = destinationContainer as LayoutAnchorablePane;

            if (destinationContainer != null &&
                destinationContainer.FindParent<LayoutFloatingWindow>() != null)
                return false;


            var content = anchorableToShow.Content;
            bool result;
            if (destinationContainer != null && destinationContainer.FindParent<LayoutFloatingWindow>() != null)
            {
                result = false;
            }
            else
            {
                var layoutAnchorablePane =
                    layout.Descendents()
                        .OfType<LayoutAnchorablePane>()
                        .FirstOrDefault(d => d.Name == "BottomPane");
                var layoutAnchorablePane2 =
                    layout.Descendents()
                        .OfType<LayoutAnchorablePane>()
                        .FirstOrDefault(d => d.Name == "LeftPane");
                var layoutAnchorablePane3 =
                    layout.Descendents()
                        .OfType<LayoutAnchorablePane>()
                        .FirstOrDefault(d => d.Name == "RightPane");
                switch (((ToolViewModel) content).DefaultPane)
                {
                    case DefaultToolPane.Left:
                        if (layoutAnchorablePane2 != null)
                        {
                            layoutAnchorablePane2.Children.Add(anchorableToShow);
                            result = true;
                            return result;
                        }
                        break;
                    case DefaultToolPane.Right:
                        if (layoutAnchorablePane3 != null)
                        {
                            layoutAnchorablePane3.Children.Add(anchorableToShow);
                            result = true;
                            return result;
                        }
                        break;
                    case DefaultToolPane.Bottom:
                        if (layoutAnchorablePane != null)
                        {
                            layoutAnchorablePane.Children.Add(anchorableToShow);
                            result = true;
                            return result;
                        }
                        break;
                }
                var layoutAnchorablePane4 =
                    layout.Descendents()
                        .OfType<LayoutAnchorablePane>()
                        .FirstOrDefault((LayoutAnchorablePane d) => d.Name == "ToolsPane");
                if (layoutAnchorablePane4 != null)
                {
                    layoutAnchorablePane4.Children.Add(anchorableToShow);
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
            //   var toolViewModel = anchorableShown.Content as ToolViewModel;
            //   if (toolViewModel != null)
            //   {
            //       switch (toolViewModel.DefaultPane)
            //       {
            //           case DefaultToolPane.Left:
            //           case DefaultToolPane.Right:
            //               anchorableShown.AutoHideMinWidth = (double)toolViewModel.Width;
            //               anchorableShown.AutoHideWidth = (double)toolViewModel.Width;
            //               break;
            //           case DefaultToolPane.Bottom:
            //               anchorableShown.AutoHideMinHeight = (double)toolViewModel.Height;
            //               anchorableShown.AutoHideHeight = 100.0;
            //               break;
            //       }
            //   }
        }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow,
            ILayoutContainer destinationContainer) => false;

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
        }
    }
}