using System;
using Couchbase.Lite;
using System.Collections.Generic;
using Android.Util;


namespace CBChannelsTest
{
	public class Documents
	{
		private const string docType = "message";
		private const string viewName = "messages";

		public void addContent(Database mDatabase,String mMessage,String mChannel)
		{
			var docProperties = new Dictionary<string,object> {
				{"type",docType},
				{"message",mMessage},
				{"channels",mChannel},
				{"created",DateTime.Now.ToString()}
			};

			var document = mDatabase.CreateDocument ();
			document.PutProperties (docProperties);

			var retrieveDocument = mDatabase.GetDocument (document.Id);
			foreach (var props in retrieveDocument.Properties) 
			{
				Log.Info ("--DOCUMENT EVENT--","Document properties: " + props.Key + " " + props.Value);
			} 		
		}

		public Query getAllContent(Database mDatabase)
		{
			var view = mDatabase.GetView (viewName);

			if (view.Map == null) {

				view.SetMap((document, emitter) =>
					{
						object type;
						document.TryGetValue("type",out type);

						if (Documents.docType.Equals ((string)type)) {
							emitter (document["created"], document);
						}
					}, "2");
			}
			var query = view.CreateQuery ();
			return query;
		}
	}
}

