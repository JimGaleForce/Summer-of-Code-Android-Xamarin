using System.IO;
using Android.OS;
using Android.Widget;
using Java.IO;
using Java.Lang;
using Java.Net;
using IOException = Java.IO.IOException;

namespace QuakeX
{
    public class USGSService : AsyncTask<Void,Void,Void>
    {
        StringBuffer data = new StringBuffer();

        protected override Void RunInBackground(params Void[] @params)
        {
            try
            {
                URL url = new URL("https://earthquake.usgs.gov/fdsnws/event/1/query?format=geojson&starttime=2018-08-19&endtime=2018-09-09&minmagnitude=6&minlatitude=-30&maxlatitude=-10&minlongitude=-180&maxlongitude=-170");
                HttpURLConnection connection = (HttpURLConnection)url.OpenConnection();
                Stream inputStream = connection.InputStream;
                BufferedReader reader = new BufferedReader(new InputStreamReader(inputStream));

                string line = "";
                while (line != null)
                {
                    line = reader.ReadLine();
                    data.Append(line);
                }
            }
            catch (MalformedURLException e)
            {
                e.PrintStackTrace();
            }
            catch (IOException e)
            {
                e.PrintStackTrace();
            }

            return null;
        }

        protected override void OnPostExecute(Void aVoid)
        {
            base.OnPostExecute(aVoid);

            MainActivity.txt.SetText(data.ToString(), TextView.BufferType.Normal);
        }
    }
}