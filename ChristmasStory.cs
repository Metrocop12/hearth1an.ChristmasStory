﻿using ChrismasStory.Characters.Travelers;
using ChrismasStory.Components;
using ChrismasStory.Utilities.ModAPIs;
using ChristmasStory.Components;
using ChristmasStory.Components.Animation;
using ChristmasStory.Utility;
using HarmonyLib;
using NewHorizons.Utility;
using OWML.Common;
using OWML.ModHelper;
using System;
using System.Reflection;
using UnityEngine;

namespace ChrismasStory
{
    public class ChristmasStory : ModBehaviour
	{
		public static INewHorizons newHorizonsAPI;
		public static ChristmasStory Instance;

		public static PrisonerBehavior behaviour;


		private void Awake()
		{
			Instance = this;
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		}

		private void Start()
		{
			var newHorizonsAPI = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
			newHorizonsAPI.LoadConfigs(this);
			newHorizonsAPI.GetStarSystemLoadedEvent().AddListener(OnStarSystemLoaded);
			ModHelper.Console.WriteLine($"{nameof(ChristmasStory)} is loaded!", MessageType.Success);
		}

		public static void WriteDebug(string line)
		{
#if DEBUG
			Instance.ModHelper.Console.WriteLine(line, MessageType.Debug);
#endif
		}
		public static void WriteLine(string line) => Instance.ModHelper.Console.WriteLine(line, MessageType.Info);
		public static void WriteError(string line) => Instance.ModHelper.Console.WriteLine(line, MessageType.Error);
		public static void WriteLine(string line, MessageType type) => Instance.ModHelper.Console.WriteLine($"{type}: " + line, type);

		private void OnStarSystemLoaded(string systemName)
		{
			WriteLine("LOADED SYSTEM " + systemName);

			if (systemName == "SolarSystem")
			{
				try
				{
					SpawnOnStart();
				}
				catch (Exception ex)
				{
					WriteError($"{ex}");
				}
			}
		}

		public void SpawnOnStart()
		{
			var player = SearchUtilities.Find("Player_Body");
			player.AddComponent<PlayerEffectController>();
			player.AddComponent<HeldItemHandler>();
			player.AddComponent<SolanumAnimationController>();
			player.AddComponent<PrisonerAnimationController>();

			var ship = SearchUtilities.Find("Ship_Body");
			ship.AddComponent<ShipHandler>();

			// Handles collecting each character
			var characterControllers = new GameObject("ChristmasCharacterControllers");
			characterControllers.AddComponent<ChertCharacterController>();
			characterControllers.AddComponent<EskerCharacterController>();
			characterControllers.AddComponent<FeldsparCharacterController>();
			characterControllers.AddComponent<GabbroCharacterController>();
			characterControllers.AddComponent<RiebeckCharacterController>();
			characterControllers.AddComponent<PlayerNPCCharacterController>();
			characterControllers.AddComponent<SolanumCharacterController>();
			characterControllers.AddComponent<PrisonerCharacterController>();

			if (Conditions.Get(Conditions.PERSISTENT.CHERT_PHRASE_TOLD))
			{
				Conditions.Set(Conditions.PERSISTENT.CHERT_PHRASE_KNOWN_NEXT_LOOP, true);
				WriteLine("Chert phrase known.");
			}
			if (Conditions.Get(Conditions.PERSISTENT.SOLANUM_DONE))
			{
				ModHelper.Console.WriteLine("Solanum event completed.", MessageType.Success);
			}

#if DEBUG
			player.AddComponent<DebugCommands>();
#endif

			Delay.FireOnNextUpdate(AfterSpawn);
			
		}

		private void AfterSpawn()
		{
			TransformThings();
			TravellersReplacements();
			CharactersReplacement();
			GeoRemovements();
		}

