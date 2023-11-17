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
/*
internal class EviLPalutenBoss : ModBoss
{
    protected override int Order => 1;
    public override string DisplayName => "Evil Paluten God";
    public override string Icon => "EvilPaluten-Icon";
    public override string Description => "\"He doesn't play God....He is a God\"";
    public override string ExtraCredits => "Sounds were made by Paluten";
    public override IEnumerable<string> DamageStates => new string[] { };
    public class EvilPalutenDisplay : ModBloonDisplay<EviLPalutenBoss>
    {
       
        public override float Scale => 1f;
        public EvilPalutenDisplay() { }

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "PunniBoss");
        }
    }
    public override string TimerDescription => "Removes for each Tower you have 3% of your Max Cash and if your Cash is under your TowerCount multiplied with 750$ and also multiplied with the BossTier squared(^2) the Boss regains his hole Live";
    public override void TimerTick(Bloon boss)
    {
        List<Tower> towers = InGame.instance.GetTowers();
        uint? tier = ModBoss.GetTier(boss);
        double cash = InGame.instance.bridge.GetCash();
        //Special
        boss.health = (int)(boss.health + boss.bloonModel.maxHealth * 0.003f > boss.bloonModel.maxHealth ?
        boss.bloonModel.maxHealth :
        boss.health + boss.bloonModel.maxHealth * 0.003f);
        //SpecialEnd

        if (cash < towers.Count * 750 * (tier * tier))
        {
            boss.health = boss.bloonModel.maxHealth;
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("I_healed_up").Play();
        }
        else
        {
            double newCash;
            newCash = cash * (1 - towers.Count * 0.03);
            if (newCash < 0)
            {
                newCash = 0;
            }
            InGame.instance.bridge.SetCash(newCash);
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
    public override string SkullDescription => "Sells 10% of your Tower Count multiplied with the actual BossTier on Random Towers for 75% of their worth | Spawn for every Tower you have one T1:Moab T2:Bfb T3:Ddt T4:Zomg T5:Bad";
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
                InGame.instance.SpawnBloons("Bad", InGame.instance.GetTowers().Count + 3, 2);
            }
            int oldCount = InGame.instance.GetTowers().Count;
            List<int> checkedIndexes = new List<int>();
            for (int i = 0; i < (oldCount * (0.2 * tier)); i++)
            {
                int towerIndex;
                while (true)
                {
                    towerIndex = BossPack.rng.Next(0, InGame.instance.GetTowers().Count);
                    if (checkedIndexes.Contains(towerIndex))
                    {
                        continue;
                    }

                    break;
                }
                checkedIndexes.Add(towerIndex);
                Tower tower = InGame.instance.GetTowers()[towerIndex];
                if (tower != null)
                {
                    tower.worth *= 0.75f;
                    InGame.instance.SellTower(tower);
                }
            }


        }


        //Sound
        int x = BossPack.rng.Next(1, 60);
        if (boss.health < boss.bloonModel.maxHealth / 7.5 || boss.isDestroyed)
        {
            x = 0;
            int z = BossPack.rng.Next(1, 4);
            if (z == 1)
            {
                ModContent.GetAudioClip<BergsExtraBossPackMOD>("GameCrasht").Play();
            }
            else if (z == 2)
            {
                ModContent.GetAudioClip<BergsExtraBossPackMOD>("Ich_bin_schon_wieder_tod_nein").Play();
            }
            else if (z == 3)
            {
                ModContent.GetAudioClip<BergsExtraBossPackMOD>("ichsterbeschonwieder").Play();
            }
            else if (z == 4)
            {
                ModContent.GetAudioClip<BergsExtraBossPackMOD>("oh_my_god_i_am_dead").Play();
            }
        }
        if (x == 1)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("HeuteSchlagIchBador").Play();
        }
        else if (x == 2)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("Nein").Play();
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
        else if (x == 10)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("of_course_I_talk_to_myself").Play();
        }
        else if (x == 11)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("punjihi").Play();
        }
        else if (x == 12)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("of_course_I_talk_to_myself").Play();
        }
        else if (x == 13)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("Nein").Play();
        }
        else if (x == 14)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("punjihi").Play();
        }
        else if (x == 15)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("I_bet_you_forgot_to_overclock").Play();
        }
        else if (x == 16)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("ganz_im_ernst_ich_mag_bador").Play();
        }
        else if (x == 17)
        {
            ModContent.GetAudioClip<BergsExtraBossPackMOD>("deutschland_1_du_0").Play();
        }
        boss.AddMutator(new SpeedUpMutator("PalutenDash", 3f), 150);

    }
    public override Dictionary<int, BossRoundInfo> RoundsInfo => new Dictionary<int, BossRoundInfo>()
    {
        [40] = new BossRoundInfo()
        {
            skullCount = 1,
            interval = 3,
            tier = 1,
            preventFallThrough = true,
            timerDescription = "Removes for each Tower you have 3% of your Max Cash and if your Cash is under your TowerCount multiplied with 750$ the Boss regains his hole Live",
        },
        [60] = new BossRoundInfo()
        {
            skullCount = 2,
            interval = 3,
            tier = 2,
            preventFallThrough = true,
            timerDescription = "Removes for each Tower you have 3% of your Max Cash and if your Cash is under your TowerCount multiplied with 3000$ the Boss regains his hole Live",
        },
        [80] = new BossRoundInfo()
        {
            skullCount = 4,
            interval = 3,
            tier = 3,
            preventFallThrough = true,
            timerDescription = "Removes for each Tower you have 3% of your Max Cash and if your Cash is under your TowerCount multiplied with 67500$ the Boss regains his hole Live",
        },
        [100] = new BossRoundInfo()
        {
            skullCount = 8,
            interval = 3,
            tier = 4,
            preventFallThrough = true,
            timerDescription = "Removes for each Tower you have 3% of your Max Cash and if your Cash is under your TowerCount multiplied with 12000$ the Boss regains his hole Live",
        },
        [120] = new BossRoundInfo()
        {
            skullCount = 16,
            interval = 3,
            tier = 5,
            preventFallThrough = true,
            timerDescription = "Removes for each Tower you have 3% of your Max Cash and if your Cash is under your TowerCount multiplied with 187500$ the Boss regains his hole Live",
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
                    bloon.speed = 1;
                    int y = BossPack.rng.Next(1, 7);
                    if (y == 1)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hello").Play();
                    }
                    else if (y == 2)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hey_Guys").Play();
                    }
                    else if (y == 3)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Halloooooo").Play();
                    }
                    else if (y == 4)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hello").Play();
                    }
                    else if (y == 5)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hey_Guys").Play();
                    }
                    else if (y == 6)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Halloooooo").Play();
                    }
                    else if (y == 7)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Halloooooo").Play();
                    }
                    break;
                case 2:
                    bloon.maxHealth = 500_000;
                    bloon.speed = 1.05f;
                    int y2 = BossPack.rng.Next(1, 7);
                    if (y2 == 1)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hello").Play();
                    }
                    else if (y2 == 2)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("och_no_du_schon_wieder").Play();
                    }
                    else if (y2 == 3)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Halloooooo").Play();
                    }
                    else if (y2 == 4)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("da_bin_ich_wieder").Play();
                    }
                    else if (y2 == 5)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hey_Guys").Play();
                    }
                    else if (y2 == 6)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("och_no_du_schon_wieder").Play();
                    }
                    else if (y2 == 7)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("da_bin_ich_wieder").Play();
                    }
                    break;
                case 3:
                    bloon.maxHealth = 2_000_000;
                    bloon.speed = 1.1f;
                    int y3 = BossPack.rng.Next(1, 7);
                    if (y3 == 1)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hello").Play();
                    }
                    else if (y3 == 2)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("da_bin_ich_wieder").Play();
                    }
                    else if (y3 == 3)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Halloooooo").Play();
                    }
                    else if (y3 == 4)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hello").Play();
                    }
                    else if (y3 == 5)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hey_Guys").Play();
                    }
                    else if (y3 == 6)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("och_no_du_schon_wieder").Play();
                    }
                    else if (y3 == 7)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("da_bin_ich_wieder").Play();
                    }
                    break;
                case 4:
                    bloon.maxHealth = 7_500_000;
                    bloon.speed = 1.15f;
                    int y4 = BossPack.rng.Next(1, 7);
                    if (y4 == 1)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hello").Play();
                    }
                    else if (y4 == 2)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("och_no_du_schon_wiede").Play();
                    }
                    else if (y4 == 3)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Halloooooo").Play();
                    }
                    else if (y4 == 4)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("da_bin_ich_wieder").Play();
                    }
                    else if (y4 == 5)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hey_Guys").Play();
                    }
                    else if (y4 == 6)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("och_no_du_schon_wieder").Play();
                    }
                    else if (y4 == 7)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("da_bin_ich_wieder").Play();
                    }
                    break;
                case 5:
                    bloon.maxHealth = 30_000_000;
                    bloon.speed = 1.2f;
                    int y5 = BossPack.rng.Next(1, 7);
                    if (y5 == 1)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hello").Play();
                    }
                    else if (y5 == 2)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("einletztesmal").Play();
                    }
                    else if (y5 == 3)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Halloooooo").Play();
                    }
                    else if (y5 == 4)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("einletztesmal").Play();
                    }
                    else if (y5 == 5)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("Hey_Guys").Play();
                    }
                    else if (y5 == 6)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("och_no_du_schon_wieder").Play();
                    }
                    else if (y5 == 7)
                    {
                        ModContent.GetAudioClip<BergsExtraBossPackMOD>("da_bin_ich_wieder").Play();
                    }
                    break;
                default:
                    break;
            }

        }
        return bloon;
    }
}
 */