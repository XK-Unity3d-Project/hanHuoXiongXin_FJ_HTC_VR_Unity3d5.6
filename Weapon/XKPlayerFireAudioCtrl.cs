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
		CheckFireRotUpAudio();
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

	public void SetFireRotDownAudio(PlayerEnum indexPlayer, bool isPlay)
	{
		int indexVal = (int)indexPlayer - 1;
		if (isPlay && !AudioRotDown[indexVal].isPlaying) {
			AudioRotDown[indexVal].Play();
		}

		if (!isPlay && AudioRotDown[indexVal].isPlaying) {
			AudioRotDown[indexVal].Stop();
		}
	}

	public void SetFireRotUpAudio(PlayerEnum indexPlayer, bool isPlay)
	{
		if (isPlay) {
			switch (indexPlayer) {
			case PlayerEnum.PlayerOne:
				if (!XkGameCtrl.IsActivePlayerOne) {
					return;
				}
				break;
			case PlayerEnum.PlayerTwo:
				if (!XkGameCtrl.IsActivePlayerTwo) {
					return;
				}
				break;
			}
		}

		int indexVal = (int)indexPlayer - 1;
		if (isPlay && !AudioRotUp[indexVal].isPlaying) {
			AudioRotUp[indexVal].Play();
			IsPlayerRotUp[indexVal] = true;
			TimeRotUp[indexVal] = Time.time;
		}

		if (!isPlay && AudioRotUp[indexVal].isPlaying) {
			AudioRotUp[indexVal].Stop();
			IsPlayerRotUp[indexVal] = false;
			//结束转管启动音效,但是不发射子弹.
		}
	}

	public static bool[] IsPlayerRotUp = new bool[2];
	float[] TimeRotUp = new float[2];
	void CheckFireRotUpAudio()
	{
		for (int i = 0; i < AudioRotUp.Length; i++) {
			if (IsPlayerRotUp[i]) {
				if (Time.time - TimeRotUp[i] >= AudioRotUp[i].clip.length || !AudioRotUp[i].isPlaying) {
					//转管启动音效结束,开始发射子弹.
					IsPlayerRotUp[i] = false;
				}
			}
		}
	}
}