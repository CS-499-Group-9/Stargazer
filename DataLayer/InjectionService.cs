using DataLayer.Implementations;
using DataLayer.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DataLayer
{
    /// <summary>
    /// Used to provide dependency injection to instantiate interface and abstract objects and provide dependencies to constructors.
    /// Registers all classes to be injected into constructors to avoid hard coding into descendant classes.
    /// </summary>
    /// <typeparam name="T">The class that instantiates the star in the front end.</typeparam>

    public static class InjectionService<T>
    {
        /// <summary>
        /// Used to instantiate the repository service.
        /// </summary>
        /// <param name="baseDirectoryPath">The base path of the executing application. (Used to find the location of the repositories in DataLayer/Repositories)</param>
        /// <returns>A new <c>StargazerRepositoryService</c> instance.</returns>
        /// <exception cref="DirectoryNotFoundException">If the base directory of the executing program cannot be found.</exception>
        public static IRepositoryService<T> GetRepositoryServiceAsync(string baseDirectoryPath)
        {
            /*
             * Register all interface implementations here. 
             * Singleton is best for this (actually irrelevant due to the current architecture, each object is only ever created once)
             * AddSingleton<Interface, Implementation>() is used to register an interface with it's specific implementation
             * AddSingleton<Class>() is used to register a concrete class that needs dependencies injected in the constructor, or creates instances in it's constructor that requires dependencies injected
             * AddSingleton<Interface, Implementation>(provider => new Implementation(someValue)) is used to pass a specific value or thing to the constructor of an interface implementation
            */
            //DirectoryInfo dir = new DirectoryInfo(baseDirectoryPath) ?? throw new DirectoryNotFoundException($"{baseDirectoryPath} is not a valid directory");
            //string dataLayer = Path.Combine(dir.Parent.FullName, "DataLayer") ?? throw new DirectoryNotFoundException();
            string repositoryPath = Path.Combine(baseDirectoryPath, "Repositories");

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IStarRepository, HygCsvStarRepository>(provider => new HygCsvStarRepository(repositoryPath))
                .AddSingleton<IMessierRepository, StarLustMessierCsvRepository>(provider => new StarLustMessierCsvRepository(repositoryPath))
                .AddSingleton<IConstellationRepository, StellariumJsonConstellationRepository>(provider => new StellariumJsonConstellationRepository(repositoryPath))
                // Since the type of T is not known at compile time, the injection service must be made aware to evaluate the type when registering.
                .AddSingleton(typeof(IRepositoryService<>), typeof(StargazerRepositoryService<>))
                .BuildServiceProvider();

            return serviceProvider.GetRequiredService<IRepositoryService<T>>();
        }

    }
}
