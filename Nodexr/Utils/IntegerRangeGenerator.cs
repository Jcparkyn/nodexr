//Algorithm based on answers in https://stackoverflow.com/questions/33512037/a-regular-expression-generator-for-number-ranges

namespace Nodexr.Utils;
using System.Text;

public class IntegerRangeGenerator
{
    private readonly struct Pair
    {
        public readonly int min;
        public readonly int max;

        public Pair(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public Pair Flipped()
        {
            return new Pair(max, min);
        }
    }

    public List<string> GenerateRegexRange(int start, int end)
    {
        if (start < 0)
            throw new ArgumentOutOfRangeException(nameof(start), "Must be non-negative");
        if (end < 0)
            throw new ArgumentOutOfRangeException(nameof(end), "Must be non-negative");

        //order inputs so start <= end
        if (start > end)
            (start, end) = (end, start);

        return GenerateRegexRanges(start, end);
    }

    private List<string> GenerateRegexRanges(int start, int end)
    {
        var pairs = GetRegexPairsRecursion(start, end);
        return FormatPairsToRegEx(pairs);
    }

    /**
     * return the regular expressions that match the ranges in the given
     * list of integers. The list is in the form firstRangeStart, firstRangeEnd,
     * secondRangeStart, secondRangeEnd, etc. Each regular expression is 0-left-padded,
     * if necessary, to match strings of the given width.
     */
    private static List<string> FormatPairsToRegEx(List<Pair> pairs)
    {
        List<string> list = new List<string>();
        for (int i = 0; i < pairs.Count; i++)
        {
            string start = pairs[i].min.ToString();
            string end = pairs[i].max.ToString();

            StringBuilder result = new StringBuilder();

            for (int pos = 0; pos < start.Length; pos++)
            {
                if (start[pos] == end[pos])
                {
                    result.Append(start[pos]);
                }
                else if (start[pos] == '0' && end[pos] == '9')
                {
                    result.Append("\\d");
                }
                else
                {
                    result.Append('[').Append(start[pos]).Append('-')
                        .Append(end[pos]).Append(']');
                }
            }

            list.Add(result.ToString());
        }
        return list;
    }

    /**
     * return the list of integers that are the paired integers
     * used to generate the regular expressions for the given
     * range. Each pair of integers in the list -- 0,1, then 2,3,
     * etc., represents a range for which a single regular expression
     * is generated.
     */
    private List<Pair> GetRegexPairsRecursion(int start, int end)
    {
        var pairs = new List<Pair>();

        if (start == 0)
        {
            if (end <= 9)
            {
                pairs.Add(new Pair(0, end));
                return pairs;
            }
            else
            {
                pairs.Add(new Pair(0, 9));
                start = 10;
            }
        }

        if (start > end)
        {
            return pairs;
        }

        /*
         * Calculate first number ending with 0, which is greater than the start value.
         * This will tell us whether or not start and end values differ only at last digit.
         */
        int firstEndingWith0 = 10 * ((start + 9) / 10);

        /*
         * Start and end values differ only at the last digit.
         */
        if (firstEndingWith0 > end) // not in range?
        {
            pairs.Add(new Pair(start, end));
            return pairs;
        }

        /*
         * start is not ending in 0.
         */
        if (start < firstEndingWith0)
        {
            pairs.Add(new Pair(start, firstEndingWith0 - 1));
        }

        //Largest number ending with 9, which is <= the Range end.
        int lastEndingWith9 = ((end + 1) / 10 * 10) - 1;

        /*
         *  All RegEx for the range [firstEndingWith0,lastEndingWith9] end with [0-9],
         *  hence, remove the rightmost 0 from new working range start and remove the rightmost
         *  9 from new working range end.
         */
        var pairsMiddle = GetRegexPairsRecursion(firstEndingWith0 / 10, lastEndingWith9 / 10);

        /*
         * Append digits to start and end of each pair. 0 will be appended to the low value of
         * the pair and 9 will be appended to the high value of the pair.
         * This is equivalent of multiplying low value by 10, and multiplying high value by 10
         * and adding 9 to it.
         */
        foreach (Pair pair in pairsMiddle)
        {
            pairs.Add(new Pair(
                (pair.min * 10) + 0,
                (pair.max * 10) + 9
                ));
        }

        if (lastEndingWith9 < end) // end is not ending in 9
        {
            pairs.Add(new Pair(
                lastEndingWith9 + 1,
                end
                ));
        }

        return pairs;
    }
}
