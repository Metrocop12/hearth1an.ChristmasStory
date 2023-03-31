﻿using HarmonyLib;
using NewHorizons.Utility;
using OWML.ModHelper;
using System;
using System.Linq;
using UnityEngine;
using ChristmasStory.Utility;
using System.Collections;

namespace ChristmasStory.Components
{

	internal class EndGameController : MonoBehaviour
	{

		public static EndGameController Instance;

		public float maxErnestoLight = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Ernesto/B_angler_root/B_angler_body01/B_angler_body02/B_angler_antenna01/B_angler_antenna02/B_angler_antenna03/B_angler_antenna04/B_angler_antenna05/B_angler_antenna06/B_angler_antenna07/B_angler_antenna08/B_angler_antenna09/B_angler_antenna10/B_angler_antenna11/B_angler_antenna12_end/Props_HEA_WallLamp_Pulsing 1/Ernesto_Light").GetComponent<PulsingLight>()._initLightRange = 300f;

		public void Start()
		{
			Instance = this;

		}

		public void StartErnestoShine()
		{
			PlayerEffectController.PlayAudioOneShot(AudioType.PlayerGasp_Light, 1f);
			StartCoroutine(IncreaseLightLevel());
			EndGameEvent();
			Locator.GetPauseCommandListener().AddPauseCommandLock();
		}

		public IEnumerator IncreaseLightLevel()
		{
			WriteUtil.WriteLine("Coroutine is running");
			int maxLight = 300;
			var ernestoLight = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Ernesto/B_angler_root/B_angler_body01/B_angler_body02/B_angler_antenna01/B_angler_antenna02/B_angler_antenna03/B_angler_antenna04/B_angler_antenna05/B_angler_antenna06/B_angler_antenna07/B_angler_antenna08/B_angler_antenna09/B_angler_antenna10/B_angler_antenna11/B_angler_antenna12_end/Props_HEA_WallLamp_Pulsing 1/Ernesto_Light").GetComponent<PulsingLight>();

			for (int i = 0; i < maxLight; i++)
			{
				WriteUtil.WriteLine("Coroutine started, light level is" + ernestoLight._initLightRange);
				ernestoLight._initLightRange += 2f;

				yield return new WaitForSeconds(0.3f);
			}
		}

		public void EndGameEvent()
		{
			Invoke("EndingTrigger", 52f);
			Invoke("BlowUpSun", 45f);

			if (Conditions.Get(Conditions.PERSISTENT.ALL_TRAVELLERS_DONE) && Conditions.Get(Conditions.PERSISTENT.SOLANUM_DONE) && Conditions.Get(Conditions.PERSISTENT.PRISONER_DONE))
			{
				SearchUtilities.Find("music_all").SetActive(true);
				SearchUtilities.Find("music_all").transform.localPosition = new Vector3(0, 1, 0);
			}
			else if (Conditions.Get(Conditions.PERSISTENT.ALL_TRAVELLERS_DONE) && !Conditions.Get(Conditions.PERSISTENT.SOLANUM_DONE) && Conditions.Get(Conditions.PERSISTENT.PRISONER_DONE))
			{
				SearchUtilities.Find("music_no_sol").SetActive(true);
				SearchUtilities.Find("music_no_sol").transform.localPosition = new Vector3(0, 1, 0);
			}
			else if (Conditions.Get(Conditions.PERSISTENT.ALL_TRAVELLERS_DONE) && Conditions.Get(Conditions.PERSISTENT.SOLANUM_DONE) && !Conditions.Get(Conditions.PERSISTENT.PRISONER_DONE))
			{
				SearchUtilities.Find("music_no_bird").SetActive(true);
				SearchUtilities.Find("music_no_bird").transform.localPosition = new Vector3(0, 1, 0);
			}
			else if (Conditions.Get(Conditions.PERSISTENT.ALL_TRAVELLERS_DONE) && !Conditions.Get(Conditions.PERSISTENT.SOLANUM_DONE) && !Conditions.Get(Conditions.PERSISTENT.PRISONER_DONE))
			{
				SearchUtilities.Find("music_no_sol_no_bird").SetActive(true);
				SearchUtilities.Find("music_no_sol_no_bird").transform.localPosition = new Vector3(0, 1, 0);
			}
		}
		public void EndingTrigger()
		{
			PlayerEffectController.Blink(3);
			SearchUtilities.Find("Ending_Trigger").transform.localPosition = new Vector3(0, 0, 0);
			SearchUtilities.Find("Ending_Trigger").SetActive(true);
			PlayerEffectController.OpenEyes(3);
			/*
            SearchUtilities.Find("music_no_sol_no_bird").SetActive(false);
            SearchUtilities.Find("music_no_bird").SetActive(false);            
            SearchUtilities.Find("music_no_sol").SetActive(false);
            SearchUtilities.Find("music_all").SetActive(false);
            */
		}

		public void BlowUpSun()
		{
			SearchUtilities.Find("Sun_Body/Sector_SUN/Effects_SUN/Supernova").GetComponent<SupernovaEffectController>().enabled = true;
			ChristmasStory.Instance.ModHelper.Console.WriteLine("Starting supernova");
		}

		public void DisableProps()
		{

		}



	}
}
