﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using D = System.Collections.Generic.Dictionary<string, object>;

namespace ObjectRPC.WPF
{
    class TextBlockFacet : Facet<TextBlock>
    {
        public TextBlockFacet(Entity entity, TextBlock obj)
            : base(entity, obj)
        {
        }

        public string Text
        {
            set
            {
                obj.Text = value;
            }
        }
    }

    class ButtonFacet : Facet<Button>
    {
        public ButtonFacet(Entity entity, Button obj)
            : base(entity, obj)
        {
            obj.Click += OnClick;
        }

        public string Label
        {
            set
            {
                obj.Content = value;
            }
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            entity.SendUpdate(new Dictionary<string, object> { {"click", true } });
        }
    }

    class TreeViewFacet : Facet<TreeView>
    {
        private IList<object> data;
        private bool isTreeViewUpdateInProgress = false;
        
        public TreeViewFacet(Entity entity, TreeView obj)
            : base(entity, obj)
        {
            obj.SelectedItemChanged += OnSelectedItemChanged;
        }

        public string SelectedId
        {
            get
            {
                var selectedTVI = (TreeViewItem)obj.SelectedItem;
                return (string)((selectedTVI != null) ? selectedTVI.Tag : null);
            }
        }

        public IList<object> Data
        {
            set
            {
                data = value;

                var items = obj.Items;

                TreeViewItem newSelectedTVI = null;

                isTreeViewUpdateInProgress = true;
                string oldSelectedId = SelectedId;
                items.Clear();
                foreach (var itemDataRaw in data)
                {
                    var itemData = (Dictionary<string, object>)itemDataRaw;
                    string id = (string)itemData["id"];
                    string text = (string)itemData["text"];

                    var tvi = new TreeViewItem();
                    tvi.Header = text;
                    tvi.Tag = id;
                    items.Add(tvi);

                    if (id == oldSelectedId)
                    {
                        newSelectedTVI = tvi;
                    }
                }

                if (oldSelectedId != null)
                    if (newSelectedTVI == null )
                    {
                        isTreeViewUpdateInProgress = false;
                        OnSelectedItemChanged(null, null); // need to reset view
                    }
                    else
                    {
                        SelectItem(newSelectedTVI);
                        isTreeViewUpdateInProgress = false;
                    }
                else
                    isTreeViewUpdateInProgress = false;
            }
        }

        private void SelectItemHelper(TreeViewItem item) // unneeded ATM, retest when we will have tree depth > 1
        {
            if (item == null)
                return;
            SelectItemHelper((TreeViewItem)item.Parent);
            if (!item.IsExpanded)
            {
                item.IsExpanded = true;
                item.UpdateLayout();
            }
        }
        private void SelectItem(TreeViewItem item) // QND solution
        {
            SelectItemHelper(item.Parent as TreeViewItem);
            item.IsSelected = true;
        }

        private void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!isTreeViewUpdateInProgress)
                entity.SendUpdate(new Dictionary<string, object> { { "selectedId", SelectedId } });
        }
    }

    public static class UIFacets
    {
        public static void Register(RootEntity rpc)
        {
            rpc.Register(typeof(TextBlock), typeof(TextBlockFacet));
            rpc.Register(typeof(Button), typeof(ButtonFacet));
            rpc.Register(typeof(TreeView), typeof(TreeViewFacet));
        }
    }
}
