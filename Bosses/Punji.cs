using BergsExtraBossPack;
using BossIntegration;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Data.EmotesNS;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Profile;
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

internal class PunjiBoss : ModBoss
{
    protected override int Order => 1;
    public override string DisplayName => "Punji the Businessman";
    public override string Icon => "Punji-Icon";
    public override string Description => "\"He's an gigantic Moneymaster\"";
    public override IEnumerable<string> DamageStates => new string[] { };
    public class PunjiDisplay : ModBloonDisplay<PunjiBoss>
    {
        public override string BaseDisplay => ModDisplay.Generic2dDisplay;
        public PunjiDisplay() { }

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "PunniBoss");
        }
    }
    public override string TimerDescription => "Removes for each Tower you have 5% of your Max Cash and if your Cash is under your TowerCount multiplied with 1000$ and also multiplied with the BossTier squared(^2) the Boss regains his hole Live";
    public override void TimerTick(Bloon boss)
    {
        List<Tower> towers = InGame.instance.GetTowers();
        uint? tier = ModBoss.GetTier(boss);
        double cash = InGame.instance.bridge.GetCash();
        //Special
        boss.health = boss.health + boss.bloonModel.maxHealth * 0.003f > boss.bloonModel.maxHealth ?
        boss.bloonModel.maxHealth :
        boss.health + boss.bloonModel.maxHealth * 0.003f;
        //SpecialEnd
        if (cash < towers.Count*1000*(tier*tier))
        {
            boss.health = boss.bloonModel.maxHealth;
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("I_healed_up").Play();
        }
        else
        {
            InGame.instance.bridge.SetCash(cash *(1-towers.Count * 0.05));
        }

        if (tier == 1)
        {
        }
        else if (tier == 2)
        {
        }
        else if (tier == 3)
        {
        }
        else if (tier == 4)
        {
        }
        else if (tier == 5)
        {
        }

    }
    public override string SkullDescription => "Sells 10% of your Tower Count multiplied with the actual BossTier on Random Towers for their half worth | Spawn for every Tower you have one T1:Moab T2:Bfb T3:Ddt T4:Zomg T5:Bad";
    public override void SkullEffect(Bloon boss)
    {
        uint? tier = ModBoss.GetTier(boss);
        if (tier != null)
        {
            if (tier == 1)
            {
                InGame.instance.SpawnBloons("Moab", InGame.instance.GetTowers().Count, 2);
            }
            else if (tier == 2)
            {
                InGame.instance.SpawnBloons("Bfb", InGame.instance.GetTowers().Count, 2);
            }
            else if (tier == 3)
            {

                InGame.instance.SpawnBloons("Ddt", InGame.instance.GetTowers().Count, 2);
            }
            else if (tier == 4)
            {
                InGame.instance.SpawnBloons("Zomg", InGame.instance.GetTowers().Count, 2);

            }
            else if (tier == 5)
            {
                InGame.instance.SpawnBloons("Bad", InGame.instance.GetTowers().Count+3, 2);
            }
            int oldCount = InGame.instance.GetTowers().Count;
            for (int i = 0;i < (oldCount * (0.2 * tier)); i++)
            {
                Tower tower = InGame.instance.GetTowers()[BossPack.rng.Next(0, InGame.instance.GetTowers().Count)];
                if (tower != null)
                {
                    tower.worth /= 2;
                    InGame.instance.SellTower(tower);
                }
            }


        }


        //Sound
        int x = BossPack.rng.Next(1, 7);
        if (x == 1)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("HeuteSchlagIchBador").Play();
        }
        else if (x == 2)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("ichsterbeschonwieder").Play();
        }
        else if (x == 3)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("indieFresse").Play();
        }
        else if (x == 4)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("menschbador").Play();
        }
        else if (x == 5)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("neinweich").Play();
        }
        else if (x == 6)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("nönö").Play();
        }
        else if (x == 7)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("BitteGehKaputt").Play();
        }
        else if (x == 8)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("einletztesmal").Play();
        }
        else if (x == 9)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("GameCrasht").Play();
        }
        boss.AddMutator(new SpeedUpMutator("PunjiDash", -3f), 150);

    }
    public override Dictionary<int, BossRoundInfo> RoundsInfo => new Dictionary<int, BossRoundInfo>()
    {
        [40] = new BossRoundInfo()
        {
            skullCount = 8,
            interval = 30,
            tier = 1,
            timerDescription = "Removes for each Tower you have 5% of your Max Cash and if your Cash is under your TowerCount multiplied with 1000$ the Boss regains his hole Live",
        },
        [60] = new BossRoundInfo()
        {
            skullCount = 8,
            interval = 30,
            tier = 2,
            timerDescription = "Removes for each Tower you have 5% of your Max Cash and if your Cash is under your TowerCount multiplied with 4000$ the Boss regains his hole Live",
        },
        [80] = new BossRoundInfo()
        {
            skullCount = 8,
            interval = 30,
            tier = 3,
            timerDescription = "Removes for each Tower you have 5% of your Max Cash and if your Cash is under your TowerCount multiplied with 9000$ the Boss regains his hole Live",
        },
        [100] = new BossRoundInfo()
        {
            skullCount = 8,
            interval = 30,
            tier = 4,
            timerDescription = "Removes for each Tower you have 5 % of your Max Cash and if your Cash is under your TowerCount multiplied with 16000$ the Boss regains his hole Live",
        },
        [120] = new BossRoundInfo()
        {
            skullCount = 8,
            interval = 30,
            tier = 5,
            timerDescription = "Removes for each Tower you have 5% of your Max Cash and if your Cash is under your TowerCount multiplied with 25000$ the Boss regains his hole Live",
        },
    };
    public override BloonModel ModifyForRound(BloonModel bloon, int round)
    {
        if (RoundsInfo.TryGetValue(round, out var roundInfo))
        {
            switch (roundInfo.tier)
            {

                case 1:
                    bloon.maxHealth = 100_000;
                    bloon.speed = 3;
                    ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hello").Play();
                    break;
                case 2:
                    bloon.maxHealth = 500_000;
                    bloon.speed = 3.05f;
                    ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hello").Play();
                    break;
                case 3:
                    bloon.maxHealth = 1_000_000;
                    bloon.speed = 3.1f;
                    ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hello").Play();
                    break;
                case 4:
                    bloon.maxHealth = 5_000_000;
                    bloon.speed = 3.15f;
                    ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hello").Play();
                    break;
                case 5:
                    bloon.maxHealth = 25_000_000;
                    bloon.speed = 3.2f;
                    ModContent.GetAudioClip<BergsExtraBossPackMOD>("HeuteSchlagIchBador").Play();
                    break;
                default:
                    break;
            }

        }
        return bloon;
    }
}