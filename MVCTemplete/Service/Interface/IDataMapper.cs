
using System.Collections.Generic;
using System.Data;


namespace MVCTemplete.Service.Interface
{
    public interface IDataMapper
    {
        T Map<T>(DataRow row) where T : new();

        List<T> MapList<T>(DataTable table) where T : new();
    }
}