using System.Text.RegularExpressions;

namespace iRacingBusinessLayer
{
    public class ResolveNames
    {
        public static string[] IGNORE_WORDS = { "of", "at", "by", "from", "from" };
        private static List<string> MatchesByNumberOfWords(string text, List<string> list)
        {
            if(list.Count < 1) return new List<string>();

            string patternDeleteWords = @"\b(" + string.Join('|', IGNORE_WORDS) + @")\b";
            string patternWords = @"\b\w+\b";
            string textWithoutWords = Regex.Replace(text, patternDeleteWords, string.Empty);
            IEnumerable<string> words = Regex.Matches(textWithoutWords, patternWords)
                .OfType<Match>().Select(m => m.Value);
            List<KeyValuePair<string, int>> lineCoincidences = new List<KeyValuePair<string, int>>();

            foreach (string line in list)
            {
                int coincidences = 0;
                foreach (string word in words)
                {
                    coincidences += line.Contains(word)?1:0;
                }

                lineCoincidences.Add(new KeyValuePair<string, int>(line, coincidences));
            }

            int maxCoincidences = lineCoincidences.Max(x => x.Value);

            return lineCoincidences
                .Where(y => y.Value == maxCoincidences)
                .Select(x => x.Key)
                .ToList();
        }

        public static string GetVeryLikelyTrack(string track, List<string> tracksListOriginal)
        {
            string patternWords = @"\b\w+\b";
            IEnumerable<string> words = Regex.Matches(track, patternWords)
                .OfType<Match>().Select(m => m.Value);
            string trackOnlyWordsAndNoSpaces = string.Join(string.Empty, words);

            return tracksListOriginal.FirstOrDefault(x => {
                IEnumerable<string> wordsX = Regex.Matches(x, patternWords)
                .OfType<Match>().Select(m => m.Value);
                string onlyWordsAndNoSpaces = string.Join(string.Empty, wordsX);
                bool found = trackOnlyWordsAndNoSpaces.ToUpper().StartsWith(x.ToUpper());

                if (!found)
                {
                    string trackTemp = Regex.Replace(trackOnlyWordsAndNoSpaces, @"^Legacy ", string.Empty);
                    found = trackTemp.ToUpper().StartsWith(onlyWordsAndNoSpaces.ToUpper());
                    if (!found)
                    {
                        found = trackTemp.Contains(onlyWordsAndNoSpaces) 
                        || onlyWordsAndNoSpaces.Contains(trackTemp);
                    }
                }

                return found;
            }, string.Empty);
        }

        public static List<string> GetLikelyTracks(string track, List<string> tracksListOriginal)
        {
            string name = GetVeryLikelyTrack(track, tracksListOriginal);

            if (name == string.Empty)
            {
                return MatchesByNumberOfWords(track, tracksListOriginal);
            }
            
            return (new List<string>() { name });
        }

        public static List<string> GetLikelyCars(string car, List<string> carsListOriginal)
        {
            return MatchesByNumberOfWords(car, carsListOriginal);
        }
    }
}
