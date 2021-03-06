﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace System.Windows.Controls
{
    public interface IItemContainer
    {
        void PrepareContainerForItem(object item, DataTemplate itemTemplate, Style itemContainerStyle);
        void ClearContainerForItem(object item);
    }

    [ContentProperty("Items")]
    public class ItemsControl : Control, IGeneratorHost
    {
        private class DefaultItemsPanelTemplate : IFrameworkTemplate
        {
            public void Attach(FrameworkElement element)
            {
                element.TemplateChild = new StackPanel();
            }

            public void Detach(FrameworkElement element)
            {
                element.TemplateChild = null;
            }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(ItemsControl), new FrameworkPropertyMetadata(null, (dependencyObject, e) => (dependencyObject as ItemsControl).OnItemsSourceChanged(e)));
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemContainerStyleProperty = DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(ItemsControl), new FrameworkPropertyMetadata());
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public static readonly DependencyProperty ItemContainerStyleSelectorProperty = DependencyProperty.Register("ItemContainerStyleSelector", typeof(IStyleSelector), typeof(ItemsControl), new FrameworkPropertyMetadata());
        public IStyleSelector ItemContainerStyleSelector
        {
            get { return (IStyleSelector)GetValue(ItemContainerStyleSelectorProperty); }
            set { SetValue(ItemContainerStyleSelectorProperty, value); }
        }

        public static readonly DependencyProperty ItemsPanelProperty = DependencyProperty.Register("ItemsPanel", typeof(IFrameworkTemplate), typeof(ItemsControl), new FrameworkPropertyMetadata(new DefaultItemsPanelTemplate()));
        public IFrameworkTemplate ItemsPanel
        {
            get { return (IFrameworkTemplate)GetValue(ItemsPanelProperty); }
            set { SetValue(ItemsPanelProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ItemsControl), new FrameworkPropertyMetadata());
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(IDataTemplateSelector), typeof(ItemsControl), new FrameworkPropertyMetadata());
        public IDataTemplateSelector ItemTemplateSelector
        {
            get { return (IDataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        public ItemCollection Items { get; private set; }
        ItemCollection IGeneratorHost.View { get { return Items; } }

        private static readonly DependencyPropertyKey ItemContainerGeneratorPropertyKey = DependencyProperty.RegisterReadOnly("ItemContainerGenerator", typeof(IItemContainerGenerator), typeof(ItemsControl), new FrameworkPropertyMetadata());
        public static readonly DependencyProperty ItemContainerGeneratorProperty = ItemContainerGeneratorPropertyKey.DependencyProperty;
        public IItemContainerGenerator ItemContainerGenerator
        {
            get { return (IItemContainerGenerator)GetValue(ItemContainerGeneratorPropertyKey); }
            private set { SetValue(ItemContainerGeneratorPropertyKey, value); }
        }

        public ItemsControl()
        {
            Items = new ItemCollection();
            ItemContainerGenerator = new ItemContainerGenerator(this);
        }

        public bool IsItemItsOwnContainer(object item)
        {
            return IsItemItsOwnContainerOverride(item);
        }

        protected virtual bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FrameworkElement;
        }

        public FrameworkElement GetContainerForItem(object item)
        {
            if (IsItemItsOwnContainer(item))
            {
                return item as FrameworkElement;
            }

            return GetContainerForItemOverride();
        }

        protected virtual FrameworkElement GetContainerForItemOverride()
        {
            return new ContentPresenter();
        }

        public void PrepareContainerForItem(object item, FrameworkElement container)
        {
            PrepareContainerForItemOverride(item, container);

            OnPrepareContainerForItem(item, container);
        }

        protected virtual void PrepareContainerForItemOverride(object item, FrameworkElement container)
        {
            if (container is IItemContainer)
            {
                DataTemplate itemTemplate = ItemTemplate ?? (ItemTemplateSelector != null ? ItemTemplateSelector.SelectTemplate(item, container) : null);
                Style itemContainerStyle = ItemContainerStyle ?? (ItemContainerStyleSelector != null ? ItemContainerStyleSelector.SelectStyle(item, container) : null);

                ((IItemContainer)container).PrepareContainerForItem(item, itemTemplate, itemContainerStyle);
            }
        }

        protected virtual void OnPrepareContainerForItem(object item, FrameworkElement container)
        {
            //
        }

        public void ClearContainerForItem(object item, FrameworkElement container)
        {
            ClearContainerForItemOverride(item, container);

            OnClearContainerForItem(item, container);
        }

        protected virtual void ClearContainerForItemOverride(object item, FrameworkElement container)
        {
            if (container is IItemContainer)
            {
                ((IItemContainer)container).ClearContainerForItem(item);
            }
        }

        protected virtual void OnClearContainerForItem(object item, FrameworkElement container)
        {
            //
        }

        private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ItemsSource == null && !GetValueSource(ItemsSourceProperty).IsExpression)
            {
                Items.ClearItemsSource();
            }
            else
            {
                Items.SetItemsSource(ItemsSource);
            }
        }
    }
}