		public void GeoRemovements()
		{
			try
			{
				SearchUtilities.Find("TimberHearth_Body/Sector_TH/Interactables_TH/Geysers/Geyser_Village").SetActive(false);
				SearchUtilities.Find("TimberHearth_Body/Sector_TH/Interactables_TH/Geysers/Geyser_TutorialLand").SetActive(false);
				SearchUtilities.Find("TimberHearth_Body/Sector_TH/Geometry_TH/Terrain_TH_Water_v3/Village_Lower_Water").SetActive(false);
				SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Geometry_LowerVillage/OtherComponentsGroup/ControlledByProxy_Structures/Terrain_TH_VillageGeyser").SetActive(false);
				SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Lighting_LowerVillage/OtherComponentsGroup/LowerVillage/GeyserPlatform_Low").SetActive(false);
				SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Lighting_LowerVillage/OtherComponentsGroup/LowerVillage/GeyserPlatform_Mid").SetActive(false);
				SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Interactables_Village/GeyserBoards_Flags").SetActive(false);
				SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Lighting_LowerVillage/OtherComponentsGroup/LowerVillage/Props_HEA_Lantern (11)").SetActive(false);
				SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Volumes_Village/MusicVolume_Village").SetActive(false);
				SearchUtilities.Find("BrittleHollow_Body/Sector_BH/Sector Quantum Pole Path/Fragment QuantumPolePath 5").GetComponent<FragmentIntegrity>().enabled = false;
				SearchUtilities.Find("QuantumMoon_Body/Sector_QuantumMoon/State_EYE/Interactables_EYEState/ConversationPivot/Character_NOM_Solanum/ConversationZone").SetActive(false);
				SearchUtilities.Find("QuantumMoon_Body/Sector_QuantumMoon/State_EYE/Interactables_EYEState/ConversationPivot/Character_NOM_Solanum/WatchZone").SetActive(false);
				SearchUtilities.Find("DB_HubDimension_Body/Sector_HubDimension/Interactables_HubDimension/InnerWarp_ToCluster/Signal_Harmonica").SetActive(false);
			}
			catch (Exception ex)
			{
				WriteError(ex.ToString());
			}
		}

