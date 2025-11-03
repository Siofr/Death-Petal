using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Predefined Assembly Utilities
///
/// Used to find existing Event Types from compiled C# scripts
/// for the creation of the needed EventBuses per Event Type
/// </summary>
public static class PredefinedAssemblyUtils
{
    //Enum for Assembly Types Readability
    private enum AssemblyType
    {
        AssemblyCSharp,
        AssemblyCSharpEditor,
        AssemblyCSharpEditorFirstPass,
        AssemblyCSharpFirstPass,
    }
    
    //Converts assembly name to Assembly enum type
    private static AssemblyType? GetAssemblyType(string assemblyName)
    {
        return assemblyName switch
        {
            "Assembly-CSharp" => AssemblyType.AssemblyCSharp,
            "Assembly-CSharp-Editor" => AssemblyType.AssemblyCSharpEditor,
            "Assembly-CSharp-Editor-firstpass" => AssemblyType.AssemblyCSharpEditorFirstPass,
            "Assembly-CSharp-firstpass" => AssemblyType.AssemblyCSharpFirstPass,
            _ => null
        };
    }
    
    //Gets Event types from a targeted assembly
    private static void AddTypesFromAssembly(Type[] assembly, ICollection<Type> types, Type interfaceType)
    {
        if (assembly == null) return;

        for (int i = 0; i < assembly.Length; ++i)
        {
            Type type = assembly[i];
            
            if(type != interfaceType && interfaceType.IsAssignableFrom(type))
                types.Add(type);
        }
    }
    
    //Returns a list of Event Types
    public static List<Type> GetType(Type interfaceType)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        Dictionary<AssemblyType, Type[]> assemblyTypes = new Dictionary<AssemblyType, Type[]>();
        List<Type> types = new List<Type>();

        for (int i = 0; i < assemblies.Length; ++i)
        {
            AssemblyType? assemblyType = GetAssemblyType(assemblies[i].GetName().Name);

            if (assemblyType != null)
            {
                assemblyTypes.Add((AssemblyType) assemblyType, assemblies[i].GetTypes());
            }
        }
        
        AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharp], types, interfaceType);
        //AddTypesFromAssembly(assemblyTypes[AssemblyType.AssemblyCSharpFirstPass], types, interfaceType);
        
        return types;
    }
}