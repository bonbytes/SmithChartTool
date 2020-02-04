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

namespace SmithChartTool.ViewModel
{
	public interface IDragDrop
	{	
		void Drop(DragEventArgs e);
	}


	public static class DragDropHelper
	{
		public static readonly DependencyProperty CanDragDropProperty = DependencyProperty.RegisterAttached("CanDragDrop", typeof(bool), typeof(DragDropHelper), new PropertyMetadata(OnCanDragDropChanged));
		public static readonly DependencyProperty IsDragSourceProperty = DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDropHelper), new PropertyMetadata(OnIsDragSourceChanged));

		public static bool GetCanDragDrop(DependencyObject dobj)
		{
			return (bool)dobj.GetValue(CanDragDropProperty);
		}

		public static void SetCanDragDrop(DependencyObject dobj, bool value)
		{
			dobj.SetValue(CanDragDropProperty, value);
		}

		public static bool GetIsDragSource(DependencyObject dobj)
		{
			return (bool)dobj.GetValue(IsDragSourceProperty);
		}

		public static void SetIsDragSource(DependencyObject dobj, bool value)
		{
			dobj.SetValue(IsDragSourceProperty, value);
		}

		private static void OnCanDragDropChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == e.OldValue)
				return;
			UIElement uI = sender as UIElement;
			if(uI != null)
			{
				uI.Drop += OnDrop;
				uI.DragEnter += OnDragEnter;
			}
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

		private static Point startPoint;

		private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			startPoint = e.GetPosition(null);
		}
		private static void OnMouseMove(object sender, MouseEventArgs e)
		{
			Point mousePos = e.GetPosition(null);
			Vector diff = startPoint - mousePos;

			if (e.LeftButton == MouseButtonState.Pressed &&
				(Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
			{
				ListViewItem lvitem = FindControlAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);
				if (lvitem != null)
				{
					SchematicElement element = (SchematicElement)((sender as ListView).ItemContainerGenerator.ItemFromContainer(lvitem));
					DataObject dragData = new DataObject("myFormat", element);
					DragDrop.DoDragDrop(lvitem, dragData, DragDropEffects.Move);
				}
			}
		}

		private static T FindControlAnchestor<T>(DependencyObject current) where T : DependencyObject
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


		private static void OnDragEnter(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
			{
				e.Effects = DragDropEffects.None;
			}
		}

		private static void OnDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("myFormat"))
			{
				var blu = FindDragDropAnchestor(sender as DependencyObject);
				if (blu != null)
				{
					blu.Drop(e);
				}
			}
		}

		private static IDragDrop FindDragDropAnchestor(DependencyObject current)
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
