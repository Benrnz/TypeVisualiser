﻿namespace TypeVisualiser.UI.Views
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    [TemplatePart(Name = "PART_ItemsHolder", Type = typeof(Panel))]
    public class CachedTabControl : TabControl
    {
        private Panel itemsHolder;

        public CachedTabControl()
        {
            // this is necessary so that we get the initial databound selected item
            ItemContainerGenerator.StatusChanged += ItemContainerGeneratorStatusChanged;
        }

        /// <summary>
        /// get the ItemsHolder and generate any children
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.itemsHolder = GetTemplateChild("PART_ItemsHolder") as Panel;
            UpdateSelectedItem();
        }

        /// <summary>
        /// when the items change we remove any generated panel children and add any new ones as necessary
        /// </summary>
        /// <param name="e"></param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e", string.Format(CultureInfo.InvariantCulture, Properties.Resources.General_Given_Parameter_Cannot_Be_Null, "e"));
            }

            base.OnItemsChanged(e);

            if (this.itemsHolder == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    this.itemsHolder.Children.Clear();
                    break;

                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        foreach (object item in e.OldItems)
                        {
                            ContentPresenter cp = FindChildContentPresenter(item);
                            if (cp != null)
                            {
                                this.itemsHolder.Children.Remove(cp);
                            }
                        }
                    }

                    // don't do anything with new items because we don't want to
                    // create visuals that aren't being shown

                    UpdateSelectedItem();
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotSupportedException("Replace not supported yet");
            }
        }

        /// <summary>
        /// update the visible child in the ItemsHolder
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            UpdateSelectedItem();
        }

        /// <summary>
        /// copied from TabControl; wish it were protected in that class instead of private
        /// </summary>
        /// <returns></returns>
        protected TabItem FindSelectedTabItem()
        {
            object selectedItem = SelectedItem;
            if (selectedItem == null)
            {
                return null;
            }
            var item = selectedItem as TabItem;
            if (item == null)
            {
                item = ItemContainerGenerator.ContainerFromIndex(SelectedIndex) as TabItem;
            }
            return item;
        }

        /// <summary>
        /// create the child ContentPresenter for the given item (could be data or a TabItem)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private void CreateChildContentPresenter(object item)
        {
            if (item == null)
            {
                return;
            }

            ContentPresenter cp = FindChildContentPresenter(item);

            if (cp != null)
            {
                return;
            }

            // the actual child to be added.  cp.Tag is a reference to the TabItem
            cp = new ContentPresenter();
            var tabItem = item as TabItem;
            cp.Content = tabItem != null ? tabItem.Content : item;
            cp.ContentTemplate = SelectedContentTemplate;
            cp.ContentTemplateSelector = SelectedContentTemplateSelector;
            cp.ContentStringFormat = SelectedContentStringFormat;
            cp.Visibility = Visibility.Collapsed;
            cp.Tag = tabItem ?? (ItemContainerGenerator.ContainerFromItem(item));
            this.itemsHolder.Children.Add(cp);
        }

        /// <summary>
        /// Find the CP for the given object.  data could be a TabItem or a piece of data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ContentPresenter FindChildContentPresenter(object data)
        {
            var tabItem = data as TabItem;
            if (tabItem != null)
            {
                data = (data as TabItem).Content;
            }

            if (data == null)
            {
                return null;
            }

            if (this.itemsHolder == null)
            {
                return null;
            }

            return this.itemsHolder.Children.Cast<ContentPresenter>().FirstOrDefault(cp => cp.Content == data);
        }

        /// <summary>
        /// if containers are done, generate the selected item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemContainerGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                ItemContainerGenerator.StatusChanged -= ItemContainerGeneratorStatusChanged;
                UpdateSelectedItem();
            }
        }

        /// <summary>
        /// generate a ContentPresenter for the selected item
        /// </summary>
        private void UpdateSelectedItem()
        {
            if (this.itemsHolder == null)
            {
                return;
            }

            // generate a ContentPresenter if necessary
            TabItem item = FindSelectedTabItem();
            if (item != null)
            {
                CreateChildContentPresenter(item);
            }

            // show the right child
            foreach (ContentPresenter child in this.itemsHolder.Children)
            {
                var tabItem = child.Tag as TabItem;
                child.Visibility = tabItem != null && (tabItem.IsSelected) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}