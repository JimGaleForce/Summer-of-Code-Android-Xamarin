using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace QuakeX
{
    public class QuakeAdapter : ArrayAdapter<Properties>
    {
        private Context context;
        private IList<Properties> quakeList;

        public QuakeAdapter(Context context, int textViewResourceId, IList<Properties> quakes) : base(context, textViewResourceId, quakes)
        {
            quakeList = quakes;
            this.context = context;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View listItem = convertView;
            if (listItem == null)
            {
                listItem = LayoutInflater.From(context).Inflate(Resource.Layout.quake_listview, parent, false);

                var properties = quakeList[position];

                var title = (TextView) listItem.FindViewById(Resource.Id.quaketitle);
                title.Text = properties.title;

                var url = (TextView) listItem.FindViewById(Resource.Id.quakeurl);
                url.Text = properties.url;
            }

            return listItem;
        }
    }
}