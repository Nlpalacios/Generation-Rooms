using System;
using System.Collections.Generic;

public class ContainerDependences 
{
    private Dictionary<Type, Type> typeMappings = new Dictionary<Type, Type>();
    private Dictionary<Type, object> instanceMappings = new Dictionary<Type, object>();

    public void Register<TInterface, TImplementation>() where TImplementation : TInterface
    {
        typeMappings[typeof(TInterface)] = typeof(TImplementation);
    }

    public void RegisterInstance<TInterface>(TInterface instance)
    {
        instanceMappings[typeof(TInterface)] = instance;
    }

    public TInterface Resolve<TInterface>()
    {
        Type type = typeof(TInterface);

        if (instanceMappings.ContainsKey(type))
        {
            return (TInterface)instanceMappings[type];
        }

        if (typeMappings.ContainsKey(type))
        {
            Type implementationType = typeMappings[type];
            var instance = Activator.CreateInstance(implementationType);
            return (TInterface)instance;
        }

        throw new Exception($"No registration for {type}");
    }

}
