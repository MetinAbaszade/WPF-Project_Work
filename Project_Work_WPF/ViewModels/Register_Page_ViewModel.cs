using Project_Work_WPF.Commands;
using Project_Work_WPF.Navigation;
using PropertyChanged;
using System;
using System.Windows;
using System.Windows.Input;

namespace Project_Work_WPF.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	public class Register_Page_ViewModel : BaseViewModel, IPageViewModel
	{
		public static string Username { get; set; } = string.Empty;
		public static string Password { get; set; } = string.Empty;
		public string Passwordd { get; set; } = string.Empty;
		public static string Repeat_Password { get; set; } = string.Empty;
		public RelayCommand Register_Command { get; set; }





		static void Register(object obj)
		{
			if (MainViewModel.Logged_As == "User")
			{
				try
				{
					MainViewModel.Add_User(Username, Password);
					MessageBox.Show("Succesfully Completed");
					GoTo_SignIn.Execute(obj);

				}
				catch (Exception a)
				{
					MessageBox.Show(a.Message);
				}
			}

			else
			{
				try
				{
					MainViewModel.Add_Admin(Username, Password);
					MessageBox.Show("Succesfully Completed");
					GoTo_SignIn.Execute(obj);
				}
				catch (Exception a)
				{
					MessageBox.Show(a.Message);
				}
			}
		}

		Action<object> Register_Actoin = Register;

		Predicate<object> Register_Predicate = new Predicate<object>(x => Username != string.Empty &&
																		  Password != string.Empty &&
																		  Password == Repeat_Password);

		public bool Hidden = false;
		public System.Windows.Visibility password_box_visibility { get; set; } = System.Windows.Visibility.Visible;
		public System.Windows.Visibility password_box_visibility_2 { get; set; } = System.Windows.Visibility.Collapsed;


		private void Hide_Button_Click(object obj)
		{
			Passwordd = Password;

			if (!Hidden)
			{
				password_box_visibility = System.Windows.Visibility.Hidden;
				password_box_visibility_2 = System.Windows.Visibility.Visible;

				Hidden = true;
			}

			else
			{
				password_box_visibility = System.Windows.Visibility.Visible;
				password_box_visibility_2 = System.Windows.Visibility.Hidden;

				Hidden = false;
			}

		}



		public static ICommand GoTo_SignIn
		{
			get
			{
				return new RelayCommand(x =>
				{
					Username = string.Empty;
					Password = string.Empty;
					Repeat_Password = string.Empty;

					Mediator.Notify("GoToLogIn", "");
				});
			}
		}

		public RelayCommand Hide { get; set; }

		public Register_Page_ViewModel()
		{
			Register_Command = new RelayCommand(Register_Actoin, Register_Predicate);
			Hide = new RelayCommand(Hide_Button_Click);
		}
	}
}
