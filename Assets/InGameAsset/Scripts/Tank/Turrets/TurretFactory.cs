using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class TurretFactory
{
    // static Dictionary<string, Type> _turretByName;
    // static bool _isIntialized => _turretByName != null;
    //
    // static void InitializeFactory()
    // {
    //     if (_isIntialized) return;
    //     var turretTypes = Assembly.GetAssembly(typeof(ITurret)).GetTypes()
    //         .Where(myType => myType.IsClass && !myType.IsSubclassOf(typeof(Turret)));
    //     _turretByName = new Dictionary<string, Type>();
    //
    //     foreach (var turretType in turretTypes)
    //     {
    //         var tempTurret = Activator.CreateInstance(turretType) as Turret;
    //         _turretByName.Add(tempTurret.Name,turretType);
    //     }
    // }
    //
    // public static ITurret GetTurret(string turretType)
    // {
    //     InitializeFactory();
    //     if (_turretByName.ContainsKey(turretType))
    //     {
    //         Type type = _turretByName[turretType];
    //         var turret = Activator.CreateInstance(type) as Turret;
    //     }
    //     return null;
    // }
    //
    // internal static IEnumerable<String> GetTurretName()
    // {
    //     InitializeFactory();
    //     return _turretByName.Keys;
    // }

}