using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Docking;

namespace Slam_MapEditor
{
    public class CustomDockingPanesFactory : DockingPanesFactory
    {
        protected override void AddPane(RadDocking radDocking, RadPane pane)
        {
            pane.CanUserClose = false;
            var paneModel = pane.DataContext as PaneViewModel;
            if (paneModel != null && !(paneModel.IsDocument))
            {
                RadPaneGroup group = null;
                switch (paneModel.InitialPosition)
                {
                    case DockPosition.DockedRight:
                        group = radDocking.SplitItems.ToList().FirstOrDefault(i => i.Control.Name == "rightGroup") as RadPaneGroup;
                        if (group != null)
                        {
                            group.Items.Add(pane);

                        }
                        return;
                    case DockPosition.DockedRightBottom:
                        group = radDocking.SplitItems.ToList().FirstOrDefault(i => i.Control.Name == "rightBottomGroup") as RadPaneGroup;
                        if (group != null)
                        {
                            group.Items.Add(pane);
                        }
                        return;
                    case DockPosition.DockedBottom:
                        group = radDocking.SplitItems.ToList().FirstOrDefault(i => i.Control.Name == "bottomGroup") as RadPaneGroup;
                        if (group != null)
                        {
                            group.Items.Add(pane);
                        }
                        return;
                    case DockPosition.DockedLeft:
                        group = radDocking.SplitItems.ToList().FirstOrDefault(i => i.Control.Name == "leftGroup") as RadPaneGroup;
                        if (group != null)
                        {
                            group.Items.Add(pane);
                        }
                        return;
                    case DockPosition.FloatingDockable:
                        var fdSplitContainer = radDocking.GeneratedItemsFactory.CreateSplitContainer();
                        group = radDocking.GeneratedItemsFactory.CreatePaneGroup();
                        fdSplitContainer.Items.Add(group);
                        group.Items.Add(pane);
                        radDocking.Items.Add(fdSplitContainer);
                        pane.MakeFloatingDockable();
                        return;
                    case DockPosition.FloatingOnly:
                        var foSplitContainer = radDocking.GeneratedItemsFactory.CreateSplitContainer();
                        group = radDocking.GeneratedItemsFactory.CreatePaneGroup();
                        foSplitContainer.Items.Add(group);
                        group.Items.Add(pane);
                        radDocking.Items.Add(foSplitContainer);
                        pane.MakeFloatingOnly();
                        return;
                    case DockPosition.DockedTop:
                    default:
                        return;
                }
            }

            base.AddPane(radDocking, pane);
        }

        protected override RadPane CreatePaneForItem(object item)
        {
            var viewModel = item as PaneViewModel;
            if (viewModel != null)
            {
                var pane = viewModel.IsDocument ? new RadDocumentPane() : new RadPane();
                pane.DataContext = item;
                RadDocking.SetSerializationTag(pane, viewModel.Header);
                if (viewModel.ContentType != null)
                {
                    pane.Content = Activator.CreateInstance(viewModel.ContentType);
                }

                return pane;
            }

            return base.CreatePaneForItem(item);
        }

        protected override void RemovePane(RadPane pane)
        {
            pane.DataContext = null;
            pane.Content = null;
            pane.ClearValue(RadDocking.SerializationTagProperty);
            pane.RemoveFromParent();
        }
    }
}