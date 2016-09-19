using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Datapecker.Agent
{
    internal static class ReflectionExtensions
    {
        public static string GetPropertyName<T>(this T obj, Expression<Func<object>> propertyLambda)
        {
            MemberExpression body = propertyLambda.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)propertyLambda.Body;
                body = ubody.Operand as MemberExpression;
            }

            if (body.Member == null)
            {
                throw new ArgumentException("The parameter propertyLambda must be a member accessing lambda such as () => MyClass.Id", "propertyLambda");
            }

            return body.Member.Name;
        }

        public static string GetPropertyName(Expression<Func<object>> propertyLambda)
        {
            MemberExpression body = propertyLambda.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)propertyLambda.Body;
                body = ubody.Operand as MemberExpression;
            }

            if (body.Member == null)
            {
                throw new ArgumentException("The parameter propertyLambda must be a member accessing lambda such as () => MyClass.Id", "propertyLambda");
            }

            return body.Member.Name;
        }
        
        public static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> propertyLambda)
        {
            var member = propertyLambda.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException("The parameter propertyLambda must be a member accessing lambda such as x => x.Id", "propertyLambda");
            }
            return member.Member.Name;
        }
    }

}
