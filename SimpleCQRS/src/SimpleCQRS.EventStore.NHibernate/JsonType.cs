using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using Newtonsoft.Json;

namespace SimpleCQRS.EventStore.NHibernate
{

  [Serializable]
  public class JsonType : IUserType
  {

    private static object Deserialize(string data, string type)
    {
      return Deserialize(data, TypeNameHelper.GetType(type));
    }

    private static object Deserialize(string data, Type type)
    {
      return JsonConvert.DeserializeObject(data, type);
    }

    private static string Serialize(object value)
    {
      return null == value
                ? null
                : JsonConvert.SerializeObject(value);
    }

    private static string GetType(object value)
    {
      return null == value
                ? null
                : TypeNameHelper.GetSimpleTypeName(value);
    }

    public object NullSafeGet(IDataReader rs, string[] names, object owner)
    {
      int typeIndex = rs.GetOrdinal(names[0]);
      int dataIndex = rs.GetOrdinal(names[1]);
      if (rs.IsDBNull(typeIndex) || rs.IsDBNull(dataIndex))
      {
        return null;
      }

      var type = (string) rs.GetValue(typeIndex);
      var data = (string) rs.GetValue(dataIndex);
      return Deserialize(data, type);
    }

    public void NullSafeSet(IDbCommand cmd, object value, int index)
    {
      if (value == null)
      {
        NHibernateUtil.String.NullSafeSet(cmd, null, index);
        NHibernateUtil.String.NullSafeSet(cmd, null, index + 1);
        return;
      }

      var type = GetType(value);
      var data = Serialize(value);
      NHibernateUtil.String.NullSafeSet(cmd, type, index);
      NHibernateUtil.String.NullSafeSet(cmd, data, index + 1);
    }

    public object DeepCopy(object value)
    {
      return value == null
                ? null
                : Deserialize(Serialize(value), GetType(value));
    }

    public object Replace(object original, object target, object owner)
    {
      return original;
    }

    public object Assemble(object cached, object owner)
    {
      var parts = cached as string[];
      return parts == null
                ? null
                : Deserialize(parts[1], parts[0]);
    }

    public object Disassemble(object value)
    {
      return (value == null)
                ? null
                : new string[]
                    {
                      GetType(value),
                      Serialize(value)
                    };
    }

    public SqlType[] SqlTypes
    {
      get
      {
        return new[]
                  {
                    SqlTypeFactory.GetString(10000), // Type
                    SqlTypeFactory.GetStringClob(10000) // Data
                  };
      }
    }

    public Type ReturnedType
    {
      get { return typeof(Event); }
    }

    public bool IsMutable
    {
      get { return false; }
    }

    public new bool Equals(object x, object y)
    {
      if (ReferenceEquals(x, y))
      {
        return true;
      }
      if (ReferenceEquals(null, x) || ReferenceEquals(null, y))
      {
        return false;
      }

      return x.Equals(y);
    }

    public int GetHashCode(object x)
    {
      return (x == null) ? 0 : x.GetHashCode();
    }
  }

}
