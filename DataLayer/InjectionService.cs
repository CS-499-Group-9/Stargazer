

using DataLayer.Interfaces;
using DataLayer.Implementations;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using DataLayer.EquitorialObjects;
using CosineKitty;
using DataLayer.HorizontalObjects;
using System.Diagnostics;
using System;
namespace DataLayer
{
    /*
     * Used to register all classes to be injected into constructors
     * This means that no implementations have to be hardcoded into decendant classes 
     * This file contains the implementation configuration for all interface implementations injected into class constructors
     * During startup, when creating the objects, use a reference to the ServiceProvider and call ServiceProvider.GetRequiredServices<T>() where T is the object you wish to create
     * Any parameters needed in the constructor will be injected, as well as any parameters needed by any object created by that constructor or any constructors of composite objects
     * If an interface is swapped out anywhere in the program, just change the interface in the registration below and no other code will need to be changed. 
     */
    public static class InjectionService<T>
    {
        // Set the max (min) star magnitude. The magnitude is stored with smaller numbers being brighter, so the filter ends up being magnitude <= maxStarMagnitude
        const int maxStarMagnitude = 6;

        public static async Task<StargazerRepositoryService<T>> GetRepositoryServiceAsync(string baseDirectoryPath)
        {
            /*
             * Register all interface implementations here. 
             * Singleton is best for this (actually irrelevant due to the current architecture, each object is only ever created once)
             * AddSingleton<Interface, Implementation>() is used to register an interface with it's specific implementation
             * AddSingleton<Class>() is used to register a concrete class that needs dependancies injected in the constructor, or creates instances in it's constructor that requires dependancies injected
             * AddSingleton<Interface, Implementation>(provider => new Implementation(someValue)) is used to pass a specific value or thing to the constructor of an interface implementation
            */
            DirectoryInfo dir = new(baseDirectoryPath);
            if (dir == null) { throw new DirectoryNotFoundException($"{baseDirectoryPath} is not a valid directory"); }
            string dataLayer = Path.Combine(dir.Parent.FullName, "DataLayer");
            string repositoryPath = Path.Combine(dataLayer, "Repositories");

            var serviceProvider =  new ServiceCollection()
                .AddSingleton<IStarRepository, HygCsvStarRepository>(provider => new HygCsvStarRepository(repositoryPath))
                .AddSingleton<IMessierRepository, StarLustMessierCsvRepository>(provider => new StarLustMessierCsvRepository(repositoryPath))
                .AddSingleton<IConstellationRepository, StellariumJsonConstellationRepository>(provider => new StellariumJsonConstellationRepository(repositoryPath))
                .AddSingleton(async provider =>
                {
                    var starRepository = provider.GetRequiredService<IStarRepository>();
                    var constellationRepository = provider.GetRequiredService<IConstellationRepository>();
                    var messierRepository = provider.GetRequiredService<IMessierRepository>();

                    // Use CreateAsync to initialize the StargazerRepositoryService asynchronously
                    return await StargazerRepositoryService<T>.CreateAsync(starRepository, constellationRepository, messierRepository);
                })
                .BuildServiceProvider();
            return await serviceProvider.GetRequiredService<Task<StargazerRepositoryService<T>>>();
        }

    }
}
