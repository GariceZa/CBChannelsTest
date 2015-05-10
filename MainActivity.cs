using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Couchbase.Lite;
using Android.Util;
using System.Collections.Generic;

namespace CBChannelsTest
{
	[Activity (Label = "CBChannelsTest", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		//Couchbase
		static readonly string dbName = "channeltestdb";
		Database mDatabase { get; set; }
		Query mQuery { get; set; }
		LiveQuery mLiveQuery { get; set; }
		//---------------------

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);

			Button button = FindViewById<Button> (Resource.Id.myButton);
			EditText edittext = FindViewById<EditText> (Resource.Id.myEditText);

			//Returns the db with the given name, if the db does not exist it is created
			mDatabase = Manager.SharedInstance.GetDatabase (dbName);

			startReplication ();


			button.Click += delegate {

				//Adds new document
				Documents docs = new Documents();
				docs.addContent(mDatabase,edittext.Text,"Channel2"); 

			};
			setupLiveQuery();	
		}

		private void setupLiveQuery()
		{
			QueryEnumerator mEnumerator;
			Documents docs = new Documents ();
			mLiveQuery = docs.getAllContent (mDatabase).ToLiveQuery ();
			mLiveQuery.Changed += delegate(object sender, QueryChangeEventArgs e) {

				mEnumerator = e.Rows;

				foreach (var q in mEnumerator) {

					var rDoc = mDatabase.GetDocument(q.DocumentId);
					var props = rDoc.Properties;
					Log.Info ("--NEW DATA--","Type: " + rDoc.GetProperty("type").ToString() + " message: " + rDoc.GetProperty("message").ToString() + " Channel: " + rDoc.GetProperty("channels").ToString() + " Created: " + rDoc.GetProperty("created").ToString() );

				}
			};
				
			mLiveQuery.Start ();
		}

		private void startReplication()
		{
			var url = new Uri ("http://192.168.2.28:4984/"+dbName);

			var push = mDatabase.CreatePushReplication (url);
			var pull = mDatabase.CreatePullReplication (url);

			var channel = new List<string> () { "Channel1" };  

			pull.Channels = channel;

			push.Continuous = true;
			pull.Continuous = true;


			push.Start();
			pull.Start();

		}				
	}
}


