using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class YleResponse {
	public DataComponent[] data;

	/* Yle API returns response in form of json:
		{
		 	"apiVersion":"",
		 	"meta" : {},
		 	"data" : []
		}
	   Therefore, the response is deserialized as follow:
	   		apiVersion -> ApiVersionComponent (not require yet)
	 		meta -> MetaComponent (not require yet)
	 		data -> DataComponent
	*/
	[System.Serializable]
	public class DataComponent {
		public Description desciption;
		public Video video;
		public string typeMedia;
		public string[] creator;
		public string type;
		public Title title;
		public Title itemTitle;
		public string[] countryOfOrigin;
		public string id;
		public string typeCreative;
		public Image image;
	};

	/* These are the deserialized children of DataComponent */
	[System.Serializable]
	public class PartOfSeries {
		public Description description;
		public string[] creator;
		public string[] alternativeId;
		public string type;
	}

	[System.Serializable]
	public class Description {
		public string fi;
	}

	[System.Serializable]
	public class Video {
		public string[] language;
		public string[] format;
		public string type;
	}

	[System.Serializable]
	public class Title {
		public string fi;
		public string sv;
		public string und;
	}

	[System.Serializable]
	public class Image {
		public string id;
		public bool available;
	}

}
