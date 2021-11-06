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

namespace Project_Work_WPF
{
	/// <summary>
	/// Interaction logic for Register.xaml
	/// </summary>
	public partial class Register : Page
	{
		bool Hidden = false;

		private void Hide_Button_Click(object sender, RoutedEventArgs e)
		{
			if (!Hidden)
			{
				password_box.Visibility = System.Windows.Visibility.Collapsed;
				MyTextBox.Text = password_box.Password;
				MyTextBox.Visibility = System.Windows.Visibility.Visible;

				MyTextBox.Focus();
				Hidden = true;
			}
			else
			{
				password_box.Password = MyTextBox.Text;
				password_box.Visibility = System.Windows.Visibility.Visible;
				MyTextBox.Visibility = System.Windows.Visibility.Collapsed;

				password_box.Focus();
				Hidden = false;
			}
		}
		public Register()
		{
			InitializeComponent();
		}
	}
}
