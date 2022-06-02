using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmithChartToolLibrary
{
    public class ObservableSchematicList: ObservableCollection<SchematicElement>
    {
        public ObservableSchematicList()
        {
            CollectionChanged += (s, e) =>
            {
                /* In case Move / Rearrange
                
                foreach (var item in e.OldItems)
                {
                    ((SchematicElement)item).SchematicElementChanged -= ChangeHandler;
                }
                foreach (var item in e.NewItems)
                {
                    ((SchematicElement)item).SchematicElementChanged += ChangeHandler;
                }

                */

                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        foreach (var item in e.NewItems)
                        {
                            ((SchematicElement)item).SchematicElementChanged += ChangeHandler;
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        foreach (var item in e.OldItems)
                        {
                            ((SchematicElement)item).SchematicElementChanged -= ChangeHandler;
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        break;
                    default:
                        break;
                }

            };

        }

        private void ChangeHandler(object sender, PropertyChangedEventArgs e)
        {
            OnSchematicElementChanged((SchematicElement)sender, e);
        }

        public event PropertyChangedEventHandler SchematicElementChanged;
        protected void OnSchematicElementChanged(SchematicElement item, PropertyChangedEventArgs e)
        {
            SchematicElementChanged?.Invoke(item, e);
        }

    }
}
