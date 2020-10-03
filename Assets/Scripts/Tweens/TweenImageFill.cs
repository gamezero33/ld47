using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenImageFill : Tweener
{

	[SerializeField, Range( 0, 1 )] private float m_StartFill;
	[SerializeField, Range( 0, 1 )] private float m_EndFill;

	private Image m_Image;

	private void Awake ()
	{
		m_RandomTime = Random.Range( 0.0f, m_Duration );
	}

	public float GetValue ()
	{
		return Mathf.LerpUnclamped( m_StartFill, m_EndFill, 1f / m_Duration * m_RandomTime );
	}

	protected override void UpdateTween ()
	{
		if ( !m_Image ) m_Image = GetComponent<Image>();
		if ( m_Image )  m_Image.fillAmount = Mathf.LerpUnclamped( m_StartFill, m_EndFill, Factor );
	}

}