		public void TransformThings()
        {
            try
            {
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Center_Barrel").transform.localScale = new Vector3(7f, 4.5f, 7f);
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Center_Barrel").AddComponent<OWCapsuleCollider>();
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Christmas_Tree").AddComponent<OWCapsuleCollider>();
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Christmas_Tree").GetComponent<CapsuleCollider>().radius = 2f;
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Prefab_HEA_ChertShip").AddComponent<OWCapsuleCollider>();
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Prefab_HEA_ChertShip").GetComponent<CapsuleCollider>().radius = 7f;
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Christmas_Tree").GetComponent<CapsuleCollider>().radius = 2f;
                // SearchUtilities.Find("TimberHearth_Body/Sector_TH/Geometry_GabbroShip").AddComponent<OWCapsuleCollider>();
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Traveller_HEA_Chert_ANIM_Chatter_Chipper").AddComponent<OWCapsuleCollider>();
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Villager_HEA_Esker_ANIM_Rocking").AddComponent<OWCapsuleCollider>();
                SearchUtilities.Find("Ship_Body/ShipSector/Ship_Esker").AddComponent<OWCapsuleCollider>();

                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Traveller_HEA_Feldspar").AddComponent<OWCapsuleCollider>();
                SearchUtilities.Find("TimeLoopRing_Body/Characters_TimeLoopRing/NPC_Player/ConversationZone_NPC_Player").SetActive(false);


                SearchUtilities.Find("DB_AnglerNestDimension_Body/Sector_AnglerNestDimension/Traveller_HEA_Feldspar/ConversationZone").transform.localPosition = new Vector3(0, 0, 0);

                SearchUtilities.Find("Ship_Body/ShipSector/Ship_Feldspar/ConversationZone").GetComponent<InteractReceiver>()._usableInShip = true;
                SearchUtilities.Find("Ship_Body/ShipSector/Ship_Player/ConversationZone").GetComponent<InteractReceiver>()._usableInShip = true;
                SearchUtilities.Find("Ship_Body/ShipSector/Ship_Riebeck/ConversationZone").GetComponent<InteractReceiver>()._usableInShip = true;

                SearchUtilities.Find("Ship_Body/ShipSector/Ship_Esker/ConversationZone").GetComponent<InteractReceiver>()._usableInShip = true;

                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Villager_HEA_Esker_ANIM_Rocking/Signal_Whistling").transform.localPosition = new Vector3(0, 1f, 0);
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Traveller_HEA_Riebeck/ConversationZone").transform.localPosition = new Vector3(0, 1f, 0);

                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Traveller_HEA_Riebeck/Signal_Banjo").transform.localPosition = new Vector3(0, 1f, 0);
                SearchUtilities.Find("Ship_Body/ShipSector/Ship_Feldspar/ConversationZone").transform.localPosition = new Vector3(-0.1f, 0.8f, 0);

                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Traveller_HEA_Feldspar/ConversationZone").transform.localPosition = new Vector3(-0.1f, 0.4f, 0);
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Traveller_HEA_Feldspar/Signal_Harmonica").transform.localPosition = new Vector3(0f, 0f, 0f);

                SearchUtilities.Find("TimberHearth_Body/Sector_TH/GhostBird/Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:ClavicleR/Ghostbird_Skin_01:Ghostbird_Rig_V01:ShoulderR/Ghostbird_Skin_01:Ghostbird_Rig_V01:ElbowR/Ghostbird_Skin_01:Ghostbird_Rig_V01:WristR/Ghostbird_Skin_01:Ghostbird_Rig_V01:HandAttachR/Props_IP_DW_GhostbirdInstrument_Bow").transform.localPosition = new Vector3(0.551f, -0.5451f, 0.2882f);
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/GhostBird/Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:ClavicleR/Ghostbird_Skin_01:Ghostbird_Rig_V01:ShoulderR/Ghostbird_Skin_01:Ghostbird_Rig_V01:ElbowR/Ghostbird_Skin_01:Ghostbird_Rig_V01:WristR/Ghostbird_Skin_01:Ghostbird_Rig_V01:HandAttachR/Props_IP_DW_GhostbirdInstrument_Bow").transform.localRotation = new Quaternion(0.8624f, 0.037f, -0.5042f, 0.0256f);
                // SearchUtilities.Find("Ship_Body/ShipSector/Ship_Riebec/ConversationZone").GetComponent<InteractReceiver>()._usableInShip = true;
                /*
				SearchUtilities.Find("Ship_Body/ShipSector/Ship_Player/ConversationZone").GetComponent<InteractReceiver>()._usableInShip = true;			*/


                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Sector_PrisonCell/Interactibles_PrisonCell/PrisonerSequence/VisionTorchWallSocket/Prefab_IP_VisionTorchItem").GetComponent<OWItem>()._interactable = true;

                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Prefab_IP_DreamLanternItem_2/Props_IP_Artifact/Flame").transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                var artifact = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Prefab_IP_DreamLanternItem_2").GetComponent<DreamLanternController>();
                artifact.enabled = true;
                artifact._lit = true;
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Prefab_IP_DreamLanternItem_2").GetComponent<DreamLanternItem>()._interactable = false;

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Sector_PrisonCell/Ghosts_PrisonCell/GhostNodeMap_PrisonCell_Lower/Prefab_IP_GhostBird_Prisoner/InteractReceiver").SetActive(false);
                SearchUtilities.Find("TimberHearth_Body/Sector_TH/Effects_IP_SarcophagusGlowCenter").transform.localScale = new Vector3(0.4f, 1f, 1f);
                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Sector_PrisonCell/Ghosts_PrisonCell/GhostNodeMap_PrisonCell_Lower/Prefab_IP_GhostBird_Prisoner/Ghostbird_IP_ANIM/Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:Neck01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Neck02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Head/PrisonerHeadDetector").SetActive(false);
                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Sector_PrisonCell/Ghosts_PrisonCell/GhostNodeMap_PrisonCell_Lower/Prefab_IP_GhostBird_Prisoner/Ghostbird_IP_ANIM/Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:Neck01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Neck02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Head/LightSensor_GhostHead").SetActive(false);

				var prisonerArtifact = SearchUtilities.Find("Prisoner_Artifact");
				prisonerArtifact.transform.localPosition = new Vector3(233.4259f, -75.8258f, -144.5425f);
				prisonerArtifact.transform.localRotation = new Quaternion(-0.4581f, 0.7751f, 0.422f, -0.1066f);
				prisonerArtifact.GetComponent<DreamLanternController>().enabled = true;
				prisonerArtifact.GetComponent<DreamLanternController>()._lit = true;


				SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (9)").SetActive(false);
				SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (1)").SetActive(false);
				SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (2)").SetActive(false);
				SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (10)").SetActive(false);
				SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (10)").SetActive(false);
				SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (2)").SetActive(false);
				SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (3)").SetActive(false);

				// SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (14)").transform.localPosition = new Vector3(-5f, -2f, -35f);

				SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (14)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (15)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                // SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (15)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (5)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                // SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (5)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (6)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (6)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (7)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (7)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (6)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (6)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (13)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (13)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (8)").transform.localPosition = new Vector3(-5f, -2f, -35f);
                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (7)").transform.localPosition = new Vector3(-5f, -10f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (8)").transform.localPosition = new Vector3(-5f, -2f, -35f);

                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (5)").transform.localPosition = new Vector3(-5f, -2f, -35f);
                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (14)").transform.localPosition = new Vector3(-5f, -2f, -35f);
                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_L (15)").transform.localPosition = new Vector3(-5f, -2f, -35f);
                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/SarcophagusController/PrisonerFootprints/Decal_DW_Footprint_R (12)").transform.localPosition = new Vector3(-5f, -2f, -35f);




