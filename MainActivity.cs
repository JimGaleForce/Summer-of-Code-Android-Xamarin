using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;

using Newtonsoft;
using Newtonsoft.Json;

namespace QuakeX
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.activity_main);

			Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            ImageButton image = FindViewById<ImageButton>(Resource.Id.image);
            image.Click += ImageOnClick;
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void ImageOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();

            var usgs = new BackgroundWorker();

            List<Properties> quakes = null;
            var list = (ListView)FindViewById(Resource.Id.list);
            list.ItemClick += (obj, args) =>
            {
                var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(quakes[args.Position].url));
                intent.AddFlags(ActivityFlags.NewTask);
                intent.SetPackage("com.microsoft.emmx");

                this.ApplicationContext.StartActivity(intent);
            };

            usgs.DoWork += async (obj, args) =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var data = await client.GetStringAsync("https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime=2018-08-19&endtime=2018-09-09&minmagnitude=6&minlatitude=-30&maxlatitude=-10&minlongitude=-180&maxlongitude=-170");

                        var quake = JsonConvert.DeserializeObject<Quake>(data);
                        quakes = quake.features.Select(f => f.properties).ToList();
                        
                        RunOnUiThread(() =>
                        {
                            var adapter = new QuakeAdapter(this.ApplicationContext, Android.Resource.Layout.SimpleListItem1, quakes);
                            list.Adapter = adapter;
                        });
                    }
                }
                catch (HttpRequestException exception)
                {
                    Log.Error(this.GetType().ToString(), exception.ToString());
                }
            };

            usgs.RunWorkerAsync();
        }
	}
}

