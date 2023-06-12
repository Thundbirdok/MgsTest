using System;
using UnityEngine;
using UnityEngine.UI;

namespace VLCUnity.Demos.Scripts
{
	///This script controls all the GUI for the VLC Unity Canvas Example
	///It sets up event handlers and updates the GUI every frame
	///This example shows how to safely set up LibVLC events and a simple way to call Unity functions from them
	/// On Android, make sure you require Internet access in your manifest to be able to access internet-hosted videos in these demo scenes.
	public class VlcPlayerGui : MonoBehaviour
	{
		public VLCPlayer vlcPlayer;
	
		//GUI Elements
		public AspectRatioFitter screenAspectRatioFitter;
		public Button playButton;
		public Slider volumeBar;
		
		//Configurable Options
		public int maxVolume = 100; //The highest volume the slider can reach. 100 is usually good but you can go higher.

		//State variables
		private bool _isPlaying; //We use VLC events to track whether we are playing, rather than relying on IsPlaying 

		private void Start()
		{
			//VLC Event Handlers
			vlcPlayer.mediaPlayer.Playing += (_, _) => 
			{
				//Always use Try/Catch for VLC Events
				try
				{
					//Because many Unity functions can only be used on the main thread, they will fail in VLC event handlers
					//A simple way around this is to set flag variables which cause functions to be called on the next Update
					_isPlaying = true; //Switch to the Pause button next update
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception caught in mediaPlayer.Play: \n" + ex);
				}
			};

			vlcPlayer.mediaPlayer.Paused += (_, _) => 
			{
				//Always use Try/Catch for VLC Events
				try
				{
					_isPlaying = false; //Switch to the Play button next update
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception caught in mediaPlayer.Paused: \n" + ex);
				}
			};

			vlcPlayer.mediaPlayer.Stopped += (_, _) => 
			{
				//Always use Try/Catch for VLC Events
				try
				{
					_isPlaying = false; //Switch to the Play button next update
				}
				catch (Exception ex)
				{
					Debug.LogError("Exception caught in mediaPlayer.Stopped: \n" + ex);
				}
			};

			//Buttons
			playButton.onClick.AddListener(Play);

			//Volume Bar
			volumeBar.wholeNumbers = true;
			volumeBar.maxValue = maxVolume; //You can go higher than 100 but you risk audio clipping
			volumeBar.value = vlcPlayer.Volume;
			
			volumeBar.onValueChanged.AddListener
			(
				_ => { vlcPlayer.SetVolume((int)volumeBar.value); }
			);
			
			volumeBar.gameObject.SetActive(true);
		}

		private void Update()
		{
			if (vlcPlayer.texture != null)
			{
				screenAspectRatioFitter.aspectRatio =
					(float)vlcPlayer.texture.width / vlcPlayer.texture.height;
			}
		}

		private void Play()
		{
			if (_isPlaying == false)
			{
				vlcPlayer.Open();
			}
		}
	}
}