                SearchUtilities.Find("RingWorld_Body/Sector_RingInterior/Sector_Zone4/Sector_PrisonDocks/Sector_PrisonInterior/Interactibles_PrisonInterior/Prefab_IP_Sarcophagus/Prefab_IP_SleepingMummy_v2 (PRISONER)/Mummy_IP_ArtifactAnim").SetActive(false);

                SearchUtilities.Find("Prisoner_Dialogue").SetActive(false);
                SearchUtilities.Find("Prisoner_Clone").SetActive(false);
                SearchUtilities.Find("Prisoner_Vision_Torch").SetActive(false);
                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Prisoner_Vision_Torch/VisionBeam").SetActive(true);
                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Effects_IP_SIM_VisionTorch").SetActive(false);
                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Interactibles_Underground/Prefab_IP_VisionTorchProjector").transform.localPosition = new Vector3(-5f, -2f, -35f);
                SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Sector_PrisonCell/Volumes_PrisonCell/ElevatorLightsOutTrigger").SetActive(false);

                SearchUtilities.Find("Prisoner_Lantern").GetComponent<DreamLanternController>()._lit = true;
                SearchUtilities.Find("Prisoner_Artifact").GetComponent<DreamLanternController>()._lit = true;

                var prisonerDialogue = SearchUtilities.Find("Prisoner_Dialogue");
                var prisonerInteractReciever = SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Sector_PrisonCell/Ghosts_PrisonCell/GhostNodeMap_PrisonCell_Lower/Prefab_IP_GhostBird_Prisoner/InteractReceiver");
                prisonerDialogue.transform.parent = prisonerInteractReciever.transform.parent;
                prisonerDialogue.transform.localPosition = new Vector3(0, 2.92f, 0.369f);

                var prisonerVision = SearchUtilities.Find("Prisoner_Vision");
                prisonerVision.transform.parent = prisonerInteractReciever.transform.parent;
                prisonerVision.transform.localPosition = new Vector3(0, 2.92f, 0.369f);

                var prisonerClone = SearchUtilities.Find("Prisoner_Clone");
                prisonerClone.SetActive(false);
                prisonerClone.AddComponent<CapsuleCollider>();


