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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DComposer
{
    /// <summary>
    /// Interaction logic for DialogOptionControl.xaml
    /// </summary>
    public partial class DialogOptionControl : UserControl
    {
        public event RoutedEventHandler OnOpen;
        public event RoutedEventHandler OnRefresh;

        public DialogOptionControl()
        {
            InitializeComponent();
        }

        private void Conditions_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var option = (DialogOption)DataContext;

            ConditionEditor editor = new ConditionEditor();
            editor.DataContext = option;
            editor.ShowDialog();
            option.OnPropertyChanged("ConditionsText");
        }

        private void Setters_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var option = (DialogOption)DataContext;

            ModiferEditor editor = new ModiferEditor();
            editor.DataContext = option;
            editor.ShowDialog();
            option.OnPropertyChanged("SettersText");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (OnOpen != null)
            {
                OnOpen(sender, e);
            }
        }


        //move up
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var option = (DialogOption)DataContext;
            var parent = option.Parent;

            if (parent == null)
            {
                return;
            }

            var idx = parent.Options.IndexOf(option);

            if (idx == 0 )
            {
                return;
            }

            parent.Options.RemoveAt(idx);
            parent.Options.Insert(idx - 1, option);

            if (OnRefresh != null)
            {
                OnRefresh(sender, e);
            }

        }

        //move down
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var option = (DialogOption)DataContext;
            var parent = option.Parent;

            if (parent == null)
            {
                return;
            }
            var idx = parent.Options.IndexOf(option);

            if (idx == parent.Options.Count-1)
            {
                return;
            }

            if (parent.Options[idx +1].Parent == null)
            {
                return;
            }

            parent.Options.RemoveAt(idx);
            parent.Options.Insert(idx+1, option);

            if (OnRefresh != null)
            {
                OnRefresh(sender, e);
            }


        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var box = (TextBox)sender;

            var option = (DialogOption)DataContext;

            if (!option.Enabled && !String.IsNullOrWhiteSpace( box.Text ) )
            {
                option.Label = box.Text;
                OnRefresh(sender, e);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            DialogOption option = (DialogOption)DataContext;
            if (!option.Enabled) return;


            RebindWindow rebind = new RebindWindow();
            var lstbox = rebind.DialogList;

            DialogPage parent = null;
            DialogLabel label = (DialogLabel)option;
            while(label.Parent != null)
            {
                label = label.Parent;
            }
            parent = label as DialogPage;

            List<DialogPage> pages = new List<DialogPage>();
            FindPages(parent, pages);


            foreach(var page in pages)
            {
                if (page == option.Parent) continue;
                if (option.Contains(page)) continue;
                lstbox.Items.Add(page);
            }
            rebind.Option = option;

            rebind.ShowDialog();
            if (OnRefresh != null)
            {
                OnRefresh(sender, e);
            }

            var owner = parent.Owner;
            owner.Rebind();
        }

        private void FindPages(DialogPage page, List<DialogPage> pages)
        {
            pages.Add(page);
            foreach(var option in page.Options)
            {
                if (option.Target != null)
                {
                    FindPages(option.Target, pages);
                }
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            DialogOption option = (DialogOption)DataContext;
            if (!option.Enabled) return;

            var result = MessageBox.Show("Are you sure you want to delete this node?", "Delete", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DialogPage parent = null;
                DialogLabel label = (DialogLabel)option;
                while (label.Parent != null)
                {
                    label = label.Parent;
                }
                parent = label as DialogPage;

                option.Parent.Options.Remove(option);

                if (OnRefresh != null)
                {
                    OnRefresh(sender, e);
                }
                var owner = parent.Owner;
                owner.Rebind();
            }
        }
    }
}
