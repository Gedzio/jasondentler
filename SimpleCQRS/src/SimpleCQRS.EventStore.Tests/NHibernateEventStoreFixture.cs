using System;
using System.Linq;
using NHibernate;
using NUnit.Framework;
using Ninject;
using SharpTestsEx;
using SimpleCQRS.EventStore.NHibernate;

namespace SimpleCQRS.EventStore
{

  [TestFixture]
  public class NHibernateEventStoreFixture : NHibernateFixture
  {

    private IKernel _kernel;

    protected override void OnFixtureSetup()
    {
      base.OnFixtureSetup();
      _kernel = new StandardKernel();
      _kernel.Bind<IEventPublisher>().To<FakeBus>();
      _kernel.Bind<IStatelessSession>()
        .ToMethod(ctx => SessionFactory.OpenStatelessSession());
      _kernel.Bind<IEventStore>().To<NHibernateEventStore>();
    }

    public class TestEvent : Event
    {

      public readonly string Name;

      public TestEvent(string name)
      {
        Name = name;
      }

    }

    [Test]
    public void GetEventsReturnsCorrectNumberOfEvents()
    {
      var aggregateId = Guid.NewGuid();
      var @event = new TestEvent("Bob");
      var es = _kernel.Get<IEventStore>();
      es.SaveEvents(aggregateId, new Event[] { @event }, -1);

      var events = es.GetEventsForAggregate(aggregateId);
      events.Count().Should().Be.EqualTo(1);
    }

    [Test]
    public void EventTypeIsPreserved()
    {
      var aggregateId = Guid.NewGuid();
      var @event = new TestEvent("Bob");
      var es = _kernel.Get<IEventStore>();
      es.SaveEvents(aggregateId, new Event[] { @event }, -1);

      var events = es.GetEventsForAggregate(aggregateId);
      events.Single().Should().Be.InstanceOf<TestEvent>();
    }

    [Test]
    public void EventDataIsPreserved()
    {
      var aggregateId = Guid.NewGuid();
      var @event = new TestEvent("Bob");
      var es = _kernel.Get<IEventStore>();
      es.SaveEvents(aggregateId, new Event[] { @event }, -1);

      var events = es.GetEventsForAggregate(aggregateId);
      var testEvent = events.Single() as TestEvent;
      testEvent.Name.Should().Be.EqualTo("Bob");
    }

    [Test]
    public void CanSaveMultipleEvents()
    {
      var aggregateId = Guid.NewGuid();
      var eventsIn = new Event[]
                       {
                         new TestEvent("Number 1"),
                         new TestEvent("Number 2")
                       };
      var es = _kernel.Get<IEventStore>();
      es.SaveEvents(aggregateId, eventsIn, -1);

      var eventsOut = es.GetEventsForAggregate(aggregateId);
      eventsOut.Count.Should().Be.EqualTo(eventsIn.Count());
    }

    [Test]
    public void FirstEventIsVersionZero()
    {
      var aggregateId = Guid.NewGuid();
      var eventsIn = new Event[]
                       {
                         new TestEvent("Number 1"),
                       };
      var es = _kernel.Get<IEventStore>();
      es.SaveEvents(aggregateId, eventsIn, -1);

      var eventsOut = es.GetEventsForAggregate(aggregateId);
      eventsOut.Single().Version.Should().Be.EqualTo(0);
    }

    [Test]
    public void VersionsAreSequential()
    {
      var aggregateId = Guid.NewGuid();
      var eventsIn = new Event[]
                       {
                         new TestEvent("Number 1"),
                         new TestEvent("Number 2"),
                         new TestEvent("Number 3"),
                       };
      var es = _kernel.Get<IEventStore>();
      es.SaveEvents(aggregateId, eventsIn, -1);

      var eventsOut = es.GetEventsForAggregate(aggregateId);
      eventsOut[0].Version.Should().Be.EqualTo(0);
      eventsOut[1].Version.Should().Be.EqualTo(1);
      eventsOut[2].Version.Should().Be.EqualTo(2);
    }

    [Test]
    public void ConcurrencyViolationThrowsConcurrencyException()
    {
      var aggregateId = Guid.NewGuid();
      var eventsIn = new Event[]
                       {
                         new TestEvent("Number 1"),
                         new TestEvent("Number 2"),
                         new TestEvent("Number 3"),
                       };
      var es = _kernel.Get<IEventStore>();
      es.SaveEvents(aggregateId, eventsIn, -1);

      eventsIn = new Event[]
                   {
                     new TestEvent("Number 4")
                   };

      Assert.Throws<ConcurrencyException>(
        () => es.SaveEvents(aggregateId, eventsIn, 0));

    }

    [Test]
    public void MissingAggregateThrowsException()
    {
      var es = _kernel.Get<IEventStore>();
      Assert.Throws<AggregateNotFoundException>(
        () => es.GetEventsForAggregate(Guid.NewGuid()));
    }

  }
}