                var ghostBird = SearchUtilities.Find("TimberHearth_Body/Sector_TH/GhostBird/Ghostbird_Skin_01:Ghostbird_v004:Ghostbird_IP/Ghostbird_Skin_01:Ghostbird_v004:Ghostbird_Merged").GetComponent<SkinnedMeshRenderer>();
                var ghostBirdAntler = SearchUtilities.Find("TimberHearth_Body/Sector_TH/GhostBird/Ghostbird_Skin_01:Ghostbird_v004:Ghostbird_IP/Ghostbird_Skin_01:Ghostbird_v004:Ghostbird_Accessories/Ghostbird_Skin_01:Ghostbird_v004:Antlers_Left/Ghostbird_Skin_01:Ghostbird_v004:Antler_Upward").GetComponent<SkinnedMeshRenderer>();
                var ghostBirdAntlerBroken = SearchUtilities.Find("TimberHearth_Body/Sector_TH/GhostBird/Ghostbird_Skin_01:Ghostbird_v004:Ghostbird_IP/Ghostbird_Skin_01:Ghostbird_v004:Ghostbird_Accessories/Ghostbird_Skin_01:Ghostbird_v004:Antlers_Right/Ghostbird_Skin_01:Ghostbird_v004:Antler_Broken 1").GetComponent<SkinnedMeshRenderer>();
                var ghostBirdInstrument = SearchUtilities.Find("TimberHearth_Body/Sector_TH/GhostBird/Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:ClavicleL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ShoulderL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ElbowL/Ghostbird_Skin_01:Ghostbird_Rig_V01:WristL/Ghostbird_Skin_01:Ghostbird_Rig_V01:HandAttachL/Props_IP_DW_GhostbirdInstrument/Ghostbird_Instrument_geo").GetComponent<MeshRenderer>();
                var ghostBirdInstrumentStand_1 = SearchUtilities.Find("TimberHearth_Body/Sector_TH/GhostBird/Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:ClavicleL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ShoulderL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ElbowL/Ghostbird_Skin_01:Ghostbird_Rig_V01:WristL/Ghostbird_Skin_01:Ghostbird_Rig_V01:HandAttachL/Props_IP_DW_GhostbirdInstrument/ip_instrument_stand/stand_bottom").GetComponent<MeshRenderer>();
                var ghostBirdInstrumentStand_2 = SearchUtilities.Find("TimberHearth_Body/Sector_TH/GhostBird/Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:ClavicleL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ShoulderL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ElbowL/Ghostbird_Skin_01:Ghostbird_Rig_V01:WristL/Ghostbird_Skin_01:Ghostbird_Rig_V01:HandAttachL/Props_IP_DW_GhostbirdInstrument/ip_instrument_stand/stand_middle").GetComponent<MeshRenderer>();
                var ghostBirdInstrumentStand_3 = SearchUtilities.Find("TimberHearth_Body/Sector_TH/GhostBird/Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:ClavicleL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ShoulderL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ElbowL/Ghostbird_Skin_01:Ghostbird_Rig_V01:WristL/Ghostbird_Skin_01:Ghostbird_Rig_V01:HandAttachL/Props_IP_DW_GhostbirdInstrument/ip_instrument_stand/stand_top").GetComponent<MeshRenderer>();
                var ghostBirdInstrumentBow = SearchUtilities.Find("TimberHearth_Body/Sector_TH/GhostBird/Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:ClavicleR/Ghostbird_Skin_01:Ghostbird_Rig_V01:ShoulderR/Ghostbird_Skin_01:Ghostbird_Rig_V01:ElbowR/Ghostbird_Skin_01:Ghostbird_Rig_V01:WristR/Ghostbird_Skin_01:Ghostbird_Rig_V01:HandAttachR/Props_IP_DW_GhostbirdInstrument_Bow").GetComponent<MeshRenderer>();
                var ghostBirdInstrumentMusicBox = SearchUtilities.Find("TimberHearth_Body/Sector_TH/GhostBird/Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:ClavicleL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ShoulderL/Ghostbird_Skin_01:Ghostbird_Rig_V01:ElbowL/Ghostbird_Skin_01:Ghostbird_Rig_V01:WristL/Ghostbird_Skin_01:Ghostbird_Rig_V01:HandAttachL/Props_IP_DW_GhostbirdInstrument/instrument_music_box").GetComponent<MeshRenderer>();
                var simHeadMaterial = SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Prisoner_Clone/Ghostbird_IP_ANIM/Ghostbird_Skin_01:Ghostbird_Rig_V01:Base/Ghostbird_Skin_01:Ghostbird_Rig_V01:Root/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine03/Ghostbird_Skin_01:Ghostbird_Rig_V01:Spine04/Ghostbird_Skin_01:Ghostbird_Rig_V01:Neck01/Ghostbird_Skin_01:Ghostbird_Rig_V01:Neck02/Ghostbird_Skin_01:Ghostbird_Rig_V01:Head/SIM_GhostBirdHead").GetComponent<MeshRenderer>();

                var neededMaterial = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Effects_IP_SarcophagusGlowCenter").GetComponent<MeshRenderer>();

                ghostBird.materials[0].shader = neededMaterial.materials[0].shader;
                ghostBird.materials[1].shader = neededMaterial.materials[0].shader;
                ghostBird.materials[2].shader = simHeadMaterial.materials[0].shader;

                ghostBird.materials[0].CopyPropertiesFromMaterial(neededMaterial.material);
                ghostBird.materials[1].CopyPropertiesFromMaterial(neededMaterial.material);
                ghostBird.materials[2].CopyPropertiesFromMaterial(simHeadMaterial.material);

                ghostBirdAntler.material.shader = neededMaterial.material.shader;
                ghostBirdAntler.materials[0].CopyPropertiesFromMaterial(neededMaterial.material);
                ghostBirdAntlerBroken.material.shader = neededMaterial.material.shader;
                ghostBirdAntlerBroken.materials[0].CopyPropertiesFromMaterial(neededMaterial.material);

