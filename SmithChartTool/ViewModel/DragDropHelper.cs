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
				uI.PreviewMouseLeftButtonUp += OnMouseButtonUp;
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
			}
		}

		private static Point _dragStartPoint;
		private static UIElement _realDragSource;
		private static UIElement _dummyDragSource = new UIElement();
		private static bool _isDown = false;
		private static bool _isDragging = false;

		private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.Source == null)
			{ }
			else
			{
				_isDown = true;
				_dragStartPoint = e.GetPosition(null);
			}
		}
		private static void OnMouseButtonUp(object sender, MouseButtonEventArgs e)
		{
			_isDown = false;
			_isDragging = false;
			_realDragSource.ReleaseMouseCapture();
		}

		private static void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (_isDown)
			{
				if ((_isDragging == false) && ((Math.Abs(e.GetPosition(null).X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
					   (Math.Abs(e.GetPosition(null).Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
				{
					_isDragging = true;
					_realDragSource = e.Source as UIElement;
					_realDragSource.CaptureMouse();
					DataObject dragData = new DataObject("SchematicElement", e.Source);
					DragDrop.DoDragDrop(_dummyDragSource, dragData, DragDropEffects.Copy);

					//ListViewItem lvitem = FindControlAncestor<ListViewItem>((DependencyObject)e.OriginalSource);
					//if (lvitem != null)
					//{
					//	SchematicElement element = (SchematicElement)((sender as StackPanel).ItemContainerGenerator.ItemFromContainer(lvitem));
					//	DataObject dragData = new DataObject("SchematicElement", element);
					//	DragDrop.DoDragDrop(lvitem, dragData, DragDropEffects.Move);
					//}
				}
			}
		}

		//private static T FindControlAncestor<T>(DependencyObject current) where T : DependencyObject
		//{
		//	do
		//	{
		//		if (current is T)
		//		{
		//			return (T)current;
		//		}
		//		current = VisualTreeHelper.GetParent(current);
		//	}
		//	while (current != null);
		//	return null;
		//}


		private static void OnDragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SchematicElement"))
			{
				e.Effects = DragDropEffects.Copy;
			}
			//if (!e.Data.GetDataPresent("SchematicElement") || sender == e.Source)
			//{
			//	e.Effects = DragDropEffects.None;
			//}
		}

		private static void OnDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SchematicElement"))
			{
				StackPanel senderTarget = sender as StackPanel;
				UIElement dropDest = e.Source as UIElement;
				int dropDestIndex = -1;
				int i = 0;

				foreach (UIElement element in senderTarget.Children)
				{
					if (element.Equals(dropDest))
					{
						dropDestIndex = i;
						break;
					}
					i++;
				}
				if(dropDestIndex != -1)
				{
					//((IDragDrop)dropDest).Drop(e);
					try
					{
						//senderTarget.Children.Remove(_realDragSource);
						//if(_realDragSource.)
						//senderTarget.Children.Insert(dropDestIndex, new Button(_realDragSource));

						//senderTarget.Children.Insert(dropDestIndex, new Button() { Width = 100, Height = 200 });
						//senderTarget.Children.Insert(dropDestIndex, (Button)_realDragSource);

						senderTarget.Children.Insert(dropDestIndex, new MySchematicElementControl() { Type = SchematicElementType.ResistorSerial });
						
					}
					catch (Exception err)
					{
						MessageBox.Show("Das hat wohl nicht geklappt. Fehlercode: " + err.Message) ;
					}
				}
				else
				{
					senderTarget.Children.Add(new MySchematicElementControl() { Type = SchematicElementType.ResistorSerial });
				}
				_isDown = false;
				_isDragging = false;
				_realDragSource.ReleaseMouseCapture();
			}
		}

		//private static IDragDrop FindDragDropAncestor(DependencyObject current)
		//{
		//	do
		//	{
		//		if (current is FrameworkElement)
		//		{
		//			object a = (current as FrameworkElement).DataContext;
		//			if (a is IDragDrop)
		//			{
		//				return a as IDragDrop;
		//			}
		//		}
		//		current = VisualTreeHelper.GetParent(current);
		//	} while (current != null);
		//	return null;
		//}
	}
}
