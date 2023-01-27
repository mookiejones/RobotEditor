using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;

namespace RobotEditor.UI.Behaviors
{
    public class DataGridHelper
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetTextHeaderStyle(DependencyObject element, Style value) => element.SetValue(TextHeaderStyleProperty, value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Style GetTextHeaderStyle(DependencyObject element) => (Style)element.GetValue(TextHeaderStyleProperty);


        // Using a DependencyProperty as the backing store for TextHeaderStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextHeaderStyleProperty =
            DependencyProperty.RegisterAttached("TextHeaderStyle", typeof(Style), typeof(DataGridHelper), new PropertyMetadata(default(Style), OnTextHeaderChangedCallback));

        private static void OnTextHeaderChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DataGrid grid))
                return;

            if (e.OldValue == null && e.NewValue != null)
                grid.Columns.CollectionChanged += (obj2, e2) =>
                {
                    UpdateHeaderStyles(grid);
                };
        }




        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TextColumnStyleProperty =
            DependencyProperty.RegisterAttached("TextColumnStyle", typeof(Style), typeof(DataGridHelper), new PropertyMetadata(default(Style), TextColumnStyleChangedCallback));

        private static void TextColumnStyleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is DataGrid grid))
                return;

            if (e.OldValue == null && e.NewValue != null)
                grid.Columns.CollectionChanged += (obj2, e2) => UpdateColumnStyles(grid);
           

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void SetTextColumnStyle(DependencyObject element, Style value) => element.SetValue(TextColumnStyleProperty, value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Style GetTextColumnStyle(DependencyObject element) => (Style)element.GetValue(TextColumnStyleProperty);


        private static void UpdateColumnStyles(DataGrid grid)
        {
            var style = GetTextColumnStyle(grid);

            foreach (var column in grid.Columns.OfType<DataGridTextColumn>())
                foreach (var setter in style.Setters.OfType<Setter>())
                {
                    if (setter.Value is BindingBase setterBase)
                        BindingOperations.SetBinding(column, setter.Property, setterBase);
                    else
                        column.SetValue(setter.Property, setter.Value);
                }

        }

        private static void UpdateHeaderStyles(DataGrid grid)
        {
            var style = GetTextHeaderStyle(grid);

            foreach (var column in grid.Columns.OfType<DataGridTextColumn>())
                column.SetValue(DataGridTextColumn.HeaderStyleProperty, style);
            //foreach (var setter in style.Setters.OfType<Setter>())
            //    {
            //        column.SetValue(DataGridTextColumn.HeaderStyleProperty, style);
            //        if (setter.Value is BindingBase setterBase)
            //            BindingOperations.SetBinding(column, setter.Property, setterBase);
            //        else
            //            column.SetValue(setter.Property, setter.Value);
            //    }

        }
    }
}
