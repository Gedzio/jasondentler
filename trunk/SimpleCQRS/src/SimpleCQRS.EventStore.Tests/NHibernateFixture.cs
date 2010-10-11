using NHibernate;
using NHibernate.Context;
using NHibernate.Tool.hbm2ddl;

namespace SimpleCQRS.EventStore
{
  public abstract class NHibernateFixture : BaseFixture
  {

    protected ISessionFactory SessionFactory
    {
      get { return SessionFactoryProvider.SessionFactory; }
    }

    protected override void OnSetup()
    {
      base.OnSetup();
      SetupNHibernateSession();

    }

    protected override void OnTeardown()
    {
      TearDownNHibernateSession();
      base.OnTeardown();
    }

    protected void SetupNHibernateSession()
    {
      TestConnectionProvider.CloseDatabase();
      SetupContextualSession();
      BuildSchema();
    }

    protected void TearDownNHibernateSession()
    {
      TearDownContextualSession();
      TestConnectionProvider.CloseDatabase();
    }

    private void SetupContextualSession()
    {
      if (SessionFactoryProvider.HasContextualSessions())
      {
        var session = SessionFactory.OpenSession();
        CurrentSessionContext.Bind(session);
      }
    }

    private void TearDownContextualSession()
    {
      if (SessionFactoryProvider.HasContextualSessions())
      {
        var session = CurrentSessionContext.Unbind(SessionFactory);
        session.Close();
      }
    }

    private static void BuildSchema()
    {
      var cfg = SessionFactoryProvider.Configuration;
      var schemaExport = new SchemaExport(cfg);
      schemaExport.Create(false, true);
    }

  }
}