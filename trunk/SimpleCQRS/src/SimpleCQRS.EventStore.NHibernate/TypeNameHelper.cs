using System;

namespace SimpleCQRS.EventStore.NHibernate
{
  public static class TypeNameHelper
  {
    
    public static string GetSimpleTypeName(object obj)
    {
      return null == obj
               ? null
               : obj.GetType().AssemblyQualifiedName;
    }

    public static Type GetType(string simpleTypeName)
    {
      return Type.GetType(simpleTypeName);
    }

  }
}
