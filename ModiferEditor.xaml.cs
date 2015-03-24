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
    /// Interaction logic for ModiferEditor.xaml
    /// </summary>
    public partial class ModiferEditor : Window
    {
        public ModiferEditor()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear all modifiers?", "Clear", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                DialogOption option = DataContext as DialogOption;
                foreach (var modifier in option.Modifiers)
                {
                    modifier.Skill = "";
                    modifier.Type = ModifierTypes.Equal;
                    modifier.Value = "";
                    modifier.OnPropertyChanged("Skill");
                    modifier.OnPropertyChanged("Type");
                    modifier.OnPropertyChanged("Value");
                }

            }
        }
    }
}
