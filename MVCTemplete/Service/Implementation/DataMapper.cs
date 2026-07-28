using MVCTemplete.Service.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

public class DataMapper : IDataMapper
{
    public T Map<T>(DataRow row) where T : new()
    {
        T obj = new T();

        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            if (!row.Table.Columns.Contains(prop.Name))continue;

            if (row[prop.Name] == DBNull.Value) continue;

            var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            prop.SetValue(obj,Convert.ChangeType(row[prop.Name], type));
        }

        return obj;
    }

    public List<T> MapList<T>(DataTable table) where T : new()
    {
        return table.AsEnumerable().Select(Map<T>) .ToList();
    }
}