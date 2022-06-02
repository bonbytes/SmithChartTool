using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmithChartToolLibrary;

namespace SmithChartToolApp.View.Controls
{
    public class SchematicElementControl : ListBoxItem
    {
        static public readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(SchematicElementControl), new PropertyMetadata(string.Empty));
        static public readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(SchematicElementControl), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        static public readonly DependencyProperty DesignatorProperty = DependencyProperty.Register("Designator", typeof(string), typeof(SchematicElementControl), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        static public readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SchematicElementControl), new PropertyMetadata(null, OnCommandPropertyChanged));

        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string Designator
        {
            get { return (string)GetValue(DesignatorProperty); }
            set { SetValue(DesignatorProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        static SchematicElementControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SchematicElementControl), new FrameworkPropertyMetadata(typeof(SchematicElementControl)));
        }

        private void UpdateControl(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var elementData = sender.GetValue(DataContextProperty);
            var t = ((SchematicElement)elementData).Type.GetType();
            var attributeData = t.GetMember(((SchematicElement)elementData).Type.ToString());
            var attributes = attributeData[0].GetCustomAttributes(typeof(SchematicElementInfo), false);
            SchematicElementInfo sei = (SchematicElementInfo)attributes[0];

            var sri = Application.GetResourceStream(new Uri("pack://application:,,,/Images/SchematicElements/" + sei.Icon + ".xaml"));
            var content = XamlReader.Load(sri.Stream);
            Content = content;

            if (((SchematicElement)elementData).Type == SchematicElementType.Port)
            {
                Value = ((SchematicElement)elementData).Impedance.ToString() + " Ohms";
            }
            else
            {
                Value = ((SchematicElement)elementData).Value.ToString();
            }

            Designator = sei.Designator + ((SchematicElement)elementData).Designator.ToString();
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SchematicElementControl control = d as SchematicElementControl;
            if (control == null) return;

            control.MouseLeftButtonDown -= OnControlLeftClick;
            control.MouseLeftButtonDown += OnControlLeftClick;
        }

        private static void OnControlLeftClick(object sender, MouseButtonEventArgs e)
        {
            SchematicElementControl control = sender as SchematicElementControl;
            if (control == null || control.Command == null) return;

            ICommand command = control.Command;

            if (command.CanExecute(null))
                command.Execute(null);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateControl(this, new DependencyPropertyChangedEventArgs());
        }
    }
}

