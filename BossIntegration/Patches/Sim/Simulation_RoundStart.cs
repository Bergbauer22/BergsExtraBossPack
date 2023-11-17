using BossIntegration.UI;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Simulation;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BossIntegration.Patches.Sim;

[HarmonyPatch(typeof(Simulation), nameof(Simulation.RoundStart))]
internal class Simulation_RoundStart
{
    [HarmonyPostfix]
    internal static void Postfix()
    {
        try
        {
            if (ModBoss.Cache.Count > 0)
            {
                var currentRound = Il2CppAssets.Scripts.Unity.UI_New.InGame.InGame.instance.GetUnityToSimulation().GetCurrentRound() + 1;
                ModBossUI.UpdateWaitPanel(currentRound);

                IEnumerable<KeyValuePair<string, ModBoss>> currentBosses = ModBoss.Cache.Where(x => x.Value.SpawnRounds.Contains(currentRound));
                foreach (var (_, boss) in currentBosses)
                {
                    if (ModBoss.GetPermission(boss, currentRound))
                    {
                        BloonModel bossModel = boss.ModifyForRound(Game.instance.model.GetBloon(boss.Id), currentRound);
                        Bloon bossBloon = Il2CppAssets.Scripts.Unity.UI_New.InGame.InGame.instance.GetMap().spawner.Emit(bossModel, 0, 0);
                        bossBloon.bloonModel.speedFrames = ModBoss.SpeedToSpeedFrames(bossModel.speed);
                        boss.OnSpawnMandatory(bossBloon, boss);
                    }
                }
            }
        }
        catch (Exception e)
        {
            ModHelper.Error<BossIntegration>(e);
        }
    }
}