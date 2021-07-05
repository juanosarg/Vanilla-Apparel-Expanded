using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using Verse;
using RimWorld;

namespace AchievementsExpanded
{
    public class ItemTrackerMultipleVanillaApparel : ItemTracker
    {


        public ItemTrackerMultipleVanillaApparel()
        {
        }

        public ItemTrackerMultipleVanillaApparel(ItemTrackerMultipleVanillaApparel reference) : base(reference)
        {
            thingList = reference.thingList;
            playerHasIt = reference.playerHasIt;
            foreach (KeyValuePair<ThingDef, int> set in thingList)
            {
                playerHasIt.Add(set.Key,false);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref thingList, "thingList", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look(ref playerHasIt, "playerHasIt", LookMode.Def, LookMode.Value);
        }

        public override bool Trigger()
        {
            bool playerHasThemAll = true;
            foreach (KeyValuePair<ThingDef, int> set in thingList)
            {
                playerHasIt[set.Key] =  UtilityMethods.PlayerHas(set.Key, out int total, set.Value);               
            }
            foreach (KeyValuePair<ThingDef, bool> set in playerHasIt)
            {
                playerHasThemAll = playerHasThemAll && playerHasIt[set.Key];
            }
            return playerHasThemAll;
        }

        Dictionary<ThingDef, int> thingList = new Dictionary<ThingDef, int>();
        Dictionary<ThingDef,bool> playerHasIt = new Dictionary<ThingDef, bool>();
    }
}
