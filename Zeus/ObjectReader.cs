using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace Zeus {

  public class ObjectReader {

    private SqlDataReader _dataReader;
    private Type _dataType;

    public ObjectReader(SqlDataReader dataReader, Type dataType) {
      this._dataReader = dataReader;
      this._dataType = dataType;
    }

    public IEnumerable<object> ReadAllObjects() {
      ObjectBuilder objectBuilder = ObjectBuilderCache.GetObjectBuilder(this._dataType);
      while (this._dataReader.Read()) {
        yield return objectBuilder.InitializeObjectFromDataRecord(this._dataReader);
      }
      this._dataReader.Close();
    }

    public object ReadObject() {
      ObjectBuilder objectBuilder = ObjectBuilderCache.GetObjectBuilder(this._dataType);
      if (this._dataReader.Read()) {
        object result = objectBuilder.InitializeObjectFromDataRecord(this._dataReader);
        this._dataReader.Close();
        return result;
      } else {
        this._dataReader.Close();
        return null;
      }
    }
  }
}
