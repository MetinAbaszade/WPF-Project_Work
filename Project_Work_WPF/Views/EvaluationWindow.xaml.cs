using Project_Work_WPF.ViewModels;
using PropertyChanged;
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

namespace Project_Work_WPF.Views
{
	/// <summary>
	/// Interaction logic for EvaluationWindow.xaml
	/// </summary>
	/// 
	[AddINotifyPropertyChangedInterface]
	public partial class EvaluationWindow : Window
	{
		public EvaluationWindow()
		{
			InitializeComponent();
			DataContext = this;
		}

		public static int star_count = 0;

		public string Button1_Source { get; set; } = "/Resources/Black StarIcon.png";
		public string Button2_Source { get; set; } = "/Resources/Black StarIcon.png";
		public string Button3_Source { get; set; } = "/Resources/Black StarIcon.png";
		public string Button4_Source { get; set; } = "/Resources/Black StarIcon.png";
		public string Button5_Source { get; set; } = "/Resources/Black StarIcon.png";

		private void Star_Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (sender is Button button)
			{
				if (button.Name == "Button1")
				{
					if (Button1_Source == "/Resources/Black StarIcon.png")
					{
						Button1_Source = "/Resources/Yellow StarIcon.png";
					}
					else
					{
						Button2_Source = "/Resources/Black StarIcon.png";
						Button3_Source = "/Resources/Black StarIcon.png";
						Button4_Source = "/Resources/Black StarIcon.png";
						Button5_Source = "/Resources/Black StarIcon.png";
					}
					star_count = 1;
				}
				else if (button.Name == "Button2")
				{
					if (Button2_Source == "/Resources/Black StarIcon.png")
					{
						Button1_Source = "/Resources/Yellow StarIcon.png";
						Button2_Source = "/Resources/Yellow StarIcon.png";
					}
					else
					{
						Button3_Source = "/Resources/Black StarIcon.png";
						Button4_Source = "/Resources/Black StarIcon.png";
						Button5_Source = "/Resources/Black StarIcon.png";
					}
					star_count = 2;
				}

				else if (button.Name == "Button3")
				{
					if (Button3_Source == "/Resources/Black StarIcon.png")

					{
						Button1_Source = "/Resources/Yellow StarIcon.png";
						Button2_Source = "/Resources/Yellow StarIcon.png";
						Button3_Source = "/Resources/Yellow StarIcon.png";
					}
					else
					{
						Button4_Source = "/Resources/Black StarIcon.png";
						Button5_Source = "/Resources/Black StarIcon.png";
					}
					star_count = 3;
				}
				else if (button.Name == "Button4")
				{
					if (Button4_Source == "/Resources/Black StarIcon.png")
					{
						Button1_Source = "/Resources/Yellow StarIcon.png";
						Button2_Source = "/Resources/Yellow StarIcon.png";
						Button3_Source = "/Resources/Yellow StarIcon.png";
						Button4_Source = "/Resources/Yellow StarIcon.png";
					}
					else
					{
						Button5_Source = "/Resources/Black StarIcon.png";
					}
					star_count = 4;
				}
				else
				{
					if (Button5_Source == "/Resources/Black StarIcon.png")
					{
						Button1_Source = "/Resources/Yellow StarIcon.png";
						Button2_Source = "/Resources/Yellow StarIcon.png";
						Button3_Source = "/Resources/Yellow StarIcon.png";
						Button4_Source = "/Resources/Yellow StarIcon.png";
						Button5_Source = "/Resources/Yellow StarIcon.png";
					}
					star_count = 5;
				}

			}
		}

		private void Delete_Button_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			Button1_Source = "/Resources/Black StarIcon.png";
			Button2_Source = "/Resources/Black StarIcon.png";
			Button3_Source = "/Resources/Black StarIcon.png";
			Button4_Source = "/Resources/Black StarIcon.png";
			Button5_Source = "/Resources/Black StarIcon.png";
			star_count = 0;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
