using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XKPlayerFireAudioCtrl : MonoBehaviour
{
	//转管正常转动音效.
	public AudioSource[] AudioRot;
	//转管结束音效.
	public AudioSource[] AudioRotDown;
	//转管起步音效.
	public AudioSource[] AudioRotUp;
	// Use this for initialization
	void Start ()
	{
		for (int i = 0; i < AudioRot.Length; i++) {
			AudioRot[i].loop = true;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		
	}

	public void SetFireRotAudio(PlayerEnum indexPlayer, bool isPlay)
	{
		int indexVal = (int)indexPlayer - 1;
		if (isPlay && !AudioRot[indexVal].isPlaying) {
			AudioRot[indexVal].Play();
		}

		if (!isPlay && AudioRot[indexVal].isPlaying) {
			AudioRot[indexVal].Stop();
		}
	}
}