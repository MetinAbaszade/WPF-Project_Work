using BingMapsRESTToolkit;
using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using Project_Work_WPF.Commands;
using Project_Work_WPF.Models;
using Project_Work_WPF.Navigation;
using Project_Work_WPF.Services;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Project_Work_WPF.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	class User_Page_ViewModel : BaseViewModel, IPageViewModel
	{
		public ApplicationIdCredentialsProvider Provider { get; set; } =
			new ApplicationIdCredentialsProvider(ConfigurationManager.AppSettings["apiKey"]);

		#region Variables

		public Microsoft.Maps.MapControl.WPF.Location center { get; set; }

		Departure currentdeparture = new Departure();

		public double zoomlevel { get; set; }

		List<Pushpin> taxies = new List<Pushpin>();

		Pushpin MyPushPin;
		Pushpin MyPushPin_2;
		Pushpin Taxi;

		ImageBrush imgB = new ImageBrush();

		MapPolyline routeLine;
		MapPolyline Taxi_routeLine;

		int taxi_bound;
		int route_bound;

		DispatcherTimer timer = new DispatcherTimer();

		private static readonly HttpClient _httpClient = new HttpClient();

		public ObservableCollection<UIElement> Route { get; set; } = new ObservableCollection<UIElement>();
		public ObservableCollection<Departure> Departures { get; set; } = new ObservableCollection<Departure>();

		public string From { get; set; }

		public string To { get; set; } = "Qara Qarayev Baku";

		public string Price { get; set; }

		#endregion

		#region Relay Commands

		public RelayCommand Rotate_Command { get; set; }

		public RelayCommand Get_Taxi_Command { get; set; }

		public RelayCommand From_Textbox_GotFocus_Command { get; set; }

		public RelayCommand To_Textbox_GotFocus_Command { get; set; }

		public RelayCommand Map_DoubleClick_Command { get; set; }


		private RelayCommand _goTo1;

		public RelayCommand Log_Out
		{
			get
			{
				return _goTo1 ?? (_goTo1 = new RelayCommand(x =>
				{
					Mediator.Notify("GoToLogIn", "");
				}));
			}
		}


		private RelayCommand _goTo2;

		public RelayCommand History_Command
		{
			get
			{
				return _goTo2 ?? (_goTo2 = new RelayCommand(x =>
				{
					Mediator.Notify("GoToHistory", "");
				}));
			}
		}

		#endregion

		string SelectedTxtBox;

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


		public object our_object { get; set; }
		public MouseButtonEventArgs mouseButtonEventArgs { get; set; }

		public User_Page_ViewModel()
		{
			Map_DoubleClick_Command = new RelayCommand(
				a =>
				{
					Map_DoubleClick(our_object, mouseButtonEventArgs);
				}
			);



			Rotate_Command = new RelayCommand(
				a =>
				{
					Rotate();
				}
			);

			Get_Taxi_Command = new RelayCommand(
				a =>
				{
					Get_Taxi();
				}
			);

			From_Textbox_GotFocus_Command = new RelayCommand(
				a =>
				{
					From_Textbox_GotFocus();
				}
			);

			To_Textbox_GotFocus_Command = new RelayCommand(
				a =>
				{
					To_Textbox_GotFocus();
				}
			);
			center = new Microsoft.Maps.MapControl.WPF.Location(40.4093, 49.8671);
			zoomlevel = 14;
			imgB.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Resources/Taxi Icon.png"));
			timer.Interval = new TimeSpan(0, 0, 0, 2);
			timer.Tick += Timer_Tick;
			GetCurrentLocation();
		}

		int counter = 0;

		#region Timer Ticks

		private void Timer_Tick(object sender, EventArgs e)
		{
			Taxi.Location = Taxi_routeLine.Locations[0];
			center = Taxi.Location;
			Taxi_routeLine.Locations.Remove(Taxi_routeLine.Locations[0]);
			counter++;
			if (counter > taxi_bound - 1)
			{
				Route.Add(routeLine);
				counter = 0;
				timer.Tick -= Timer_Tick;
				timer.Tick += Timer_Tick_2;
				timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
				currentdeparture.Date = DateTime.Now;
				currentdeparture.StartTime = DateTime.Now;
				Route.Remove(MyPushPin);
				timer.Stop();
				timer.Start();
			}
		}

		private void Timer_Tick_2(object sender, EventArgs e)
		{
			Taxi.Location = routeLine.Locations[0];
			center = Taxi.Location;
			routeLine.Locations.Remove(routeLine.Locations[0]);
			counter++;
			if (counter > route_bound - 1)
			{
				Route.Remove(routeLine);
				currentdeparture.EndTime = DateTime.Now;
				currentdeparture.Duration = currentdeparture.EndTime.Subtract(currentdeparture.StartTime);
				Departures.Add(currentdeparture);
				History_Page_ViewModel.Departures = Departures;
				timer.Stop();
			}
		}

		#endregion

		double Distance = 0;

		private async void Rotate()
		{

			try
			{
				currentdeparture = new Departure();
				Distance = 0;
				taxies.Clear();
				Route = new ObservableCollection<UIElement>();

				string URL = "http://dev.virtualearth.net/REST/V1/Routes/Driving?wp.0=" +
				$"{From}" + ",MN&wp.1=" + $"{To}" +
				",MN&optmz=distance&routeAttributes=routePath&key=" + ConfigurationManager.AppSettings["apiKey"];
				Uri geocodeRequest = new Uri(URL);

				Response r = await GetResponse(geocodeRequest);

				MyPushPin = new Pushpin();


				float currentdeparture_price = (float)(((Route)(r.ResourceSets[0].Resources[0])).TravelDistance * 0.4);
				currentdeparture.Cost = currentdeparture_price.ToString() + " AZN";
				Price = currentdeparture.Cost;
				currentdeparture.Distance = (float)((Route)(r.ResourceSets[0].Resources[0])).TravelDistance;


				var FromLatitude = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[0][0];
				var FromLongitude = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[0][1];

				var location = new Microsoft.Maps.MapControl.WPF.Location(FromLatitude, FromLongitude);
				MyPushPin.Location = location;

				Route.Add(MyPushPin);

				routeLine = new MapPolyline();
				routeLine.Locations = new LocationCollection();
				routeLine.Stroke = new SolidColorBrush(Colors.Blue);
				routeLine.Opacity = 150;

				route_bound = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates.GetUpperBound(0);

				MyPushPin_2 = new Pushpin();

				var FromLatitude_2 =
					((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[route_bound - 1][0];
				var FromLongitude_2 =
					((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates[route_bound - 1][1];

				var location_2 = new Microsoft.Maps.MapControl.WPF.Location(FromLatitude_2, FromLongitude_2);
				MyPushPin_2.Location = location_2;

				Route.Add(MyPushPin_2);

				zoomlevel = ((1 / ((Route)(r.ResourceSets[0].Resources[0])).TravelDistance) * 150);

				center = new Microsoft.Maps.MapControl.WPF.Location((location.Latitude + location_2.Latitude) / 2, (location.Longitude + location_2.Longitude) / 2);

				for (int i = 0; i < route_bound; i++)
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
				string latitude_2 = MyPushPin.Location.Latitude.ToString().Replace(',', '.');
				string longitude_2 = MyPushPin.Location.Longitude.ToString().Replace(',', '.');

				string URL = "http://dev.virtualearth.net/REST/V1/Routes/Driving?o=json&wp.0=" +
					latitude + "," +
					longitude + "&wp.1=" +
					latitude_2 + "," +
					longitude_2 + "&optmz=distance&rpo=Points&key=" +
					ConfigurationManager.AppSettings["apiKey"];

				geocodeRequest = new Uri(URL);
				r = await GetResponse(geocodeRequest);

				taxi_bound = ((Route)(r.ResourceSets[0].Resources[0])).RoutePath.Line.Coordinates.Length;

				Taxi_routeLine = new MapPolyline();
				Taxi_routeLine.Locations = new LocationCollection();
				Taxi_routeLine.Stroke = new SolidColorBrush(Colors.Red);
				Taxi_routeLine.StrokeThickness = 2;
				Taxi_routeLine.Opacity = 350;

				for (int i = 0; i < taxi_bound; i++)
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
				timer.Start();
			}
		}

		private void From_Textbox_GotFocus()
		{
			SelectedTxtBox = "From";
		}

		private void To_Textbox_GotFocus()
		{
			SelectedTxtBox = "To";
		}


		private static async Task<string> GetIPAddress()
		{
			var ipAddress = await _httpClient.GetAsync($"http://ipinfo.io/ip");
			if (ipAddress.IsSuccessStatusCode)
			{
				var json = await ipAddress.Content.ReadAsStringAsync();
				return json.ToString();
			}
			return "";
		}
		public async void GetCurrentLocation()
		{

			var ipAddress = await GetIPAddress();

			IpInfo ipInfo = new IpInfo();

			try
			{
				string info = new WebClient().DownloadString("http://ipinfo.io/" + ipAddress);
				ipInfo = JsonConvert.DeserializeObject<IpInfo>(info);
				Pushpin pushpin = new Pushpin();
				double lat = double.Parse(ipInfo.Loc.Split(',')[0]);
				double lon = double.Parse(ipInfo.Loc.Split(',')[1]);
				lat += 0.001;
				lon += 0.001;
				pushpin.Location = new Microsoft.Maps.MapControl.WPF.Location(lat, lon);
				center = pushpin.Location;
				Route.Add(pushpin);

				Uri geocodeRequest = new Uri("http://dev.virtualearth.net/REST/v1/Locations/" + lat.ToString() + "," + lon.ToString() +
					"?key=" + ConfigurationManager.AppSettings["apiKey"]);
				Response r = await GetResponse(geocodeRequest);

				From = ((BingMapsRESTToolkit.Location)r.ResourceSets[0].Resources[0]).Address.AddressLine + " Baku";
			}
			catch (Exception)
			{
			}
		}

		public void Map_DoubleClick(object sender, MouseButtonEventArgs e)
		{

		}

	}

}
