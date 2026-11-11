using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using AnnShihoConfig.Template;
using AnnShihoConfig.Configuration;
using CriFs.V2.Hook.Interfaces;
#if DEBUG
using System.Diagnostics;
#endif

namespace AnnShihoConfig;

public class Mod : ModBase // <= Do not Remove.
{
        private readonly IModLoader _modLoader;
        private readonly IReloadedHooks? _hooks;
        private readonly ILogger _logger;
        private readonly IMod _owner;
        private Config _configuration;
        private readonly IModConfig _modConfig;

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;

#if DEBUG
        // Attaches debugger in debug mode; ignored in release.
        Debugger.Launch();
#endif

        string modDir = _modLoader.GetDirectoryForModId(_modConfig.ModId);
        string modId = _modConfig.ModId;

        var criFsCtl = _modLoader.GetController<ICriFsRedirectorApi>();
        var bfEmuCtl = _modLoader.GetController<BF.File.Emulator.Interfaces.IBfEmulator>();
        var bmdEmuCtl = _modLoader.GetController<BMD.File.Emulator.Interfaces.IBmdEmulator>();
        var pakEmuCtl = _modLoader.GetController<PAK.Stream.Emulator.Interfaces.IPakEmulator>();

        if (criFsCtl == null || !criFsCtl.TryGetTarget(out var criFsApi)) { _logger.WriteLine("CRI FS Emu missing → config binds broken.", System.Drawing.Color.Red); return; }
        if (bfEmuCtl == null || !bfEmuCtl.TryGetTarget(out var bfEmu)) { _logger.WriteLine("BF Emulator API missing → file emulation broken.", System.Drawing.Color.Red); return; }
        if (bmdEmuCtl == null || !bmdEmuCtl.TryGetTarget(out var bmdEmu)) { _logger.WriteLine("BMD Emu missing → BMD merges broken.", System.Drawing.Color.Red); return; }
        if (pakEmuCtl == null || !pakEmuCtl.TryGetTarget(out var pakEmu)) { _logger.WriteLine("PAK Emu missing → PAK merges broken.", System.Drawing.Color.Red); return; }
            

        // Rose & Violet patch
        if (_configuration.RVPatch)
        {
            _logger.WriteLine($"AnnShiho R&V patch selected.");
            var patchFolder = Path.Combine(modDir, "OptionalModFiles", "Rose&Violet");
            if (Directory.Exists(patchFolder))
            {
                _logger.WriteLine($"Found AnnShiho R&V patch at {patchFolder}.");
                bfEmu.AddDirectory(Path.Combine(patchFolder, "BF"));
                bmdEmu.AddDirectory(Path.Combine(patchFolder, "BF"));
                bmdEmu.AddDirectory(Path.Combine(patchFolder, "BMD"));
                pakEmu.AddDirectory(Path.Combine(patchFolder, "PAK"));
            }
            else
            {
                _logger.WriteLine($"Couldn't find AnnShiho R&V patch at {patchFolder}.", System.Drawing.Color.Red);
            }
        }
        else
        {
            _logger.WriteLine($"AnnShiho R&V patch not selected.");
        }
    }

    #region Standard Overrides
    public override void ConfigurationUpdated(Config configuration)
    {
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }
    #endregion

    #region For Exports, Serialization etc.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod() { }
#pragma warning restore CS8618
    #endregion
}