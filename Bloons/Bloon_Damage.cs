using BossIntegration;
using BossPackReborn.Bosses;
using HarmonyLib;
using Il2Cpp;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Projectiles;

namespace BossPackReborn.Patches.Bloons;

[HarmonyPatch(typeof(Bloon), nameof(Bloon.Damage))]
internal class Bloon_Damage
{
    [HarmonyPrefix]
    internal static bool Prefix(Bloon __instance, float totalAmount, Projectile projectile,
        bool distributeToChildren, bool overrideDistributeBlocker, bool createEffect,
        Tower tower, BloonProperties immuneBloonProperties,
        bool canDestroyProjectile = true, bool ignoreNonTargetable = false,
        bool blockSpawnChildren = false, bool ignoreInvunerable = false)
    {
        bool result = true;


        return result;
    }
}