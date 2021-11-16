using Microsoft.Maps.MapControl.WPF;
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

namespace Project_Work_WPF.Views
{
	/// <summary>
	/// Interaction logic for User_Page_UserControl.xaml
	/// </summary>
	public partial class User_Page_UserControl : UserControl
	{
		public static Microsoft.Maps.MapControl.WPF.Location pinLocation;

		public User_Page_UserControl()
		{
			InitializeComponent();
		}

		private void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{

		}
	}
}
