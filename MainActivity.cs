using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using Android.App;
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

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
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

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();

            var usgs = new BackgroundWorker();

            usgs.DoWork += async (obj, args) =>
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var data = await client.GetStringAsync("https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime=2018-08-19&endtime=2018-09-09&minmagnitude=6&minlatitude=-30&maxlatitude=-10&minlongitude=-180&maxlongitude=-170");

                        var quake = JsonConvert.DeserializeObject<Quake>(data);
                        var result = new StringBuilder();
                        quake.features.ToList().ForEach(f => result.AppendLine(f.properties.title));
                        
                        RunOnUiThread(() =>
                        {
                            var txt = (TextView)FindViewById(Resource.Id.txt);
                            txt.SetText(result.ToString(), TextView.BufferType.Normal);
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

