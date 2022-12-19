﻿using NewHorizons.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChrismasStory.Characters.Travelers
{
    internal class EskerCharacterController : TravelerCharacterController
	{
        /* Visit Esker > He will say that he already knows everything bc he is listening to signalscope (he will be weirdo like always) >
		close eyes > he will appear in your ship > track if we are on Timber Hearth > talk to him > closing eyes > he will appear on TH always.
		*/

        public override void Start()
        {
            // dialogue =
            originalCharacter = SearchUtilities.Find("Moon_Body/Sector_THM/Characters_THM/Villager_HEA_Esker/Villager_HEA_Esker_ANIM_Rocking");
            shipCharacter = SearchUtilities.Find("Ship_Body/ShipSector/Ship_Esker");
            treeCharacter = SearchUtilities.Find("TimberHearth_Body/Sector_TH/Villager_HEA_Esker_ANIM_Rocking");

            base.Start();

            ChangeState(STATE.ORIGINAL);
        }

		protected override void Dialogue_OnStartConversation()
		{

		}

		protected override void Dialogue_OnEndConversation()
		{
			ChangeState(STATE.ON_SHIP);
		}
	}
}