				ghostBirdInstrument.materials[0].shader = neededMaterial.materials[0].shader;
				ghostBirdInstrument.materials[1].shader = neededMaterial.materials[0].shader;
				ghostBirdInstrument.materials[2].shader = neededMaterial.materials[0].shader;
				ghostBirdInstrument.materials[3].shader = neededMaterial.materials[0].shader;
				ghostBirdInstrument.materials[4].shader = neededMaterial.materials[0].shader;
				ghostBirdInstrument.materials[5].shader = neededMaterial.materials[0].shader;

				ghostBirdInstrument.materials[0].CopyPropertiesFromMaterial(neededMaterial.material);
				ghostBirdInstrument.materials[1].CopyPropertiesFromMaterial(neededMaterial.material);
				ghostBirdInstrument.materials[2].CopyPropertiesFromMaterial(neededMaterial.material);
				ghostBirdInstrument.materials[3].CopyPropertiesFromMaterial(neededMaterial.material);
				ghostBirdInstrument.materials[4].CopyPropertiesFromMaterial(neededMaterial.material);
				ghostBirdInstrument.materials[5].CopyPropertiesFromMaterial(neededMaterial.material);

				ghostBirdInstrumentStand_1.material.shader = neededMaterial.material.shader;
                ghostBirdInstrumentStand_1.materials = neededMaterial.materials;

                ghostBirdInstrumentStand_2.material.shader = neededMaterial.material.shader;
                ghostBirdInstrumentStand_2.materials = neededMaterial.materials;

                ghostBirdInstrumentStand_3.material.shader = neededMaterial.material.shader;
                ghostBirdInstrumentStand_3.materials = neededMaterial.materials;

				ghostBirdInstrumentMusicBox.materials[0].shader = neededMaterial.materials[0].shader;
				ghostBirdInstrumentMusicBox.materials[1].shader = neededMaterial.materials[0].shader;
				ghostBirdInstrumentMusicBox.materials[0].CopyPropertiesFromMaterial(neededMaterial.material);
                ghostBirdInstrumentMusicBox.materials[1].CopyPropertiesFromMaterial(neededMaterial.material);

				ghostBirdInstrumentBow.materials[0].shader = neededMaterial.materials[0].shader;
				ghostBirdInstrumentBow.materials[1].shader = neededMaterial.materials[0].shader;
				ghostBirdInstrumentBow.materials[2].shader = neededMaterial.materials[0].shader;
				ghostBirdInstrumentBow.materials[3].shader = neededMaterial.materials[0].shader;

				ghostBirdInstrumentBow.materials[0].CopyPropertiesFromMaterial(neededMaterial.material);
                ghostBirdInstrumentBow.materials[1].CopyPropertiesFromMaterial(neededMaterial.material);
				ghostBirdInstrumentBow.materials[2].CopyPropertiesFromMaterial(neededMaterial.material);
				ghostBirdInstrumentBow.materials[3].CopyPropertiesFromMaterial(neededMaterial.material);


				var thTerrain = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Geometry_Village/OtherComponentsGroup/ControlledByProxy_Base/VillageCraterFloors/BatchedGroup/BatchedMeshRenderers_0").GetComponent<MeshRenderer>();
				var otherTerrain = SearchUtilities.Find("QuantumMoon_Body/Sector_QuantumMoon/State_BH/Geometry_BHState/BatchedGroup/BatchedMeshRenderers_3").GetComponent<MeshRenderer>();
				// var otherTerrain = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_NomaiCrater/Geometry_NomaiCrater/OtherComponentsGroup/ControlledByProxy_Base/NomaiCrater/BatchedGroup/BatchedMeshRenderers_1").GetComponent<MeshRenderer>();

				thTerrain.material.shader = otherTerrain.material.shader;
				thTerrain.materials = otherTerrain.materials;

				// ghostBird.materials = neededMaterial.materials;
				//ghostBird.material. = neededMaterial.materials;


				// ShaderReplacer.ReplaceShaders(ghostBird);

				// PrisonerBehavior prisonerBehavior = new PrisonerBehavior();
				var prisonerBehavior = SearchUtilities.Find("DreamWorld_Body/Sector_DreamWorld/Sector_Underground/Sector_PrisonCell/Ghosts_PrisonCell/GhostDirector_Prisoner").GetComponent<PrisonerDirector>();


