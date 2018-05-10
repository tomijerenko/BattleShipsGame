using System;
using System.Linq;
using System.Reflection;

namespace BattleShip.Helpers
{
    //By Dipon Roy : https://www.codeproject.com/Tips/807820/Simple-Model-Entity-Mapper-in-Csharp
    public static class MapperUtility
    {
        public static TTarget MapTo<TSource, TTarget>(this TSource aSource, TTarget aTarget)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            var srcFields = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
                             where aProp.CanRead
                             select new
                             {
                                 Name = aProp.Name,
                                 Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType
                             }).ToList();
            var trgFields = (from PropertyInfo aProp in aTarget.GetType().GetProperties(flags)
                             where aProp.CanWrite
                             select new
                             {
                                 Name = aProp.Name,
                                 Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType
                             }).ToList();

            var commonFields = srcFields.Intersect(trgFields).ToList();

            foreach (var aField in commonFields)
            {
                var value = aSource.GetType().GetProperty(aField.Name).GetValue(aSource, null);
                PropertyInfo propertyInfos = aTarget.GetType().GetProperty(aField.Name);
                propertyInfos.SetValue(aTarget, value, null);
            }
            return aTarget;
        }

        public static TTarget CreateMapped<TSource, TTarget>(this TSource aSource) where TTarget : new()
        {
            return aSource.MapTo(new TTarget());
        }
    }
}
