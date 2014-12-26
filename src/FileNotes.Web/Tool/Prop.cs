using System;
using System.Linq.Expressions;

namespace FileNotes.Web.Tool
{
    public static class Prop
    {
        public static string GetName(Expression<Func<object>> exp)
        {
            var body = exp.Body as MemberExpression;

            if (body == null)
            {
                var ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body.Member.Name;
        }
    }
}