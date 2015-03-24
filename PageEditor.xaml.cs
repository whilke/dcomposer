using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DComposer
{
    /// <summary>
    /// Interaction logic for PageEditor.xaml
    /// </summary>
    public partial class PageEditor : Window
    {
        public PageEditor()
        {
            InitializeComponent();


            Loaded += PageEditor_Loaded;
            Closing += PageEditor_Closing;
        }

        void PageEditor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var page = DataContext as DialogPage;
            foreach(var option in page.Options)
            {
                page.EnableOption(option);
            }
        }

        void PageEditor_Loaded(object sender, RoutedEventArgs e)
        {
            BindOptions();
        }

        private void BindOptions()
        {
            DialogPage dp = (DialogPage)DataContext;

            bool needNew = true;
            var options = dp.Options;
            foreach(var option in options)
            {
                if (String.IsNullOrEmpty(option.Label))
                    needNew = false;

                DialogOptionControl ctr = new DialogOptionControl();
                ctr.OnOpen += ctr_OnOpen;
                ctr.OnRefresh += ctr_OnRefresh;
                ctr.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                ctr.DataContext = option;

                stackPanel.Children.Add(ctr);
            }

            if (needNew)
            {
                DialogOption option = new DialogOption();

                DialogOptionControl ctr = new DialogOptionControl();
                ctr.OnOpen += ctr_OnOpen;
                ctr.OnRefresh += ctr_OnRefresh;
                ctr.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                ctr.DataContext = option;
                stackPanel.Children.Add(ctr);
                dp.AddOption(option);
            }

        }

        void ctr_OnRefresh(object sender, RoutedEventArgs e)
        {
            DialogOption senderOption = (DialogOption)(((Control)sender).DataContext);
            var page = DataContext as DialogPage;

            if (!senderOption.Enabled && !String.IsNullOrWhiteSpace(senderOption.Label))
            {
                page.EnableOption(senderOption);
            }

            page.OnPropertyChanged("Options");

            foreach(var option in page.Options)
            {
                option.RefreshBinding();
            }
            stackPanel.Children.Clear();
            BindOptions();
        }

        void ctr_OnOpen(object sender, RoutedEventArgs e)
        {
            var option = ((Control)sender).DataContext as DialogOption;
            if (option.Target != null)
            {
                stackPanel.Children.Clear();
                this.DataContext = option.Target;
                BindOptions();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var page = (this.DataContext as DialogPage);
            if (page.PageOwner != null)
            {
                stackPanel.Children.Clear();
                this.DataContext = page.PageOwner;
                BindOptions();
            }
        }
    }
}
