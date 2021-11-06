using Project_Work_WPF.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Work_WPF.ViewModels
{
	class Login_Page_ViewModel
	{ 
		public static string Username { get; set; }
		public static string Password { get; set; }

		public RelayCommand SignIn_Command { get; set; }

		public RelayCommand Hide_Command { get; set; }

		private void SignIn()
		{
			MainViewModel.Sign_in(Username, Password);
		}

		Predicate<object> SignIn_Predicate = 
			new Predicate<object>(x => Username != string.Empty && Password != string.Empty);

		//private void Hide_Button_Click()
		//{
		//	if (!Hidden)
		//	{
		//		password_box.Visibility = System.Windows.Visibility.Collapsed;
		//		MyTextBox.Text = password_box.Password;
		//		MyTextBox.Visibility = System.Windows.Visibility.Visible;

		//		MyTextBox.Focus();
		//		Hidden = true;
		//	}
		//	else
		//	{
		//		password_box.Password = MyTextBox.Text;
		//		password_box.Visibility = System.Windows.Visibility.Visible;
		//		MyTextBox.Visibility = System.Windows.Visibility.Collapsed;

		//		password_box.Focus();
		//		Hidden = false;
		//	}
		//}

		public Login_Page_ViewModel() {

			SignIn_Command = new RelayCommand(
				a =>
				{
					SignIn();
				},
				SignIn_Predicate
			);

		}
	}
}
