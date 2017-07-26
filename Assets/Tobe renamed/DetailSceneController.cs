using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DetailSceneController : MonoBehaviour {
	[SerializeField]
	private Image m_CoverImage;
	public Image coverImage { get { return m_CoverImage; } set { m_CoverImage = value; } }
	[SerializeField]
	private Text m_Title;
	public Text tile { get { return m_Title; } set { m_Title = value; } }
	[SerializeField]
	private Text m_Description;
	public Text description { get { return m_Description; } set { m_Description = value; } }


	private YleAPI m_API = new YleAPI();
	private Item m_Item = SceneTransitionData.currentItem;

	// Use this for initialization
	void Start () {
		GetCoverImage();
		m_Title.text = m_Item.title;
		m_Description.text = m_Item.description;
	}

	void GetCoverImage() {
		string imageId = m_Item.imageId;
		Observable.FromCoroutine<Texture>((observer, cancellationToken) => m_API.GetCoverImage(observer, cancellationToken, imageId))
				  .Subscribe(texture => {
					  Texture2D texture2D = texture as Texture2D;
					  Sprite coverSprite = Sprite.Create(texture2D, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
					  m_CoverImage.sprite = coverSprite;
				  });
	}
}