				ModHelper.Events.Unity.RunWhen(() => prisonerBehavior._prisonerBrain._currentBehavior == PrisonerBehavior.WaitForConversation, () =>
				{
					SearchUtilities.Find("Prisoner_Dialogue").SetActive(true);
					
				});
			}

			catch (Exception ex)
			{
				WriteError(ex.ToString());
			}

		}

		public void CharactersReplacement()
		{
			try
			{
				// Marl
				var Marl_Character = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Villager_HEA_Marl");
				Marl_Character.transform.localPosition = new Vector3(8.3747f, 7.4018f, -8.3346f);
				Marl_Character.transform.localRotation = new Quaternion(-0.02323f, -0.8668f, 0.0022f, 0.4982f);

				var Marl_Look = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Villager_HEA_Marl/Villager_HEA_Marl_ANIM_StareDwn").GetComponent<CharacterAnimController>();
				Marl_Look.lookOnlyWhenTalking = false;
				Marl_Look._currentLookTarget = new Vector3(8.96f, -6.2049f, 186.7599f);
				// Marl_Look._animator.SetLookAtPosition();
				
				

				// Tephra
				var Tephra_Character = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Kids_PreGame/Villager_HEA_Tephra");
				Tephra_Character.transform.localPosition = new Vector3(-5.9785f, 8.7614f, -1.742f);
				Tephra_Character.transform.localRotation = new Quaternion(0.0245f, 0.5553f, 0.0357f, 0.8305f);

				// Galena
				var Galena_Character = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Sector_Village/Sector_LowerVillage/Characters_LowerVillage/Kids_PreGame/Villager_HEA_Galena");
				Galena_Character.transform.localPosition = new Vector3(1.2199f, 7.7457f, -2.38f);
			}
			catch (Exception ex)
			{
				WriteError(ex.ToString());
			}
		}

		public void TravellersReplacements()
		{
			try
			{
				// Feldspar
				SearchUtilities.Find("DB_PioneerDimension_Body/Sector_PioneerDimension/Interactables_PioneerDimension/Pioneer_Characters").SetActive(false);

				// Riebec
				SearchUtilities.Find("BrittleHollow_Body/Sector_BH/Sector_Crossroads/Characters_Crossroads/Traveller_HEA_Riebeck").SetActive(false);
				SearchUtilities.Find("BrittleHollow_Body/Sector_BH/Sector_Crossroads/Characters_Crossroads/Signal_Banjo").SetActive(false);

				// Esker
				SearchUtilities.Find("Moon_Body/Sector_THM/Characters_THM/Villager_HEA_Esker/ConversationZone_Esker").SetActive(false);

				// Chert
				SearchUtilities.Find("CaveTwin_Body/Sector_CaveTwin/Sector_NorthHemisphere/Sector_NorthSurface/Sector_Lakebed/Interactables_Lakebed/Traveller_HEA_Chert/ConversationZone_Chert").SetActive(false);

				// Gabbro
				SearchUtilities.Find("GabbroIsland_Body/Sector_GabbroIsland/Interactables_GabbroIsland/Traveller_HEA_Gabbro/ConversationZone_Gabbro").SetActive(false);
			}
			catch (Exception ex)
			{
				WriteError(ex.ToString());
			}
		}



		public void Ernesto_Start()
		{
			/* 
            Slate will ask to check if everyone is ready to call hornfels and start, Chert would ask if I want to invite Owlks or Solanum so player can go for it or skip and talk to Hornfels instead.

            Player goes to Hornfels, telling about celebration > closing your eyes > Hornfels appears near the Christmas tree and asks to put something shiny on the top of the tree and we can search for something like this in the observatory >
            There will be Ernesto dialogue and he will shine very BRIGHT! > player asks him for help > closing eyes > appears on the top of the tree. 

            */
            }


        public void Optional_Prisoner_Start()
		{
			/* 
            Player will need to go to the stranger, do the EoTE ending but players vision will be different, about inviting him to the christmas > he goes away like always and his vision torch will give player new vision about how player can bring him to the TH >
            He will show that in his prison IRL there will be an artifact that will make his proection when dropped. > Player will bring it to the TH > script will need to check if artifact on timber hearth to complete the quest.

            */
		}

		public void Update()
		{

		}
	}
}