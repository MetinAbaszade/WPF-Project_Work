using BingMapsRESTToolkit;
using Microsoft.Maps.MapControl.WPF;
using Project_Work_WPF.Commands;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Project_Work_WPF.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	class User_Page_ViewModel
	{
		Map m = new Map();

		public static string From { get; set; }

		public static string To { get; set; }

		Predicate<object> Rotate_Predicate = new Predicate<object>(x => From != string.Empty && To != string.Empty);

		public RelayCommand Rotate_Command { get; set; }

		public ApplicationIdCredentialsProvider Provider { get; set; } =
			new ApplicationIdCredentialsProvider(ConfigurationManager.AppSettings["apiKey"]);

		private async Task<Response> GetResponse(Uri uri)
		{
			System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
			var response = await client.GetAsync(uri);
			using (var stream = await response.Content.ReadAsStreamAsync())
			{
				DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Response));
				return ser.ReadObject(stream) as Response;
			}
		}

		public User_Page_ViewModel()
		{

			Rotate_Command = new RelayCommand(
				a =>
				{
					Rotate();
				},
				Rotate_Predicate
			);

			//m.CredentialsProvider = Provider;
			//m.ZoomLevel = 2;
		}

		private async void Rotate()
		{

			try
			{
				string URL = "http://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0=" +
				$"{From}" + ",MN&wp.1=" + $"{To}" +
				",MN&optmz=distance&routeAttributes=routePath&key=" + ConfigurationManager.AppSettings["apiKey"];
				Uri geocodeRequest = new Uri(URL);

				Response r = await GetResponse(geocodeRequest);

				var MyPushPin = new Pushpin();

				var FromLatitude = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[0][0];
				var FromLongitude = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[0][1];

				var location = new Microsoft.Maps.MapControl.WPF.Location(FromLatitude, FromLongitude);

				MapLayer.SetPosition(MyPushPin, location);
				m.Children.Add(MyPushPin);

				MapPolyline routeLine = new MapPolyline();
				routeLine.Locations = new LocationCollection();
				routeLine.Stroke = new SolidColorBrush(Colors.Blue);
				routeLine.Opacity = 150;


				int bound = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates.GetUpperBound(0);

				var MyPushPin_2 = new Pushpin();

				var FromLatitude_2 =
					((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[bound - 1][0];
				var FromLongitude_2 =
					((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[bound - 1][1];

				var location_2 = new Microsoft.Maps.MapControl.WPF.Location(FromLatitude_2, FromLongitude_2);
				MapLayer.SetPosition(MyPushPin_2, location_2);
				m.Children.Add(MyPushPin_2);


				double zoomlevel = ((1 / ((Route)(r.ResourceSets[0].Resources[0])).TravelDistance) * 150);

				m.SetView(new Microsoft.Maps.MapControl.WPF.Location((location.Latitude + location_2.Latitude) / 2, (location.Longitude + location_2.Longitude) / 2), 7.0f);

				for (int i = 0; i < bound; i++)
				{
					routeLine.Locations.Add(new Microsoft.Maps.MapControl.WPF.Location
					{
						Latitude = ((Route)
						  (r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[i][0],
						Longitude = ((Route)
						  (r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[i][1]
					});
				}

				m.Children.Add(routeLine);
			}

			catch (Exception)
			{
				MessageBox.Show("Error Occured !!! Please Try Again");
			}
		}

	}
}
