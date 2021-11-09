using BingMapsRESTToolkit;
using Microsoft.Maps.MapControl.WPF;
using Project_Work_WPF.Commands;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Project_Work_WPF.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	class User_Page_ViewModel
	{
		public double zoomlevel { get; set; }

		List<Pushpin> taxies = new List<Pushpin>();
		Pushpin MyPushPin, MyPushPin_2, Taxi;
		ImageBrush imgB = new ImageBrush();
		MapPolyline routeLine;

		public ObservableCollection<UIElement> Route { get; set; } = new ObservableCollection<UIElement>();

		public static string From { get; set; } = "Baku";

		public static string To { get; set; } = "Italy";

		Predicate<object> Rotate_Predicate = new Predicate<object>(x => From != string.Empty && To != string.Empty);

		public RelayCommand Rotate_Command { get; set; }

		public RelayCommand Get_Taxi_Command { get; set; }

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

			Get_Taxi_Command = new RelayCommand(
				a =>
				{
					Get_Taxi();
				},
				Rotate_Predicate
			);

			imgB.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Taxi Icon.png"));
		}


		double Distance = 0;


		private async void Rotate()
		{

			try
			{
				Distance = 0;
				taxies.Clear();
				Route = new ObservableCollection<UIElement>();

				string URL = "http://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0=" +
				$"{From}" + ",MN&wp.1=" + $"{To}" +
				",MN&optmz=distance&routeAttributes=routePath&key=" + ConfigurationManager.AppSettings["apiKey"];
				Uri geocodeRequest = new Uri(URL);

				Response r = await GetResponse(geocodeRequest);

				MyPushPin = new Pushpin();

				var FromLatitude = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[0][0];
				var FromLongitude = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[0][1];

				var location = new Microsoft.Maps.MapControl.WPF.Location(FromLatitude, FromLongitude);
				MyPushPin.Location = location;

				Route.Add(MyPushPin);

				routeLine = new MapPolyline();
				routeLine.Locations = new LocationCollection();
				routeLine.Stroke = new SolidColorBrush(Colors.Blue);
				routeLine.Opacity = 150;

				int bound = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates.GetUpperBound(0);

				MyPushPin_2 = new Pushpin();

				var FromLatitude_2 =
					((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[bound - 1][0];
				var FromLongitude_2 =
					((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[bound - 1][1];

				var location_2 = new Microsoft.Maps.MapControl.WPF.Location(FromLatitude_2, FromLongitude_2);
				MyPushPin_2.Location = location_2;

				Route.Add(MyPushPin_2);

				zoomlevel = ((1 / ((Route)(r.ResourceSets[0].Resources[0])).TravelDistance) * 150);


				//Route.SetView(new Microsoft.Maps.MapControl.WPF.Location((location.Latitude + location_2.Latitude) / 2, (location.Longitude + location_2.Longitude) / 2), 7.0f);

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

				Route.Add(routeLine);

				string Latitude, Longitude, Latitude_2, Longitude_2, url;
				Uri geocodeRequest_2;
				Response r_2;
				double fromLatitude, fromLongitude;
				int bound_2, index;
				Random random = new Random();

				for (int i = 0; i < 5; i++)
				{
					double a = random.NextDouble() * (0.017 - 0.005) + 0.005;
					double b = random.NextDouble() * (0.017 - 0.005) + 0.005;

					if (random.Next(0, 2) == 0)
					{
						a = a * -1;
					}

					if (random.Next(0, 2) == 0)
					{
						b = b * -1;
					}

					Pushpin pushpin = new Pushpin();

					pushpin.Location = new Microsoft.Maps.MapControl.WPF.Location(FromLatitude + a, FromLongitude + b);

					Latitude = pushpin.Location.Latitude.ToString().Replace(',', '.');
					Longitude = pushpin.Location.Longitude.ToString().Replace(',', '.');
					Latitude_2 = MyPushPin.Location.Latitude.ToString().Replace(',', '.');
					Longitude_2 = MyPushPin.Location.Longitude.ToString().Replace(',', '.');

					url = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
					   Latitude + "," +
					   Longitude + "&wp.1=" +
					   Latitude_2 + "," +
					   Longitude_2 + "&optmz=distance&rpo=Points&key=" +
					   ConfigurationManager.AppSettings["apiKey"];

					geocodeRequest_2 = new Uri(url);
					r_2 = await GetResponse(geocodeRequest_2);


					bound_2 = ((Route)(r_2.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates.GetUpperBound(0);

					index = random.Next(0, bound_2 - 1);

					fromLatitude = ((Route)(r_2.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[index][0];
					fromLongitude = ((Route)(r_2.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[index][1];

					pushpin.Location = new Microsoft.Maps.MapControl.WPF.Location(fromLatitude, fromLongitude);

					pushpin.Background = imgB;

					taxies.Add(pushpin);
					Route.Add(pushpin);

					Latitude = pushpin.Location.Latitude.ToString().Replace(',', '.');
					Longitude = pushpin.Location.Longitude.ToString().Replace(',', '.');
					Latitude_2 = MyPushPin.Location.Latitude.ToString().Replace(',', '.');
					Longitude_2 = MyPushPin.Location.Longitude.ToString().Replace(',', '.');

					url = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
					  Latitude + "," +
					  Longitude + "&wp.1=" +
					  Latitude_2 + "," +
					  Longitude_2 + "&optmz=distance&rpo=Points&key=" +
					  ConfigurationManager.AppSettings["apiKey"];

					geocodeRequest_2 = new Uri(url);
					r_2 = await GetResponse(geocodeRequest_2);

					if (Distance == 0)
					{
						Distance = ((Route)(r_2.ResourceSets[0].Resources[0])).TravelDistance;
					}

					if (Distance > ((Route)(r_2.ResourceSets[0].Resources[0])).TravelDistance)
					{
						Distance = ((Route)(r_2.ResourceSets[0].Resources[0])).TravelDistance;
					}
				}
			}

			catch (Exception)
			{
				MessageBox.Show("Error Occured !!! Please Try Again");
			}

		}

		private async void Get_Taxi()
		{
			if (taxies.Count > 0)
			{
				string Latitude, Longitude, Latitude_2, Longitude_2, url;
				Uri geocodeRequest;
				Response r;

				foreach (var item in taxies)
				{
					Latitude = item.Location.Latitude.ToString().Replace(',', '.');
					Longitude = item.Location.Longitude.ToString().Replace(',', '.');
					Latitude_2 = MyPushPin.Location.Latitude.ToString().Replace(',', '.');
					Longitude_2 = MyPushPin.Location.Longitude.ToString().Replace(',', '.');

					url = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
					   Latitude + "," +
					   Longitude + "&wp.1=" +
					   Latitude_2 + "," +
					   Longitude_2 + "&optmz=distance&rpo=Points&key=" +
					   ConfigurationManager.AppSettings["apiKey"];

					geocodeRequest = new Uri(url);
					r = await GetResponse(geocodeRequest);

					if (Distance != ((Route)(r.ResourceSets[0].Resources[0])).TravelDistance)
					{
						Route.Remove(item);
					}

					else
					{
						Taxi = item;
					}

				}


				string latitude = Taxi.Location.Latitude.ToString().Replace(',', '.');
				string longitude = Taxi.Location.Longitude.ToString().Replace(',', '.');
				string latitude_2 = MyPushPin_2.Location.Latitude.ToString().Replace(',', '.');
				string longitude_2 = MyPushPin_2.Location.Longitude.ToString().Replace(',', '.');

				string URL = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
					latitude + "," +
					longitude + "&wp.1=" +
					latitude_2 + "," +
					longitude_2 + "&optmz=distance&rpo=Points&key=" +
					ConfigurationManager.AppSettings["apiKey"];

				geocodeRequest = new Uri(URL);
				r = await GetResponse(geocodeRequest);
				int bound = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates.Length;

				MapPolyline Taxi_routeLine = new MapPolyline();
				Taxi_routeLine.Locations = new LocationCollection();
				Taxi_routeLine.Stroke = new SolidColorBrush(Colors.Red);
				Taxi_routeLine.StrokeThickness = 2;
				Taxi_routeLine.Opacity = 350;

				for (int i = 0; i < bound; i++)
				{
					Taxi_routeLine.Locations.Add(new Microsoft.Maps.MapControl.WPF.Location
					{
						Latitude = ((Route)
						  (r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[i][0],
						Longitude = ((Route)
						  (r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[i][1]
					});
				}

				Route.Remove(routeLine);
				Route.Add(Taxi_routeLine);

				MessageBox.Show("Finished!!");
			}
		}
	}
}
