using System;

namespace Sibusten.Philomena.Downloader.Utility
{
    public static class FluentExtensions
    {
        /// <summary>
        /// Performs an action if a condition is true
        /// </summary>
        /// <param name="obj">The object the action is performed on</param>
        /// <param name="condition">The condition</param>
        /// <param name="actionIfTrue">The action to perform if the condition is true</param>
        /// <typeparam name="T">The type of object</typeparam>
        /// <returns>The object with action applied if the condition is true, otherwise the original object</returns>
        public static T If<T>(this T obj, bool condition, Func<T, T> actionIfTrue)
        {
            return condition ? actionIfTrue(obj) : obj;
        }
    }
}
