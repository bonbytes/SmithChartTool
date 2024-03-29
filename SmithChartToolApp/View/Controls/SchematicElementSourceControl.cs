﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    //[ContentProperty("OtherPropertyNameThanContent")]
    public class SchematicElementSourceControl : ContentControl
    {
        static public DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(string), typeof(SchematicElementSourceControl), new PropertyMetadata(string.Empty, OnTypeChanged));
        static public DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(SchematicElementSourceControl), new PropertyMetadata(string.Empty));

        public string Type
        {
            get { return (string)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        static SchematicElementSourceControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SchematicElementSourceControl), new FrameworkPropertyMetadata(typeof(SchematicElementSourceControl)));
        }

        private void UpdateControl()
        {
            var a = typeof(SchematicElementType).FromName(Type);
            if (a != null)
            {
                Type t = a.GetType();
                var b = t.GetMember(a.ToString());

                if (b.Count() > 0)
                {
                    var c = b[0].GetCustomAttributes(typeof(SchematicElementInfo), false);
                    if (c.Count() > 0)
                    {
                        SchematicElementInfo sei = (SchematicElementInfo)c[0];
                        if (sei != null)
                        {
                            Header = sei.Name;
                            var sri = Application.GetResourceStream(new Uri("pack://application:,,,/Images/SchematicElements/" + sei.Icon + ".xaml"));
                            var content = XamlReader.Load(sri.Stream);
                            Content = content;
                        }
                    }
                }
            }
        }

        public static void OnTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as SchematicElementSourceControl).UpdateControl();
        }

        //public override void OnApplyTemplate()
        //{
        //    base.OnApplyTemplate();
        //DependencyObject b = GetTemplateChild("PART_MySchematicElementSourceImage"); // UI element out of template
        //if(b != null && (b.GetType() == typeof(Image)))
        //{
        //    img = b as Image;
        //    UpdateImage();
        //}

        //UpdateControl();
        //}
    }
}
