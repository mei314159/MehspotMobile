using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Mehspot.iOS.Extensions
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
            object targetObject = model;
            var memberExpression = (MemberExpression)entityExpression.Body;
            var targetExpression = memberExpression;
            while (targetExpression.Expression.NodeType == ExpressionType.MemberAccess) {
                targetExpression = (MemberExpression)targetExpression.Expression;
                targetObject = ((PropertyInfo)targetExpression.Member).GetValue (targetObject);
            }


            var property = (PropertyInfo)memberExpression.Member;
            property.SetValue (targetObject, newValueEntity, null);
        }

        public static int Count (this IEnumerable source)
        {
            var col = source as ICollection;
            if (col != null)
                return col.Count;

            int c = 0;
            var e = source.GetEnumerator ();
            while (e.MoveNext ())
                c++;
            return c;
        }

        public static UIColor FromHexString (this UIColor color, string hexValue, float alpha = 1.0f)
        {
            var colorString = hexValue.Replace ("#", "");
            if (alpha > 1.0f) {
                alpha = 1.0f;
            } else if (alpha < 0.0f) {
                alpha = 0.0f;
            }

            float red, green, blue;

            switch (colorString.Length) {
            case 3: // #RGB
            {
                    red = Convert.ToInt32 (string.Format ("{0}{0}", colorString.Substring (0, 1)), 16) / 255f;
                    green = Convert.ToInt32 (string.Format ("{0}{0}", colorString.Substring (1, 1)), 16) / 255f;
                    blue = Convert.ToInt32 (string.Format ("{0}{0}", colorString.Substring (2, 1)), 16) / 255f;
                    return UIColor.FromRGBA (red, green, blue, alpha);
                }
            case 6: // #RRGGBB
            {
                    red = Convert.ToInt32 (colorString.Substring (0, 2), 16) / 255f;
                    green = Convert.ToInt32 (colorString.Substring (2, 2), 16) / 255f;
                    blue = Convert.ToInt32 (colorString.Substring (4, 2), 16) / 255f;
                    return UIColor.FromRGBA (red, green, blue, alpha);
                }

            default:
                throw new ArgumentOutOfRangeException (string.Format ("Invalid color value {0} is invalid. It should be a hex value of the form #RBG, #RRGGBB", hexValue));

            }
        }
    }
}

