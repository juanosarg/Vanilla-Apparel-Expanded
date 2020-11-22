using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;

namespace AchievementsExpanded
{
    public class ItemCraftTrackerMultipleVanillaApparel : Tracker<Thing>
    {
        public override string Key => "ItemCraftTrackerMultipleVanillaApparel";

        public override MethodInfo MethodHook => AccessTools.Method(typeof(QuestManager), nameof(QuestManager.Notify_ThingsProduced));
        public override MethodInfo PatchMethod => AccessTools.Method(typeof(VanillaApparel_QuestManager_Notify_ThingsProduced_Patch),
            nameof(VanillaApparel_QuestManager_Notify_ThingsProduced_Patch.CheckItemCraftedMultiple));
        protected override string[] DebugText => new string[] { 
                                                                $"MadeFrom: {madeFrom?.defName ?? "Any"}",
                                                                $"Quality: {quality}"
                                                                 };
        public ItemCraftTrackerMultipleVanillaApparel()
        {
        }

        public ItemCraftTrackerMultipleVanillaApparel(ItemCraftTrackerMultipleVanillaApparel reference) : base(reference)
        {
            thingList = reference.thingList;
            madeFrom = reference.madeFrom;
            quality = reference.quality;
            playerCraftedIt = reference.playerCraftedIt;
            foreach (KeyValuePair<ThingDef, int> set in thingList)
            {
                playerCraftedIt.Add(set.Key, 0);
            }

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref thingList, "thingList", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look(ref playerCraftedIt, "playerHasIt", LookMode.Def, LookMode.Value);

            Scribe_Defs.Look(ref madeFrom, "madeFrom");
            Scribe_Values.Look(ref quality, "quality");
           
        }

        //public override (float percent, string text) PercentComplete => count > 1 ? ((float)triggeredCount / count, $"{triggeredCount} / {count}") : base.PercentComplete;

        public override bool Trigger(Thing thing)
        {
            base.Trigger(thing);
            bool playerCraftedThemAll = true;

            foreach (KeyValuePair<ThingDef, int> set in thingList)
            {
                if ((set.Key is null || thing.def == set.Key) && (madeFrom is null || madeFrom == thing.Stuff))
                {
                    if (quality is null || (thing.TryGetQuality(out var qc) && qc >= quality))
                    {
                        playerCraftedIt[set.Key]++;
                    }
                }
            }

            foreach (KeyValuePair<ThingDef, int> set in playerCraftedIt)
            {
                playerCraftedThemAll = playerCraftedThemAll&&(playerCraftedIt[set.Key]>= thingList[set.Key]);
            }

            return playerCraftedThemAll;
        }

        Dictionary<ThingDef, int> thingList = new Dictionary<ThingDef, int>();
        Dictionary<ThingDef, int> playerCraftedIt = new Dictionary<ThingDef, int>();
        public ThingDef madeFrom;
        public QualityCategory? quality;
       
    }
}
