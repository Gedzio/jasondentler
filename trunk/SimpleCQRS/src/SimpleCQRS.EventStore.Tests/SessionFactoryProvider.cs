using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.Loquacious;
using NHibernate.Dialect;
using NHibernate.Driver;

namespace SimpleCQRS.EventStore
{
  public static class SessionFactoryProvider
  {
    private const string ConnStr =
        "Data Source=:memory:;Version=3;New=True;";


    static SessionFactoryProvider()
    {
      Initialize();
    }

    private static ISessionFactory _sessionFactory;
    private static Configuration _configuration;

    public static ISessionFactory SessionFactory
    {
      get { return _sessionFactory; }
    }

    public static Configuration Configuration
    {
      get { return _configuration; }
    }

    private static void Initialize()
    {
      _configuration = new Configuration().Configure();/*
                .DataBaseIntegration(db =>
                {
                    db.Dialect<SQLiteDialect>();
                    db.Driver<SQLite20Driver>();
                    db.ConnectionProvider<TestConnectionProvider>();
                    db.ConnectionString = ConnStr;
                })
          .SetProperty(Environment.CurrentSessionContextClass,
              "thread_static");

      var props = _configuration.Properties;
      if (props.ContainsKey(Environment.ConnectionStringName))
          props.Remove(Environment.ConnectionStringName);
      */
      _sessionFactory = _configuration.BuildSessionFactory();
    }

    public static bool HasContextualSessions()
    {
      return _configuration.Properties
        .ContainsKey(Environment.CurrentSessionContextClass);
    }

  }
}