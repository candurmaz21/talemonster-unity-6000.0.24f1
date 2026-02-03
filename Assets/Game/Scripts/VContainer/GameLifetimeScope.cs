using UnityEngine;
using VContainer;
using VContainer.Unity;
using Game.LevelModule.Config;
using Game.GridModule;
using Game.PoolSystem;
using Game.HeroModule;
using Game.HeroModule.View;
using Game.Signal.Core;
using Game.HeroModule.Config;
using Game.HeroModule.Controller;
using Game.GoalModule.Controller;
using Game.GridModule.Controller;
using Game.SceneLayout;
using Game.GridModule.View;
using Game.MatchModule.Controller;
using Game.HeroModule.Service;
using Game.MatchModule.Service;
using Game.GridModule.Service;
using Game.BoardModule.Controller;
using Game.GoalModule.Service;
using Game.GoalModule.View;
using Game.LevelModule.Service;
using Game.ParticleModule.Config;
using Game.ParticleModule.Service;
using Game.ParticleModule.Controller;
using Game.GridModule.Factory;

public sealed class GameLifetimeScope : LifetimeScope
{
    [Header("Configs")]
    [SerializeField] private LevelsConfig levelsConfig;
    [SerializeField] private HeroConfig heroConfig;
    [SerializeField] private ParticleConfig particleConfig;

    [Header("Prefabs")]
    [SerializeField] private GameObject cellPrefab;

    [Header("Components")]
    [SerializeField] private Transform heroPoolRoot;
    [SerializeField] private Transform cellPoolRoot;
    [SerializeField] private Transform mainBackgroundTransform;
    [SerializeField] private Transform particlePoolRoot;
    [SerializeField] private Transform goalPoolRoot;
    [SerializeField] private SpriteRenderer gridBackgroundRenderer;

    [Header("Goal Module")]
    [SerializeField] private GoalHeroView goalHeroPrefab;
    [SerializeField] private GoalPanelView goalPanelView;

    [Header("UI")]
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private Camera worldCamera;

    protected override void Configure(IContainerBuilder builder)
    {
        RegisterLevel(builder);
        RegisterComponents(builder);
        RegisterSignal(builder);
        RegisterGridModules(builder);
        RegisterLayoutModules(builder);
        RegisterHeroModules(builder);
        RegisterCoreModules(builder);
        RegisterGoalModules(builder);
        RegisterGameFlowModules(builder);
        RegisterParticleModules(builder);
    }

    private void RegisterLevel(IContainerBuilder builder)
    {
        LevelProvider levelProvider = new LevelProvider(levelsConfig);
        builder.RegisterInstance(levelProvider).AsSelf();

        LevelConfig levelConfig = levelProvider.GetCurrentLevelConfig();
        builder.RegisterInstance(levelConfig);
    }

    private void RegisterSignal(IContainerBuilder builder)
    {
        builder.Register<SignalCenter>(Lifetime.Singleton);
    }

    private void RegisterComponents(IContainerBuilder builder)
    {
        builder.RegisterInstance(heroConfig);
        builder.RegisterInstance(gridBackgroundRenderer);
        builder.RegisterInstance(cellPrefab);
        builder.RegisterInstance(particleConfig);
        builder.RegisterInstance(uiCanvas);
        builder.RegisterInstance(worldCamera);
    }

    private void RegisterLayoutModules(IContainerBuilder builder)
    {
        builder.Register<CameraFramer>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<GridBackgroundSizer>(Lifetime.Singleton).AsImplementedInterfaces();
        builder.Register<BackgroundScaler>(Lifetime.Singleton).AsImplementedInterfaces()
                            .WithParameter("backgroundTransform", mainBackgroundTransform);
    }
    private void RegisterHeroModules(IContainerBuilder builder)
    {
        builder.Register<HeroPoolService>(Lifetime.Singleton)
           .WithParameter("heroPoolRoot", heroPoolRoot);
        builder.Register<HeroViewFactory>(Lifetime.Singleton);
        builder.Register<HeroSpawner>(Lifetime.Singleton)
           .WithParameter("rHeroPoolRoot", heroPoolRoot);
        builder.Register<HeroViewRegistery>(Lifetime.Scoped);
    }

    private void RegisterCoreModules(IContainerBuilder builder)
    {
        builder.Register<GridController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
        builder.Register<BoardController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

        builder.Register<ISwapProvider, HeroSwapProvider>(Lifetime.Singleton);

        builder.Register<MatchAnimationService>(Lifetime.Singleton);
        builder.Register<MatchFinder>(Lifetime.Singleton);
        builder.Register<IMatchController, HeroMatchController>(Lifetime.Singleton);
        builder.Register<IGridGravityService, GridGravityService>(Lifetime.Singleton);
        builder.Register<BoardFillController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
    }

    private void RegisterGoalModules(IContainerBuilder builder)
    {
        builder.Register<GoalPoolService>(Lifetime.Singleton)
                            .WithParameter("rPrefab", goalHeroPrefab)
                            .WithParameter("rPoolRoot", goalPoolRoot);

        builder.Register<GoalCollectAnimator>(Lifetime.Singleton).As<IGoalCollectAnimator>();
        builder.RegisterInstance<IGoalUIProvider>(goalPanelView);

        builder.Register<GoalCollectionController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
    }

    private void RegisterGridModules(IContainerBuilder builder)
    {
        builder.Register<GridCellViewFactory>(Lifetime.Singleton)
                            .WithParameter("rCellPoolRoot", cellPoolRoot);
        builder.Register<GridCellViewPopulator>(Lifetime.Singleton).AsImplementedInterfaces();
    }

    private void RegisterGameFlowModules(IContainerBuilder builder)
    {
        builder.Register<GameFlowController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
    }

    private void RegisterParticleModules(IContainerBuilder builder)
    {
        builder.Register<ParticlePoolService>(Lifetime.Singleton)
                            .WithParameter("rParticlePool", particlePoolRoot);

        builder.Register<ParticleSpawner>(Lifetime.Singleton);
        builder.Register<ParticleController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();
    }

}