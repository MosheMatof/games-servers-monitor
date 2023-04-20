using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFClient.Behaviors
{
    public class DialogResultBehavior : Behavior<MetroWindow>
    {
        public static readonly DependencyProperty DialogResultProperty =
            DependencyProperty.Register("DialogResult", typeof(bool?), typeof(DialogResultBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, DialogResultChangedCallback));

        private static void DialogResultChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                window.DialogResult = e.NewValue as bool?;
            }
        }

        public bool? DialogResult
        {
            get { return (bool?)GetValue(DialogResultProperty); }
            set { SetValue(DialogResultProperty, value); }
        }

        public void SetDialogResult(bool? result)
        {
            DialogResult = result;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Window window)
            {
                window.Closed += Window_Closed;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (sender is Window window)
            {
                DialogResult = window.DialogResult;
            }
        }
    }
}
