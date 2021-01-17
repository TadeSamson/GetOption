using MongoDB.Driver;
using GetOption.Core.Utils;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace GetOption.Infrastructure.Utils
{
    internal class GetOptionToDocumentQuery<T>
    {
        public ProjectionDefinition<T,T> ProjectionDefinition { get; }
        public FilterDefinition<T> FilterDefinition { get; }
        public SortDefinition<T> SortDefinition { get; }
        public PaginationDefinition PaginationDefinition { get; }

        public GetOptionToDocumentQuery(GetOption<T> option)
        {

            TypeInfo _typeInfo = typeof(T).GetTypeInfo();
            var propertyInfo = _typeInfo.GetProperty("Id");
            this.ProjectionDefinition = GetLoadQuery(option.LoadedPropertyOption);
            this.FilterDefinition = GetSearchQuery(option.SearchOption?.Expression);
            this.SortDefinition = GetSortQuery(option.SortOption);
            this.PaginationDefinition = GetPaginationQuery(option.PaginationOption);

            


        }

        ProjectionDefinition<T> GetLoadQuery(LoadedPropertyOption<T> loadedPropertyOption)
        {
            if (loadedPropertyOption == null || loadedPropertyOption.LoadedProperties.Count == 0)
            {
                return "{ nothing: 0}";
            }
            if(loadedPropertyOption.LoadedProperties.FirstOrDefault(lp => lp.Trim().ToLower() == "id")!=null)
            {
                loadedPropertyOption.LoadedProperties.Remove(loadedPropertyOption.LoadedProperties.FirstOrDefault(lp => lp.Trim().ToLower() == "id"));
                loadedPropertyOption.LoadedProperties.Add("_id");
            }

            String projectionDefinition = "";
            loadedPropertyOption.LoadedProperties.ForEach(p => { projectionDefinition=projectionDefinition + p + ":1,"; });
            projectionDefinition = projectionDefinition.Remove(projectionDefinition.LastIndexOf(','));
            projectionDefinition = "{" + projectionDefinition + "}";
            return projectionDefinition;
        }


        FilterDefinition<T> GetSearchQuery(SearchExpression searchExpression)
        {
            try
            {
                if (searchExpression == null)
                {
                    return FilterDefinition<T>.Empty;
                }

            if (searchExpression is PropertySearchExpression)
            {
                var builder = Builders<T>.Filter;
                // This is necessary to match properties of the format ProcessITTOs.ITTOId. 
                //ProcessITTOs is the main property but MongoDB filter allows projection into properties of objects or arrays.
                string property = (searchExpression as PropertySearchExpression).Property.Split('.')[0].Trim();
                PropertyInfo info = typeof(T).GetProperties().FirstOrDefault(p => p.Name.Trim() == property);
                if (info != null)
                {
                    PropertySearchExpression propertySearchExpression = searchExpression as PropertySearchExpression;
                    propertySearchExpression.Property = propertySearchExpression.Property?.Trim().ToLower() == "id" ? "_id" : propertySearchExpression.Property;
                    return GenerateFilterConditionByType(info.PropertyType, propertySearchExpression);
                }
                return FilterDefinition<T>.Empty;

            }
            else
            {
                var expression = searchExpression as BinarySearchExpression<T>;
                return ToDocumentBinary(GetSearchQuery(expression.LeftSearch),
                    expression.BinaryOperator,
                    GetSearchQuery(expression.RightSearch));
            }
            }
            catch(Exception ex)
            {
                return FilterDefinition<T>.Empty;
            }

        }

        FilterDefinition<T> ToDocumentFilter<PropertyType>(string op,string property,PropertyType value)
        {
            switch (op)
            {
                case "==": return Builders<T>.Filter.Eq(property, value);
                case "=": return Builders<T>.Filter.Eq(property, value);
                case "!=": return  Builders<T>.Filter.Ne(property, value);
                case ">=": return Builders<T>.Filter.Gte(property, value);
                case "%": return Builders<T>.Filter.Regex(property, new MongoDB.Bson.BsonRegularExpression($".*{value as string}.*","i"));
                case "<=": return Builders<T>.Filter.Lte(property, value);
                case "<": return Builders<T>.Filter.Lt(property, value);
                case ">": return Builders<T>.Filter.Gt(property, value);
                default: return Builders<T>.Filter.Eq(property, value);
            }
        }

        FilterDefinition<T> ToDocumentBinary(FilterDefinition<T> left, string op, FilterDefinition<T> right)
        {
            switch (op.Trim().ToLower())
            {
                case "or": return Builders<T>.Filter.Or(new FilterDefinition<T>[] {left,right });
                case "and": return Builders<T>.Filter.And(new FilterDefinition<T>[] { left, right });
                case "&&": return Builders<T>.Filter.And(new FilterDefinition<T>[] { left, right });
                case "&": return Builders<T>.Filter.And(new FilterDefinition<T>[] { left, right });
                case "||": return Builders<T>.Filter.Or(new FilterDefinition<T>[] { left, right });
                case "|": return Builders<T>.Filter.Or(new FilterDefinition<T>[] { left, right });
                default: return Builders<T>.Filter.Or(new FilterDefinition<T>[] { left, right });
            }
        }

        FilterDefinition<T> GenerateFilterConditionByType(Type type, PropertySearchExpression propertyExpression)
        {
            if(type == typeof(bool) || type==typeof(bool?))
            return ToDocumentFilter(propertyExpression.Operator, propertyExpression.Property, Convert.ToBoolean( propertyExpression.Value));
            if (type == typeof(int) || type==typeof(int?))
                return ToDocumentFilter<int>(propertyExpression.Operator,propertyExpression.Property, Convert.ToInt32(propertyExpression.Value));
            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return ToDocumentFilter(propertyExpression.Operator,propertyExpression.Property, DateTime.Parse(propertyExpression.Value));
            else
                return ToDocumentFilter(propertyExpression.Operator,propertyExpression.Property, propertyExpression.Value);
        }


        PaginationDefinition GetPaginationQuery(PaginationOption<T> paginationOption)
        {
            if (paginationOption == null || paginationOption.PageSize==0)
            {
                return new PaginationDefinition(null, null);
            }
            return new PaginationDefinition( paginationOption.PageNo * paginationOption.PageSize, paginationOption.PageSize );
        }

        SortDefinition<T> GetSortQuery(SortOption<T> sortOption)
        {
            if (sortOption == null || sortOption.Property == String.Empty)
                return null;
            String sortDefinition = "";
            sortDefinition = "{" + sortOption.Property + ":"+(sortOption.Ascending?1:-1)+"}";
            return sortDefinition;
        }
    }

   
     internal class PaginationDefinition
    {
        public PaginationDefinition(int? skip, int? limit)
        {
            this.skip = skip;
            this.limit = limit;
        }
        public int? skip;
        public int? limit;
    }
 

    

}
