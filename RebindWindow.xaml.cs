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

namespace DComposer
{
    /// <summary>
    /// Interaction logic for RebindWindow.xaml
    /// </summary>
    public partial class RebindWindow : Window
    {
        public RebindWindow()
        {
            InitializeComponent();
        }

        public ListBox DialogList { get { return lstDialogs; } }

        public DialogOption Option { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var page = lstDialogs.SelectedItem as DialogPage;
            if (page == null) return;

            var currentPage = Option.Parent;

            currentPage.Options.Remove(Option);
            page.AddOption(Option);
            this.Close();
        }
    }
}
