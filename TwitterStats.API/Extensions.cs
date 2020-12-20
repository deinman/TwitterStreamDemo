using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TwitterStats.API
{
    public static class Extensions
    {
        public static MatchCollection GetEmoji(string tweet)
        {
            return Regex.Matches(tweet, Const.EmojiPattern);
        }

        public static MatchCollection GetHashtags(string tweet)
        {
            return Regex.Matches(tweet, Const.HashtagPattern);
        }

        public static Match GetDomain(string input)
        {
            return Regex.Match(input, Const.UrlPattern);
        }
        
        public static void AddManyToCountDict<T>(T enumerable, IDictionary<string, int> dict) where T : IEnumerable
        {
            foreach (var item in enumerable)
            {
                var matchString = item.ToString();
                if (matchString is null)
                {
                    continue;
                }

                AddSingleToCountDict(matchString, dict);
            }
        }
        
        public static void AddSingleToCountDict(string item, IDictionary<string, int> dict)
        {
            if (dict.ContainsKey(item))
            {
                dict[item]++;
            }
            else
            {
                dict.Add(item, 1);
            }
        }

        public static void OldAddManyToCountDict(IEnumerable matches, IDictionary<string, int> dict)
        {
            foreach (Match match in matches)
            {
                var matchString = match.ToString();

                if (dict.ContainsKey(matchString))
                {
                    dict[matchString]++;
                }
                else
                {
                    dict.Add(matchString, 1);
                }
            }
        }
    }
}