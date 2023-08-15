using MelonLoader;
using BTD_Mod_Helper;
using BergsExtraBossPack;
using System;

[assembly: MelonInfo(typeof(BergsExtraBossPack.BergsExtraBossPackMOD), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BergsExtraBossPack;

public class BergsExtraBossPackMOD : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<BergsExtraBossPackMOD>("BergsExtraBossPack loaded!");
    }
    
}
public class BossPack : BloonsTD6Mod
{
    public static Random rng = new Random();
}