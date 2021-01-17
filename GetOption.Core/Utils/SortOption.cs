using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GetOption.Core.Utils
{

    /// <summary>
    /// represent a class passed into an api  instance which represents how the current api request should be sorted
    /// </summary>
    public class SortOption<T>
    {
        /// <summary>
        /// Gets or sets the property to perform the target sorting on.
        /// </summary>
        /// <value>
        /// The string property value.
        /// </value>
        public string Property { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SortParams"/> is ascending.
        /// </summary>
        /// <value>
        ///   <c>true</c> if ascending; otherwise, <c>false</c>.
        /// </value>
        public bool Ascending { get; set; }

        public SortOption<T> ToPascal()
        {
            SortOption<T> sortOption = new SortOption<T>();
                var asList = this.Property.Split(new char['_']).ToList();
                asList.ForEach(a => a = a.First().ToString().ToUpper() + a.Substring(1, a.Length - 1));
                sortOption.Property = string.Join("", asList);
                sortOption.Ascending = this.Ascending;
                return sortOption;
        }

        public List<T> Sort(List<T> TList)
        {
            if (this.Property != null && TList?.Count > 1)
            {
                TList.Sort(new ObjectComparer<T>(this.Property, this.Ascending));
            }
            return TList;
        }

    }




    public class ObjectComparer<T> : IComparer<T>
    {

        string targetPropertyName;
        bool ascending;
        public ObjectComparer(string _targetPropertyName, bool _ascending)
        {
            this.targetPropertyName = _targetPropertyName;
            this.ascending = _ascending;
        }
        public int Compare(T x, T y)
        {
            PropertyInfo property = typeof(T).GetProperties().ToList().Find(p => p.Name.ToLower().Trim() == this.targetPropertyName.ToLower().Trim());
            if (property != null)
            {
                object xValue = property.GetValue(x);
                object yValue = property.GetValue(y);
                if (property.PropertyType.IsPrimitive &&
                    property.PropertyType != typeof(bool) &&
                    property.PropertyType != typeof(char) &&
                    property.PropertyType != typeof(UIntPtr) &&
                    property.PropertyType != typeof(IntPtr))
                {
                    return ascending ? (int)xValue - (int)yValue : (int)yValue - (int)xValue;
                }

                if (property.PropertyType == typeof(DateTime))
                {
                    return ascending ? (int)((((DateTime)xValue).Ticks - ((DateTime)yValue).Ticks) >> 32 / sizeof(int))
                        :
                        (int)((((DateTime)yValue).Ticks - ((DateTime)xValue).Ticks) >> 32 / sizeof(int));
                }
            }
            throw new Exception(targetPropertyName + " not a valid property of " + this.GetType().Name);
        }
    }
}
