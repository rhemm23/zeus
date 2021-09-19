using System.Collections.Concurrent;
using System;

namespace Zeus {

  public static class ObjectBuilderCache {

    private static ConcurrentDictionary<Type, ObjectBuilder> cachedObjectBuilders;

    static ObjectBuilderCache() {
      cachedObjectBuilders = new ConcurrentDictionary<Type, ObjectBuilder>();
    }

    public static ObjectBuilder GetObjectBuilder(Type type) {
      if (cachedObjectBuilders.TryGetValue(type, out ObjectBuilder objectBuilder)) {
        return objectBuilder;
      } else {
        ObjectBuilder newObjectBuilder = new ObjectBuilder(type);
        cachedObjectBuilders.TryAdd(type, newObjectBuilder);
        return newObjectBuilder;
      }
    }
  }
}
