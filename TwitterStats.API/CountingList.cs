using System.Collections.Generic;

namespace TwitterStats.API
{
    public class CountingList
    {
        private Dictionary<string, int> countingList = new Dictionary<string, int>();

        public void Add(string s)
        {
            if (countingList.ContainsKey(s))
            {
                countingList[s]++;
            }
            else
            {
                countingList.Add(s, 1);
            }
        }
    }
}