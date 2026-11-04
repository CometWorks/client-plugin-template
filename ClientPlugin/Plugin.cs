using System.Reflection;
using ClientPlugin.Settings;
using ClientPlugin.Settings.Layouts;
using HarmonyLib;
using Sandbox.Graphics.GUI;
using VRage.Plugins;

// Define assembly version when compiled by Pulsar
#if !LOCAL_BUILD
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#endif
namespace ClientPlugin;

// ReSharper disable once UnusedType.Global ClassNeverInstantiated.Global
public class Plugin : IPlugin
{
    public const string Name = "ClientPluginTemplate";
    public static Plugin Instance { get; private set; }
    private SettingsGenerator _settingsGenerator;

    [System.Runtime.CompilerServices.MethodImpl(
        System.Runtime.CompilerServices.MethodImplOptions.NoInlining
    )]
    public void Init(object gameInstance)
    {
        Instance = this;
        Instance._settingsGenerator = new SettingsGenerator();

        // TODO: Put your one time initialization code here.
        var harmony = new Harmony(Name);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public void Dispose()
    {
        // TODO: Save state and close resources here, called when the game exits (not guaranteed!)
        // IMPORTANT: Do NOT call harmony.UnpatchAll() here! It may break other plugins.

        Instance = null;
    }

    public void Update()
    {
        // TODO: Put your update code here. It is called on every simulation frame!
    }

    // ReSharper disable once UnusedMember.Global
    public void OpenConfigDialog()
    {
        Instance._settingsGenerator.SetLayout<Simple>();
        MyGuiSandbox.AddScreen(Instance._settingsGenerator.Dialog);
    }

    //TODO: Uncomment and use this method to load asset files
    /*public void LoadAssets(string folder)
    {

    }*/
}
