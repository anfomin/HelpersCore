using System.Linq.Expressions;
using System.Reflection;

namespace HelpersCore;

/// <summary>
/// Provides extensions for <see cref="Expression"/>.
/// </summary>
public static class ExpressionExtensions
{
	extension(Expression)
	{
		/// <summary>
		/// Returns <see cref="MemberInfo"/> if <paramref name="expression"/> is member access.
		/// Supports conversion and as-operator unwrapping.
		/// </summary>
		/// <param name="expression">Expression to get member for.</param>
		public static MemberInfo? GetMember(Expression expression)
		{
			if (expression is LambdaExpression lambdaExpression)
				expression = lambdaExpression.Body;
			return expression switch
			{
				MemberExpression memberExpr => memberExpr.Member,
				UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.TypeAs, Operand: MemberExpression memberInner } => memberInner.Member,
				_ => null
			};
		}
	}
}