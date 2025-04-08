using ComputerInterface.Interfaces;
using GorillaRegionOverride.Models;
using Zenject;

namespace GorillaRegionOverride;

public class MainInstaller : Installer
{
    public override void InstallBindings()
    {
        Container.Bind<IComputerModEntry>().To<ScreenEntry>().AsSingle();
    }
}