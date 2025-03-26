using DataLayer;
using Godot;
using Stargazer;
using System;
using System.Threading.Tasks;

public interface IUserUpdateReceiver
{
    public Task UpdateUserPosition(CelestialDataPackage<Star> dataPackage);
}
