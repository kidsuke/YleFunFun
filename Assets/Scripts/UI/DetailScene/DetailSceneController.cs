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
	private Sprite m_DefaultSprite;
	public Sprite defaultSprite { get { return m_DefaultSprite; } set { m_DefaultSprite = value; } }
	[SerializeField]
	private Text m_Title;
	public Text tile { get { return m_Title; } set { m_Title = value; } }
	[SerializeField]
	private Text m_Description;
	public Text description { get { return m_Description; } set { m_Description = value; } }

	private YleAPI m_API;
	private Program m_Program;

	void Awake () {
		m_API = new YleAPI();
		m_Program = SceneTransitionData.currentProgram;
	}

	// Use this for initialization
	void Start () {
		GetCoverImage();
		m_Title.text = m_Program.title;
		m_Description.text = m_Program.description;
	}

	// Get a cover image from Yle
	public void GetCoverImage() {
		string imageId = m_Program.imageId;
		Observable.FromCoroutine<Texture>((observer, cancellationToken) => m_API.GetCoverImage(observer, cancellationToken, imageId))
				  .Subscribe(
						texture => {
							if (texture != null) {
								Texture2D texture2D = texture as Texture2D;
								Sprite coverSprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
								m_CoverImage.sprite = coverSprite;
							} else {
								loadDefaultErrorImage();
							}
						}, 
						error => {
							loadDefaultErrorImage();
						})
				  .AddTo(this);
	}

	private void loadDefaultErrorImage() {
		m_CoverImage.sprite = defaultSprite;
		m_CoverImage.type = Image.Type.Simple;
		m_CoverImage.preserveAspect = true;
	}
}
