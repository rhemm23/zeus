using System.Collections.Generic;
using ZeusMigrations.Actions;
using System;

namespace ZeusMigrations {

  public abstract class Migration {

    public abstract Guid Id { get; }

    public abstract IEnumerable<IAction> Actions();
  }
}
