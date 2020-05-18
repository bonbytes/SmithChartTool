using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SmithChartTool.View;
using SmithChartTool.Model;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.ComponentModel;

namespace SmithChartTool.Utility
{
	public interface IDragDrop
	{	
		void DropSchematicElement(int index, DragEventArgs e);
	}

	public static class DragDropHelper
	{
		public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDropHelper), new PropertyMetadata(OnIsDragSourceChanged));
		public static readonly DependencyProperty IsDropTargetProperty = DependencyProperty.RegisterAttached("IsDropTarget", typeof(bool), typeof(DragDropHelper), new PropertyMetadata(OnIsDropTargetChanged));

		public static bool GetIsDragSource(DependencyObject dobj)
		{
			return (bool)dobj.GetValue(IsDragSourceProperty);
		}

		public static void SetIsDragSource(DependencyObject dobj, bool value)
		{
			dobj.SetValue(IsDragSourceProperty, value);
		}

		public static bool GetIsDragTarget(DependencyObject dobj)
		{
			return (bool)dobj.GetValue(IsDropTargetProperty);
		}

		public static void SetIsDropTarget(DependencyObject dobj, bool value)
		{
			dobj.SetValue(IsDropTargetProperty, value);
		}

		private static void OnIsDragSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == e.OldValue)
				return;
			var uI = sender as UIElement;
			if (uI != null)
			{
				uI.PreviewMouseLeftButtonDown += OnMouseButtonDown;
				uI.PreviewMouseMove += OnMouseMove;
			}
		}

		private static void OnIsDropTargetChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == e.OldValue)
				return;
			var uI = sender as UIElement;
			if (uI != null)
			{
				uI.DragEnter += OnDragEnter;
				uI.Drop += OnDrop;
				//DependencyPropertyDescriptor.FromProperty(UIElement.IsMouseOverProperty, typeof(UIElement)).AddValueChanged(uI, IsMouseOverTarget);
			}

		}

		private static Point _dragStartPoint;

		private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.Source != null)
			{
				_dragStartPoint = e.GetPosition(null);
			}
		}

		private static void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && ((Math.Abs(e.GetPosition(null).X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
					(Math.Abs(e.GetPosition(null).Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
			{
				ListBox lb = sender as ListBox;
				ListBoxItem lbitem = FindControlAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
				if (lbitem == null) 
					return;
				else
				{
					var type = typeof(SchematicElementType).FromName((string)(lbitem.Content));
					DataObject dragData = new DataObject("SchematicElement", type);
					DragDrop.DoDragDrop(lbitem, dragData, DragDropEffects.Copy);
				}
			}
		}

		private static void OnDragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent("SchematicElement") || sender == e.Source)
			{
				e.Effects = DragDropEffects.Copy;
			}
		}

		private static bool IsMouseOverTarget(Visual target, Point point)
		{
			var bounds = VisualTreeHelper.GetDescendantBounds(target);
			return bounds.Contains(point);
		}

		private static void OnDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SchematicElement"))
			{
				ListBox lb = sender as ListBox;
				ListBoxItem lbitem = FindControlAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
				if (lbitem == null)
				{
					e.Effects = DragDropEffects.None;
					return;
				}
				ListBox schematic = FindControlAncestor<ListBox>(lb);
				var dropDest = FindDragDropAncestor(sender as DependencyObject);
				int dropDestIndex = -1;
				for (int i = 0; i < schematic.Items.Count; i++)
				{
					var lbi = lb.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
					if (lbi == null) continue;
					
					if (IsMouseOverTarget(lbi, e.GetPosition(lbi)))
					{
						dropDestIndex = i;
						break;
					}
				}
				if (dropDestIndex != -1)
				{
					dropDest.DropSchematicElement(dropDestIndex, e);
				}
				else
				{
					dropDest.DropSchematicElement(-1, e);
				}
			}
		}

	private static T FindControlAncestor<T>(DependencyObject current) where T : DependencyObject
	{
		do
		{
			if (current is T)
			{
				return (T)current;
			}
			current = VisualTreeHelper.GetParent(current);
		}
		while (current != null);
		return null;
	}

	private static IDragDrop FindDragDropAncestor(DependencyObject current)
		{
			do
			{
				if (current is FrameworkElement)
				{
					object a = (current as FrameworkElement).DataContext;
					if (a is IDragDrop)
					{
						return a as IDragDrop;
					}
				}
				current = VisualTreeHelper.GetParent(current);
			} while (current != null);
			return null;
		}
	}
}
