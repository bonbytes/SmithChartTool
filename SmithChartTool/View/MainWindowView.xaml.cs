using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmithChartTool.ViewModel;
using SmithChartTool.Model;

namespace SmithChartTool.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowViewModel VM { get; set; } = new MainWindowViewModel();

        public MainWindowView()
        {

            this.InitializeComponent();
            this.DataContext = VM;
            CommandBindings.Add(new CommandBinding(MainWindowViewModel.CommandXYAsync, (s, e) => { VM.RunCommandXYAsync(); }, (s, e) => { Debug.Print("Blub"); })); //e.CanExecute = bli; }));
            
        }

        private Point startPoint;

        private void lvSourceMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }
        private void lvSourceMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            // fix dragdrop distance to parent 
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance
                ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
                )
            {
                ListView lv = sender as ListView;
                ListViewItem lvitem = FindAnchestor<ListViewItem>((DependencyObject)e.OriginalSource);

                if(lvitem != null)
                {
                    SchematicElement element = (SchematicElement)lv.ItemContainerGenerator.ItemFromContainer(lvitem);

                    DataObject dragData = new DataObject("myFormat", element);
                    DragDrop.DoDragDrop(lvitem, dragData, DragDropEffects.Move);
                }
            }
        }

        private static T FindAnchestor<T>(DependencyObject current) where T: DependencyObject
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

        private void lvDestDragEnter(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent("myFormat") || sender == e.Source)
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void lvDestDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("myFormat"))
            {
                SchematicElement element = e.Data.GetData("myFormat") as SchematicElement;
                ListView listView = sender as ListView;
                listView.Items.Add(element);
            }
        }

    }
}
