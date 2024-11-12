using System.Linq.Expressions;
using System.Reflection;
using LinqKit;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Libs.Filters;


public abstract class GenericFilter<T>
{
    public abstract IQueryable<V> ApplyFilter<V>(IQueryable<V> query, Expression<Func<V, T>> selector);
 public virtual Expression<Func<V, bool>> GetLambda<V>(Expression<Func<V, T>> selector, T? value, MethodInfo methodInfo, bool? not = false)
    {
        var param = selector.Parameters[0];
        Expression body = Expression.Call(selector.Body, methodInfo, Expression.Constant(value));
        if (not == true)
        {
            body = Expression.Not(body);
        }
        return Expression.Lambda<Func<V, bool>>(body, param);
    }
  public virtual Expression<Func<V, bool>> GetLambda<V>(Expression<Func<V, T>> selector, T? value, string methodName, bool? not = false)
    {
        var param = selector.Parameters[0];
        Expression body = Expression.Call(selector.Body, methodName, Type.EmptyTypes, Expression.Constant(value));
        if (not == true)
        {
            body = Expression.Not(body);
        }
        return Expression.Lambda<Func<V, bool>>(body, param);
    }

    public virtual Expression<Func<V, bool>> GetLambda<V>(Expression<Func<V, T>> selector, string methodName, IEnumerable<T> value, bool? not = false)
    {

        var param = selector.Parameters[0];

        Expression body = Expression.Call(
            typeof(Enumerable),
            "Contains",
            [typeof(T)],
            Expression.Constant(value),
            selector.Body
        );
        if (not == true)
        {
            body = Expression.Not(body);
        }
        return Expression.Lambda<Func<V, bool>>(body, param);

    }
}






public class StringFilter : GenericFilter<string>
{
    public string? Contains { get; set; }
    public string? NotContains { get; set; }
    public string? Equal { get; set; }
    public string? NotEquals { get; set; }
    public string? StartsWith { get; set; }
    public string? EndsWith { get; set; }

    public bool? IsNull { get; set; }
    public bool? IsNotNull { get; set; }
    public IEnumerable<string>? In { get; set; }
    public override IQueryable<V> ApplyFilter<V>(IQueryable<V> query, Expression<Func<V, string>> selector)
    {
        if (Contains != null)
        {
            MethodInfo ContainsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
            query = query.Where(this.GetLambda(selector, Contains, ContainsMethod));
        }
        if (NotContains != null)
        {
            MethodInfo ContainsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
            query = query.Where(this.GetLambda(selector, NotContains, ContainsMethod, true));
        }

        if (Equal != null)
        {
            var methodInfo = typeof(string).GetMethod(nameof(string.Equals), [typeof(string)])!;
            query = query.Where(this.GetLambda(selector, Equal, methodInfo));
        }
        if (StartsWith != null)
        {
            query = query.Where(this.GetLambda(selector, StartsWith, nameof(string.StartsWith)));
        }
        if (EndsWith != null)
        {
            query = query.Where(this.GetLambda(selector, EndsWith, nameof(string.EndsWith)));
        }
        if (IsNull == true)
        {
            query = query.Where(this.GetLambda(selector, null, nameof(string.Equals)));
        }
        if (IsNotNull == true)
        {
            query = query.Where(this.GetLambda(selector, null, nameof(string.Equals), true));
        }


        if (In != null)
        {
            query = query.Where(this.GetLambda(selector, "Contains", In));
        }

        return query;
    }
}

// int or double or float or decimal or long or short or byte or sbyte


public class NumberFilter<T> : GenericFilter<T> where T : struct, IComparable<T>, IEquatable<T>, IComparable
{
    public T? Equal { get; set; }
    public T? NotEquals { get; set; }

    public T? GreaterThan { get; set; }
    public T? GreaterThanOrEqual { get; set; }
    public T? LessThan { get; set; }
    public T? LessThanOrEqual { get; set; }

    public IEnumerable<T>? In { get; set; }

    public bool? IsNull { get; set; }
    public bool? IsNotNull { get; set; }

    public override IQueryable<V> ApplyFilter<V>(IQueryable<V> query, Expression<Func<V, T>> property)
    {
        if (Equal != null)
        {
            query = query.Where(this.GetLambda(property, Equal.Value, nameof(Equals)));
        }
        if (NotEquals != null)
        {
            query = query.Where(this.GetLambda(property, NotEquals.Value, nameof(Equals), true));
        }
        if (GreaterThan != null)
        {
            query = query.Where(Expression.Lambda<Func<V, bool>>(Expression.GreaterThan(property.Body, Expression.Constant(GreaterThan.Value)), property.Parameters));
        }
        if (GreaterThanOrEqual != null)
        {
            query = query.Where(Expression.Lambda<Func<V, bool>>(Expression.GreaterThanOrEqual(property.Body, Expression.Constant(GreaterThanOrEqual.Value)), property.Parameters));
        }
        if (LessThan != null)
        {
            query = query.Where(Expression.Lambda<Func<V, bool>>(Expression.LessThan(property.Body, Expression.Constant(LessThan.Value)), property.Parameters));
        }
        if (LessThanOrEqual != null)
        {
            query = query.Where(Expression.Lambda<Func<V, bool>>(Expression.LessThanOrEqual(property.Body, Expression.Constant(LessThanOrEqual.Value)), property.Parameters));
        }
        if (IsNull == true)
        {
            query = query.Where(this.GetLambda(property, default(T), nameof(Equals)));
        }
        if (IsNotNull == true)
        {
            query = query.Where(this.GetLambda(property, default(T), nameof(Equals), true));
        }
        if (In != null)
        {
            query = query.Where(this.GetLambda(property, "Contains", In));
        }

        return query;
    }

}