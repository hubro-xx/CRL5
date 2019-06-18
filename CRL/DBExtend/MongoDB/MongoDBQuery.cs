/**
* CRL 快速开发框架 V5
* Copyright (c) 2019 Hubro All rights reserved.
* GitHub https://github.com/hubro-xx/CRL5
* 主页 http://www.cnblogs.com/hubro
* 在线文档 http://crl.changqidongli.com/
*/
using CRL.LambdaQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
namespace CRL.DBExtend.MongoDBEx
{
    /// <summary>
    /// MongoDB不支持关联和直接语句查询
    /// 部份扩展方法支持
    /// </summary>
    public sealed partial class MongoDBExt
    {
        List<dynamic> GetDynamicResult<TModel>(LambdaQuery.LambdaQuery<TModel> query1) where TModel : IModel, new()
        {
            var query = query1 as MongoDBLambdaQuery<TModel>;
            //var selectField = query.__QueryFields;
            var selectField = query._CurrentSelectFieldCache.mapping;
            var collection = _MongoDB.GetCollection<TModel>(query.QueryTableName);
            var pageIndex = query1.SkipPage-1;
            var pageSize = query1.TakeNum;
            var skip = 0;
            long rowNum = 0;
            if (query.TakeNum > 0)
            {
                skip = pageSize * pageIndex;
            }
            if (query.__GroupFields != null)
            {
                #region group
                var groupField = query.__GroupFields.FirstOrDefault();//只支持一个字段
                groupField.CheckNull("groupField");

                var groupInfo = new BsonDocument();
                groupInfo.Add("_id", "$" + groupField);
                foreach (var f in selectField)
                {
                    var method = f.MethodName.ToLower();
                    object sumField = 1;
                    if (method == "sum")
                    {
                        groupInfo.Add(f.ResultName, new BsonDocument("$sum", "$" + f.FieldName));
                    }
                    else if (method == "count")
                    {
                        groupInfo.Add(f.ResultName, new BsonDocument("$sum", 1));
                    }
                    else
                    {
                        throw new CRLException("不支持此方法" + method);
                    }
                }
                var aggregate = collection.Aggregate().Match(query.__MongoDBFilter).Group(groupInfo);
                if (query.TakeNum > 0)
                {
                    aggregate.Limit(pageSize);
                    if (skip > 0)
                    {
                        aggregate.Skip(skip);
                    }
                    rowNum = collection.Count(query.__MongoDBFilter);//todo 总行数
                }

                var result = aggregate.ToList();
                if (rowNum == 0)
                {
                    rowNum = result.Count();
                }
                var list = new List<dynamic>();
                foreach (var item in result)
                {
                    dynamic obj = new System.Dynamic.ExpandoObject();
                    var dict = obj as IDictionary<string, object>;
                    foreach (var f in selectField)
                    {
                        string columnName = f.ResultName;
                        object value = item[columnName];
                        dict.Add(columnName, value);
                    }
                    dict.Add(groupField.ResultName, item["_id"]);
                    //dict.Add(groupField, item["_id"]);
                    list.Add(obj);
                }
                return list;
                #endregion
            }
            else if (query.__DistinctFields)
            {
                #region distinct
                string fieldName = selectField.FirstOrDefault().ResultName;
                FieldDefinition<TModel, dynamic> distinctField = fieldName;
                var query2 = collection.Distinct(distinctField, query.__MongoDBFilter);
                return query2.ToList();
                #endregion
            }
            else
            {
                #region 动态类型
                var query2 = collection.Find(query.__MongoDBFilter);
                if (query.TakeNum > 0)
                {
                    query2.Limit(pageSize);
                    if (skip > 0)
                    {
                        query2.Skip(skip);
                    }
                }
                var result = query2.ToList();
                var list = new List<dynamic>();
                var fields = TypeCache.GetTable(typeof(TModel)).FieldsDic;
                foreach (var item in result)
                {
                    dynamic obj = new System.Dynamic.ExpandoObject();
                    var dict = obj as IDictionary<string, object>;
                    foreach (var f in selectField)
                    {
                        string columnName = f.ResultName;
                        object value = fields[columnName].GetValue(item);
                        dict.Add(columnName, value);
                    }
                    list.Add(obj);
                }
                #endregion
                return list;
            }
        }
        #region QueryDynamic
        public override List<dynamic> QueryDynamic(LambdaQuery.LambdaQueryBase query)
        {
            throw new NotSupportedException("MongoDB暂未实现此方法");
        }
        public List<dynamic> QueryDynamic<TModel>(LambdaQuery.LambdaQuery<TModel> query) where TModel : IModel, new()
        {
            var result = GetDynamicResult(query);
            return result;
        }
        #endregion

