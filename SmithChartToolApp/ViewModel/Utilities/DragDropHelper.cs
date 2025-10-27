using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SmithChartToolApp.View;
using SmithChartToolLibrary;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Documents;
using System.Reflection;

namespace SmithChartToolApp.ViewModel.Utilities
{
    public interface IDragDrop
    {
        void DropSchematicElement(int index, DragEventArgs e);
    }

    public static class DragDropHelper
    {
        public static readonly DependencyProperty IsDragSourceProperty = 
            DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDropHelper), new PropertyMetadata(false, OnIsDragSourceChanged));
        public static readonly DependencyProperty IsDropTargetProperty = 
            DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDropHelper), new PropertyMetadata(false, OnIsDropTargetChanged));

        // keep one adorner per UIElement
        private static readonly Dictionary<UIElement, DropInsertionAdorner> _adorners = new Dictionary<UIElement, DropInsertionAdorner>();

        public static void SetIsDragSource(DependencyObject dobj, bool value)
        {
            dobj.SetValue(IsDragSourceProperty, value);
        }
        public static bool GetIsDragSource(DependencyObject dobj)
        {
            return (bool)dobj.GetValue(IsDragSourceProperty);
        }


        public static void SetIsDropTarget(DependencyObject dobj, bool value)
        {
            dobj.SetValue(IsDropTargetProperty, value);
        }
        public static bool GetIsDragTarget(DependencyObject dobj)
        {
            return (bool)dobj.GetValue(IsDropTargetProperty);
        }



        private static void OnIsDragSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ListBox lb)
            {
                if ((bool)e.NewValue)
                {
                    lb.PreviewMouseLeftButtonDown += OnMouseButtonDown;
                    lb.PreviewMouseMove += OnMouseMove;
                }
                else
                {
                    lb.PreviewMouseLeftButtonDown -= OnMouseButtonDown;
                    lb.PreviewMouseMove -= OnMouseMove;
                }
            }
        }

        private static void OnIsDropTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ListBox lb)
            {
                if ((bool)e.NewValue)
                {
                    lb.DragEnter += OnDragEnter;
                    lb.DragOver += OnDragOver;
                    lb.DragLeave += OnDragLeave;
                    lb.Drop += OnDrop;
                }
                else
                {
                    lb.DragEnter -= OnDragEnter;
                    lb.DragOver -= OnDragOver;
                    lb.DragLeave -= OnDragLeave;
                    lb.Drop -= OnDrop;
                }
            }
        }

        private static Point _dragStartPoint;
        private static object _draggedData;


        private static object GetItemUnderMouse(ListBox lb, Point position)
        {
            var element = lb.InputHitTest(position) as DependencyObject;
            while (element != null && element != lb)
            {
                if (element is ListBoxItem lbi)
                    return lbi.DataContext;
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBox lb)
            {
                _dragStartPoint = e.GetPosition(null);
                var item = GetItemUnderMouse(lb, e.GetPosition(lb));
                _draggedData = item;
            }
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is ListBox lb && e.LeftButton == MouseButtonState.Pressed)
            {
                Vector diff = e.GetPosition(null) - _dragStartPoint;
                if (_draggedData != null &&
                    (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                     Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
                {
                    // Convert dragged payload into a SchematicElementType when possible.
                    if (TryGetSchematicElementType(_draggedData, out var elementType))
                    {
                        var data = new DataObject("SchematicElement", elementType);
                        DragDrop.DoDragDrop(lb, data, DragDropEffects.Copy);
                    }
                    else
                    {
                        // Fallback: still provide the raw data under same key (consumer must handle)
                        var data = new DataObject("SchematicElement", _draggedData);
                        DragDrop.DoDragDrop(lb, data, DragDropEffects.Copy);
                    }
                }
            }
        }

        // --- Drop target logic + adorner ---
        private static ListBox _currentListBox;
        private static int _insertionIndex = -1;

        // Guard to avoid re-entrant/multiple OnDrop handling (common cause of duplicate inserts)
        private static bool _dropInProgress = false;


        private static void OnDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
            var ui = sender as UIElement;
            if (ui is ListBox lb)
            {
                EnsureAdorner(lb);
            }
        }

        private static void OnDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;

            var lb = sender as ListBox;
            if (lb == null)
                return;

            int index = GetDropIndex(lb, e);
            var adorner = EnsureAdorner(lb);

            double insertionX = 0;
            int itemCount = lb.Items.Count;
            if (index >= 0 && index < itemCount)
            {
                var target = lb.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                if (target != null)
                {
                    var pt = target.TranslatePoint(new Point(0, 0), lb);
                    insertionX = pt.X;
                }
            }
            else if (itemCount > 0)
            {
                var last = lb.ItemContainerGenerator.ContainerFromIndex(itemCount - 1) as ListBoxItem;
                if (last != null)
                {
                    var pt = last.TranslatePoint(new Point(last.ActualWidth, 0), lb);
                    insertionX = pt.X;
                }
                else
                {
                    insertionX = lb.ActualWidth - 2;
                }
            }
            else
            {
                insertionX = 2;
            }

            // show/update adorner (clip to listbox width)
            insertionX = Math.Max(0, Math.Min(insertionX, lb.ActualWidth));
            adorner.InsertionIndex = index;
            adorner.Show(insertionX);

            e.Handled = true;
            // cache insertion for use in Drop handler
            _currentListBox = lb;
            _insertionIndex = index >= 0 ? index : lb.Items.Count;
        }

        private static void OnDragLeave(object sender, DragEventArgs e)
        {
            var lb = sender as ListBox;
            if (lb == null)
                return;

            RemoveAdorner(lb);
            e.Handled = true;
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            // Prevent duplicate handling if OnDrop gets invoked more than once for a single drag op
            if (_dropInProgress)
            {
                e.Handled = true;
                return;
            }

            _dropInProgress = true;
            try
            {
                var lb = sender as ListBox;
                if (lb == null)
                    return;
                RemoveAdorner(lb);

                if (!(sender is ListBox target)) return;

                // Expecting SchematicElement payload (SchematicElementType), as MainViewModel.DropSchematicElement reads it.
                if (!e.Data.GetDataPresent("SchematicElement"))
                    return;

                // compute insertion index for the DropSchematicElement call
                int insertIndex = -1;
                if (_currentListBox == target && _insertionIndex >= 0)
                    insertIndex = _insertionIndex;
                else
                    insertIndex = GetInsertionIndex(target, e.GetPosition(target));

                var dropHandler = FindDragDropAncestor(target);
                if (dropHandler != null)
                {
                    dropHandler.DropSchematicElement(insertIndex, e);
                    e.Effects = DragDropEffects.Copy;
                    e.Handled = true;
                }
            }
            finally
            {
                _insertionIndex = -1;
                _currentListBox = null;
                _dropInProgress = false;
            }
        }


        private static int GetInsertionIndex(ListBox lb, Point point)
        {
            int index = lb.Items.Count;
            for (int i = 0; i < lb.Items.Count; i++)
            {
                var container = lb.ItemContainerGenerator.ContainerFromIndex(i) as FrameworkElement;
                if (container == null) continue;
                var bounds = VisualTreeHelper.GetDescendantBounds(container);
                var topLeft = container.TransformToAncestor(lb).Transform(new Point(0, 0));
                var rect = new Rect(topLeft, bounds.Size);

                // if orientation is horizontal (common for schematics) compare X; otherwise compare Y
                bool horizontal = rect.Width > rect.Height;
                if (horizontal)
                {
                    double middleX = rect.Left + rect.Width / 2;
                    if (point.X < middleX)
                    {
                        index = i;
                        return index;
                    }
                }
                else
                {
                    double middleY = rect.Top + rect.Height / 2;
                    if (point.Y < middleY)
                    {
                        index = i;
                        return index;
                    }
                }
            }
            return index;
        }


        // Traverses visual tree upward looking for a DataContext that implements IDragDrop
        private static IDragDrop FindDragDropAncestor(DependencyObject current)
        {
            do
            {
                if (current is FrameworkElement fe)
                {
                    if (fe.DataContext is IDragDrop dd)
                    {
                        return dd;
                    }
                }
                current = VisualTreeHelper.GetParent(current);
            } while (current != null);
            return null;
        }

        private static DropInsertionAdorner EnsureAdorner(UIElement element)
        {
            if (_adorners.TryGetValue(element, out var existing))
                return existing;

            var layer = AdornerLayer.GetAdornerLayer(element);
            if (layer == null)
                return null;

            var adorner = new DropInsertionAdorner(element);
            layer.Add(adorner);
            _adorners[element] = adorner;
            return adorner;
        }

        private static void RemoveAdorner(UIElement element)
        {
            if (!_adorners.TryGetValue(element, out var adorner))
                return;

            var layer = AdornerLayer.GetAdornerLayer(element);
            if (layer != null)
            {
                layer.Remove(adorner);
            }
            _adorners.Remove(element);
        }

        private static int GetDropIndex(ListBox lb, DragEventArgs e)
        {
            Point pos = e.GetPosition(lb);
            for (int i = 0; i < lb.Items.Count; i++)
            {
                var item = lb.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (item == null) continue;
                var bounds = VisualTreeHelper.GetDescendantBounds(item);
                var topLeft = item.TranslatePoint(new Point(0, 0), lb);
                var rect = new Rect(topLeft, new Size(bounds.Width, bounds.Height));
                if (rect.Contains(pos))
                {
                    return i;
                }
            }
            return -1;
        }

        // Try to map arbitrary payload into SchematicElementType (enum), using several heuristics:
        // - if payload is already SchematicElementType => success
        // - if payload is SchematicElement => use its Type
        // - if payload is string => try Enum.TryParse, then compare to SchematicElementInfo.Name and Icon
        private static bool TryGetSchematicElementType(object payload, out SchematicElementType elementType)
        {
            elementType = default;
            if (payload == null) return false;

            if (payload is SchematicElementType et)
            {
                elementType = et;
                return true;
            }

            if (payload is SchematicElement se)
            {
                elementType = se.Type;
                return true;
            }

            if (payload is string s)
            {
                // try parse enum name first
                if (Enum.TryParse<SchematicElementType>(s, true, out var parsed))
                {
                    elementType = parsed;
                    return true;
                }

                // try to match SchematicElementInfo attributes (Name or Icon)
                var enumType = typeof(SchematicElementType);
                foreach (var val in Enum.GetValues(enumType))
                {
                    var mem = enumType.GetMember(val.ToString());
                    if (mem.Length == 0) continue;
                    var attrs = mem[0].GetCustomAttributes(typeof(SchematicElementInfo), false);
                    if (attrs.Length > 0)
                    {
                        var info = attrs[0] as SchematicElementInfo;
                        if (info != null)
                        {
                            if (string.Equals(info.Name, s, StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(info.Icon, s, StringComparison.OrdinalIgnoreCase))
                            {
                                elementType = (SchematicElementType)val;
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
