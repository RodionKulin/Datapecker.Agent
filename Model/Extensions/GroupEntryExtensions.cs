using Datapecker.Agent.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Datapecker.Agent
{
    public static class GroupEntryExtensions
    {
        //Convert from GroupEntry
        public static TClass ToStronglyTyped<TClass>(this IEnumerable<GroupEntry> data
             , string groupName, ICaller caller, TClass defaultsSource = null)
            where TClass : class, new()
        {
            Type targetType = typeof(TClass);
            PropertyInfo[] targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            TClass target = Activator.CreateInstance<TClass>();            
            
            Dictionary<string, GroupEntry> groupData =
                data.Where(p => p.GroupName == groupName).ToDictionary(p => p.Key);

            foreach (PropertyInfo prop in targetProperties)
            {
                string entryValue = groupData.ContainsKey(prop.Name)
                    ? groupData[prop.Name].Value
                    : null;

                object defaultValue = defaultsSource == null
                    ? null
                    : prop.GetValue(defaultsSource);

                object valueTyped = entryValue == null
                    ? defaultValue
                    : ConvertValue<TClass>(prop, entryValue, caller);

                if (valueTyped != null)
                {
                    SetValue(target, prop, valueTyped, caller);
                }
            }
            
            return target;
        }

        private static object ConvertValue<TClass>(PropertyInfo prop, string value, ICaller caller)
        {
            object valueTyped = null;

            try
            {
                valueTyped = TypeDescriptor.GetConverter(prop.PropertyType)
                    .ConvertFromInvariantString(value);
            }
            catch (Exception exception)
            {
                valueTyped = GetDefaultValue(prop.PropertyType);

                Type targetType = typeof(TClass);

                if (caller != null)
                {
                    caller.Exception(exception, MessageResources.GroupEntryExtensions_ValueConvertException
                        , prop.Name, targetType.Name);
                }
            }

            return valueTyped;
        }

        private static void SetValue<TClass>(TClass target, PropertyInfo prop, object valueTyped, ICaller caller)
            where TClass : class
        {
            try
            {
                prop.SetValue(target, valueTyped);
            }
            catch (Exception exception)
            {
                Type targetType = typeof(TClass);

                if (caller != null)
                {
                    caller.Exception(exception, MessageResources.GroupEntryExtensions_ValueSetException
                        , prop.Name, targetType.Name);
                }
            }
        }

        private static object GetDefaultValue(Type type)
        {
            if (type.IsValueType && Nullable.GetUnderlyingType(type) == null)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }


        
        //Convert to GroupEntry
        public static List<GroupEntry> ToGroupEntries<TClass>(this TClass target, string groupName)
        {
            Type targetType = typeof(TClass);
            PropertyInfo[] targetProperties = targetType.GetProperties();

            List<GroupEntry> data = new List<GroupEntry>();

            foreach (PropertyInfo prop in targetProperties)
            {
                object valueTyped = prop.GetValue(target);
                                
                data.Add(new GroupEntry()
                {
                    Key = prop.Name,
                    Value = valueTyped.ToString(),
                    GroupName = groupName
                });
            }

            return data;
        }

        public static GroupEntry ToGroupEntry<TClass, TReturn>(Expression<Func<TClass, TReturn>> property, object value, string groupName)
        {
            return new GroupEntry()
            {
                Key = ReflectionExtensions.GetPropertyName<TClass, TReturn>(property),
                Value = value.ToString(),
                GroupName = groupName
            };
        }



        //Merge
        public static void MergeUpsert(this List<GroupEntry> target, IEnumerable<GroupEntry> source)
        {
            foreach (GroupEntry entry in source)
            {
                target.RemoveAll(p => p.GroupName == entry.GroupName
                        && p.Key == entry.Key);

                target.Add(entry);
            }
        }

        public static void MergeUpsert<TClass>(this List<GroupEntry> target, TClass source, string groupName)            
        {
            List<GroupEntry> groupEntries = ToGroupEntries(source, groupName);
            MergeUpsert(target, groupEntries);
        }
    }
}
