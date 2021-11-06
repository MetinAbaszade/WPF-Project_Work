using BingMapsRESTToolkit;
using Microsoft.Maps.MapControl.WPF;
using Project_Work_WPF.ViewModels;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization.Json;
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

namespace Project_Work_WPF.Views
{
	/// <summary>
	/// Interaction logic for User_Page.xaml
	/// </summary>
	/// 
	[AddINotifyPropertyChangedInterface]
	public partial class User_Page : Page
	{
		public User_Page()
		{
			InitializeComponent();
			DataContext = new User_Page_ViewModel(); 
		}
	}
}
