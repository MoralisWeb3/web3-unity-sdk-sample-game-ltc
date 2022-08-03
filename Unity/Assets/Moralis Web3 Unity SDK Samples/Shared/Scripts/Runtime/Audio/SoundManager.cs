﻿using System;
using System.Collections.Generic;
using MoralisUnity.Sdk.DesignPatterns.Creational.Singleton.SingletonMonobehaviour;
using UnityEngine;

namespace MoralisUnity.Samples.Shared.Audio
{
	/// <summary>
	/// Maintain a list of AudioSources and play the next 
	/// AudioClip on the first available AudioSource.
	/// </summary>
	public class SoundManager : SingletonMonobehaviour<SoundManager>
	{
		// Properties -------------------------------------
		public List<AudioClip> AudioClips { get { return _soundManagerConfiguration.AudioClips; } }
		
		// Fields -----------------------------------------
		[Header("References (Project)")]
		[SerializeField]
		private SoundManagerConfiguration _soundManagerConfiguration = null;

		// Unity Methods ----------------------------------
		[SerializeField]
		private List<AudioSource> _audioSources = new List<AudioSource>();
		
		// General Methods --------------------------------
		/// <summary>
		/// Play the AudioClip by index.
		/// </summary>
		public void PlayAudioClip(int index)
		{
			AudioClip audioClip = null;
			try
			{
				audioClip = AudioClips[index];
			}
			catch
			{
				throw new ArgumentException($"PlayAudioClip() failed for index = {index}");
			}
			
			PlayAudioClip(audioClip);
		}

		
		/// <summary>
		/// Play the AudioClip by reference.
		/// If all sources are occupied, nothing will play.
		/// </summary>
		public void PlayAudioClip(AudioClip audioClip)
		{
			foreach (AudioSource audioSource in _audioSources)
			{
				if (!audioSource.isPlaying)
				{
					audioSource.clip = audioClip;
					audioSource.Play();
					Debug.Log("play");
					return;
				}
			}
		}
		
		
		// Event Handlers ---------------------------------

	}
}