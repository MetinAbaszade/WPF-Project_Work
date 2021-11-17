﻿using BingMapsRESTToolkit;
using Microsoft.Maps.MapControl.WPF;
using Project_Work_WPF.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Project_Work_WPF.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	class Admin_Page_DriverOnMap_ViewModel : BaseViewModel, IPageViewModel
	{
		Random random = new Random();

		public ApplicationIdCredentialsProvider Provider { get; set; } =
					new ApplicationIdCredentialsProvider(ConfigurationManager.AppSettings["apiKey"]);

		ImageBrush imgB = new ImageBrush();

		public ObservableCollection<UIElement> Route { get; set; } = new ObservableCollection<UIElement>();

		public Microsoft.Maps.MapControl.WPF.Location center { get; set; } = new Microsoft.Maps.MapControl.WPF.Location();

		public double zoomlevel { get; set; } = new double();

		public async void SetDriversOnMap()
		{
			center = new Microsoft.Maps.MapControl.WPF.Location(40.4093, 49.8671);
			zoomlevel = 14;
			Route.Clear();
			double latitude;
			double longtitude;
			double latitude_2;
			double longtitude_2;
			int route_bound;
			int random_location;
			for (int i = 0; i < Admin_UserPage_ViewModel.Drivers.Count; i++)
			{
				latitude = random.NextDouble() * (40.506065153555 - 40.3886514287182) + 40.3886514287182;
				longtitude = random.NextDouble() * (50.1286514287182 - 49.856065153555) + 49.856065153555;
				latitude_2 = random.NextDouble() * (40.506065153555 - 40.3886514287182) + 40.3886514287182;
				longtitude_2 = random.NextDouble() * (50.1286514287182 - 49.856065153555) + 49.856065153555;

				string URL = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
			  latitude + "," +
			  longtitude + "&wp.1=" +
			  latitude_2 + "," +
			  longtitude_2 + "&optmz=distance&rpo=Points&key=" +
			  ConfigurationManager.AppSettings["apiKey"];

				var geocodeRequest = new Uri(URL);
				var r = await User_Page_ViewModel.GetResponse(geocodeRequest);
				route_bound = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates.Length;
				random_location = random.Next(0, route_bound - 1);

				Pushpin pushpin = new Pushpin();
				pushpin.Location = new Microsoft.Maps.MapControl.WPF.Location(
					((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[random_location][0],
					((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[random_location][1]
				);

				pushpin.Background = imgB;
				Route.Add(pushpin);
			}
		
		}

		public Admin_Page_DriverOnMap_ViewModel()
		{
			imgB.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Driver Icon.png"));
			imgB.Viewport = new Rect(-0.29, -0.3, 1.7, 1.7);
		}
	}
}
