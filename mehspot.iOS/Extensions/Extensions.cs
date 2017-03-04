using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace mehspot.iOS.Extensions
{
	public static class Extensions
	{
		public static DateTime NSDateToDateTime (this NSDate date)
		{
			// NSDate has a wider range than DateTime, so clip
			// the converted date to DateTime.Min|MaxValue.
			double secs = date.SecondsSinceReferenceDate;
			if (secs < -63113904000)
				return DateTime.MinValue;
			if (secs > 252423993599)
				return DateTime.MaxValue;
			return (DateTime)date;
		}

		public static NSDate DateTimeToNSDate (this DateTime date)
		{
			if (date.Kind == DateTimeKind.Unspecified)
				date = DateTime.SpecifyKind (date, DateTimeKind.Local);
			return (NSDate)date;
		}

        public static void SetProperty<TModel, TProperty> (this TModel model, Expression<Func<TModel, TProperty>> entityExpression, TProperty newValueEntity)
        {
            var memberExpression = (MemberExpression)entityExpression.Body;
            var property = (PropertyInfo)memberExpression.Member;

            property.SetValue (model, newValueEntity, null);
        }
	}
}

