using BergsExtraBossPack;
using BossIntegration;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.SMath;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppFacepunch.Steamworks;
using System.Collections.Generic;
using UnityEngine;
using static BergsExtraBossPack.BergsExtraBossPackMOD;
using static Il2CppAssets.Scripts.Models.Bloons.Behaviors.StunTowersInRadiusActionModel;

namespace BossPackReborn.Bosses;

internal class BadorBoss : ModBoss
{
    protected override int Order => 1;
    public override string DisplayName => "Bador the Rocket";
    public override string Icon => "Bador-Icon";
    public override string Description => "\"Have fun changing your Game completly\"";
    public override IEnumerable<string> DamageStates => new string[] { };
    public class BadorDisplay : ModBloonDisplay<BadorBoss>
    {
        public override string BaseDisplay => ModDisplay.Generic2dDisplay;
        public BadorDisplay() { }

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "Bador");
        }
    }
    public override string TimerDescription => "Moves around a random tower.";
    public override void TimerTick(Bloon boss)
    {
        List<Tower> towers = InGame.instance.GetTowers();

        if (towers.Count < 1)
            return; // Penality

        uint? tier = ModBoss.GetTier(boss);

        float amount = 1f;
        //Special
        boss.health = boss.health + boss.bloonModel.maxHealth * 0.0003f + 1500 > boss.bloonModel.maxHealth ?
        boss.bloonModel.maxHealth :
        boss.health + boss.bloonModel.maxHealth * 0.001f;
        //SpecialEnd
        if (tier != null)
            amount = (float)tier * 5;
        for (int i = 0; i < towers.Count; i++)
        {
            float angle = BossPack.rng.Next(0, 360) * Math.PI / 180;
            towers[BossPack.rng.Next(0, towers.Count)].MoveTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(Math.Cos(angle) * amount, Math.Sin(angle) * amount));
        }
        ModHelper.Msg<BergsExtraBossPackMOD>(tier);
        if (tier == 1)
        {
            InGame.instance.SpawnBloons("Moab", 1, 10);
        }
        else if (tier == 2)
        {
            InGame.instance.SpawnBloons("MoabFortified", 4, 50);
        }
        else if (tier == 3)
        {
            InGame.instance.SpawnBloons("BfbFortified", 2, 50);
            InGame.instance.SpawnBloons("MoabFortified", 4, 50);
        }
        else if (tier == 4)
        {
            InGame.instance.SpawnBloons("DdtFortified", 1, 50);
            InGame.instance.SpawnBloons("MoabFortified", 1, 50);

        }
        else if (tier == 5)
        {

            InGame.instance.SpawnBloons("DdtFortified", 1, 50);
            InGame.instance.SpawnBloons("MoabFortified", 2, 50);
            InGame.instance.SpawnBloons("BfbFortified", 1, 50);
        }

    }
    public override string SkullDescription => "Makes a Dash | Switch the positions of two Towers / Stun your most expensive Tower that isn't already stuned(invisible for your)for a short amount of Time(Paragons less)";
    public override void SkullEffect(Bloon boss)
    {
        uint? tier = ModBoss.GetTier(boss);
        if(tier != null)
        {
            if(tier == 1)
            {
                InGame.instance.SpawnBloons("MoabFortified", 5, 50);
            }
            else if(tier == 2)
            {
                InGame.instance.SpawnBloons("BfbFortified", 5, 50);
                InGame.instance.SpawnBloons("MoabFortified", 10, 50);
            }
            else if (tier == 3)
            {
                InGame.instance.SpawnBloons("DdtFortified", 5, 50);
                InGame.instance.SpawnBloons("BfbFortified", 10, 50);
                InGame.instance.SpawnBloons("MoabFortified", 15, 50);
            }
            else if (tier == 4)
            {
                InGame.instance.SpawnBloons("ZomgFortified", 5, 50);
                InGame.instance.SpawnBloons("DdtFortified", 10, 50);
                InGame.instance.SpawnBloons("BfbFortified", 15, 50);
                InGame.instance.SpawnBloons("MoabFortified", 20, 50);
            }
            else if (tier == 5)
            {
                InGame.instance.SpawnBloons("ZomgFortified", 10, 50);
                InGame.instance.SpawnBloons("DdtFortified", 15, 50);
                InGame.instance.SpawnBloons("BfbFortified", 20, 50);
                InGame.instance.SpawnBloons("MoabFortified", 25, 50);
                InGame.instance.SpawnBloons("BadFortified", 3, 50);
            }
        }


        //Sound
        int x = BossPack.rng.Next(1, 7);
        if (x == 1)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("WasTueIchFürTikTok").Play();
        }
        else if (x == 2)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("VollDumm").Play();
        }
        else if (x == 3)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("höö").Play();
        }
        else if (x == 4)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("DenKennIchAuchDenBoy").Play();
        }
        else if (x == 5)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("DasMachtKeinenSinn").Play();
        }
        else if (x == 6)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("AbnniertPaluten").Play();
        }
        else if (x == 7)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("AbnniertPaluten").Play();
        }
        boss.AddMutator(new SpeedUpMutator("BadorDash", 6f), 30);
        List<Tower> towers = InGame.instance.GetTowers();
        TowerFreezeMutator stun = new TowerFreezeMutator(new Il2CppAssets.Scripts.Utils.PrefabReference(), true);
        List<Tower> towers2 = InGame.instance.GetTowerManager().GetTowers().ToList();
        if (towers2.Count == 0)
            return;
        for (int i = towers2.Count - 1; i >= 0; i--)
            if (towers2[i].IsStunned)
                towers2.RemoveAt(i);
        if (towers2.Count == 0)
            return;

        float highestWorth = 0;
        int index = 0;
        for (int i = towers2.Count - 1; i >= 0; i--)
        {
            if (towers2[i].worth > highestWorth)
            {
                highestWorth = towers[i].worth;
                index = i;
            }
        }

        if (towers2[index].towerModel.isParagon)
            towers2[index].AddMutator(stun, 2400);
        else
            towers2[index].AddMutator(stun, 3000);
        //Other
        if (towers.Count < 2)
        {
            return; // Penality
        }

        do
        {
            Tower rdm1 = towers[BossPack.rng.Next(0, towers.Count)];
            towers.Remove(rdm1);

            Tower rdm2 = towers[BossPack.rng.Next(0, towers.Count)];
            towers.Remove(rdm2);

            Il2CppAssets.Scripts.Simulation.SMath.Vector2 rdm1Pos = rdm1.Position.ToVector2();
            rdm1.PositionTower(rdm2.Position.ToVector2());
            rdm2.PositionTower(rdm1Pos);

        } while (towers.Count > 2);
    }
    public override Dictionary<int, BossRoundInfo> RoundsInfo => new Dictionary<int, BossRoundInfo>()
    {
        [40] = new BossRoundInfo()
        {
            skullCount = 2,
            interval = 6,
            tier = 1,
            timerDescription = "He moves your Towers away from there actual position(He can move them a bit out of the Map) / Regenerate his life a bit / Summons Moabs",
        },
        [60] = new BossRoundInfo()
        {
            skullCount = 4,
            interval = 5,
            tier = 2,
            timerDescription = "He moves your Towers away from there actual position(He can move them a bit out of the Map) / Regenerate his life a bit / Summons FortifiedMoabs",
        },
        [80] = new BossRoundInfo()
        {
            skullCount = 6,
            interval = 4,
            tier = 3,
            timerDescription = "He moves your Towers away from there actual position(He can move them a bit out of the Map) / Regenerate his life a bit / Summons FortifiedBfbs",
        },
        [100] = new BossRoundInfo()
        {
            skullCount = 8,
            interval = 3,
            tier = 4,
            timerDescription = "He moves your Towers away from there actual position(He can move them a bit out of the Map) / Regenerate his life a bit / Summons FortifiedDdts",
        },
        [120] = new BossRoundInfo()
        {
            skullCount = 10,
            interval = 1,
            tier = 5,
            timerDescription = "He moves your Towers away from there actual position(He can move them a bit out of the Map) / Regenerate his life a bit / Summons FortifiedDdts/Bfbs/Moabs",
        },
    };
    public override BloonModel ModifyForRound(BloonModel bloon, int round)
    {
        if (RoundsInfo.TryGetValue(round, out var roundInfo))
        {
            switch (roundInfo.tier)
            {

                case 1:
                    bloon.maxHealth = 25_000;
                    bloon.speed = 5;
                    ModContent.GetAudioClip<BergsExtraBossPackMOD>("Behrüßung2").Play();
                    break;
                case 2:
                    bloon.maxHealth = 150_000;
                    bloon.speed = 5.2f;
                    ModContent.GetAudioClip<BergsExtraBossPackMOD>("Behrüßung").Play();
                    break;
                case 3:
                    bloon.maxHealth = 500_000;
                    bloon.speed = 5.4f;
                    ModContent.GetAudioClip<BergsExtraBossPackMOD>("Behrüßung2").Play();
                    break;
                case 4:
                    bloon.maxHealth = 2_000_000;
                    bloon.speed = 5.6f;
                    ModContent.GetAudioClip<BergsExtraBossPackMOD>("Behrüßung").Play();
                    break;
                case 5:
                    bloon.maxHealth = 10_000_000;
                    bloon.speed = 5.8f;
                    ModContent.GetAudioClip<BergsExtraBossPackMOD>("Behrüßung2").Play();
                    break;
                default:
                    break;
            }

        }
        return bloon;
    }
}