        #region QueryResult

        public override List<TResult> QueryResult<TResult>(LambdaQueryBase query)
        {
            throw new NotSupportedException("MongoDB暂未实现此方法");
        }
        public List<TResult> QueryResult<TModel, TResult>(LambdaQuery.LambdaQuery<TModel> query) where TModel : IModel, new()
        {
            var result = GetDynamicResult(query);
            var type = typeof(TResult);
            var pro = type.GetProperties();
            var list = new List<TResult>();
            var reflection = ReflectionHelper.GetInfo<TResult>();
            foreach (var item in result)
            {
                var dict = item as IDictionary<string, object>;
                var obj = (TResult)System.Activator.CreateInstance(type);
                foreach (var f in pro)
                {
                    string columnName = f.Name;
                    if (dict.ContainsKey(columnName))
                    {
                        object value = dict[columnName];
                        var access = reflection.GetAccessor(columnName);
                        access.Set((TResult)obj, value);
                    }
                }
                list.Add(obj);
            }
            return list;
        }


        public override List<TResult> QueryResult<TResult>(LambdaQuery.LambdaQueryBase query, NewExpression newExpression)
        {
            throw new NotSupportedException("MongoDB暂未实现此方法");
        }
        public List<TResult> QueryResult<TModel, TResult>(LambdaQuery.LambdaQuery<TModel> query, NewExpression newExpression) where TModel : IModel, new()
        {
            query.Select(newExpression);
            var result = GetDynamicResult(query);
            List<TResult> list = new List<TResult>();
            var par = Expression.Parameter(typeof(TModel), "b");
            var resultSelector = newExpression.ToLambda<Func<TModel, TResult>>(par);
            foreach (var item in result)
            {
                var obj = resultSelector.Compile()(item);
                list.Add(obj);
            }
            return list;
        }
        #endregion

        public override List<TModel> QueryOrFromCache<TModel>(LambdaQueryBase query1, out string cacheKey)
        {
            cacheKey = "none";
            var query = query1 as MongoDBLambdaQuery<TModel>;
            var collection = _MongoDB.GetCollection<TModel>(query.QueryTableName);
            long rowNum = 0;
            var query2 = collection.Find(query.__MongoDBFilter).Sort(query._MongoDBSort);
            if (query.TakeNum > 0)
            {
                var pageIndex = query1.SkipPage-1;
                var pageSize = query1.TakeNum;
                var skip = pageSize * pageIndex;
                query2.Limit(pageSize);
                if (skip > 0)
                {
                    query2.Skip(skip);
                }
                rowNum = collection.Count(query.__MongoDBFilter);
            }
            var result = query2.ToList();
            if (rowNum == 0)
            {
                rowNum = result.Count();
            }
            query.RowCount = (int)rowNum;
            SetOriginClone(result);
            return result;
        }
        public override Dictionary<TKey, TValue> ToDictionary<TModel, TKey, TValue>(LambdaQuery<TModel> query)
        {
            var dic = new Dictionary<TKey, TValue>();
            var result = GetDynamicResult(query);
            if (result.Count == 0)
            {
                return dic;
            }
            var first = result.First() as IDictionary<string, object>;
            var keys = first.Keys.ToList();
            var keyName = keys[0];
            var valueName = keys[1];
            foreach (var item in result)
            {
                var obj = item as IDictionary<string, object>;
                dic.Add((TKey)obj[keyName], (TValue)obj[valueName]);
            }
            return dic;
        }
        public override dynamic QueryScalar<TModel>(LambdaQuery<TModel> query)
        {
            var result = GetDynamicResult(query);
            if (result.Count == 0)
            {
                return null;
            }
            var first = result.First() as IDictionary<string, object>;
            var keys = first.Keys.ToList();
            return first[keys.First()];
        }
    }
}
