using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Maps.MapControl.WPF;
using BingMapsRESTToolkit;
using System.Windows;
using Project_Work_WPF.Views;
using Project_Work_WPF.Models;
using System.Windows.Controls;
using PropertyChanged;

namespace Project_Work_WPF.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	public class MainViewModel
	{
		public static string From;

		public static Uri uri { get; set; } 

		Page page = new Page();

		MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

		Login_Page login_page = new Login_Page();

		User_Page user_page = new User_Page();

		static List<Person> Users = new List<Person>();
		static List<Person> Admins = new List<Person>();

		public static void Sign_in(string username, string password)
		{
			if (From == "Admin")
			{
				if (Admins.Exists(x => x.Username == username && x.Password == password))
				{
					uri = new Uri("Admin_Page.xaml", UriKind.Relative);
				}
			}

			else
			{
				if (Users.Exists(x => x.Username == username && x.Password == password))
				{
					uri = new Uri("User_Page.xaml", UriKind.Relative);
				}
			}

		}

		public MainViewModel()
		{
			uri = new Uri("User_Page.xaml", UriKind.Relative);
		}
	}
}
