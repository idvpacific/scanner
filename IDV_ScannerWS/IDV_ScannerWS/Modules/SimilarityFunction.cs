using Accord.Video;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace IDV_ScannerWS.Modules
{
    public class SimilarityFunction
    {
        //----------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------
        private int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;
            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;
            if (sourceWordCount == 0) { return targetWordCount; }
            if (targetWordCount == 0) { return sourceWordCount; }
            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }
            return distance[sourceWordCount, targetWordCount];
        }
        private double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;
            int stepsToSame = ComputeLevenshteinDistance(source, target);
            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }
        public int GetWordsSimilarity(string Word1, string Word2)
        {
            try
            {
                return (int)(Math.Round(CalculateSimilarity(Word1, Word2) * 100));
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public int GetWordsSimilarityInString(string Word1, string FullText)
        {
            try
            {
                int MaxSimilarityPercent = 0;
                string[] WordArray = FullText.Split(' ');
                for (int i = 0; i < WordArray.Length; i++)
                {
                    int LocalSimilarityPercent = (int)(Math.Round(CalculateSimilarity(Word1.ToUpper(), WordArray[i].ToUpper()) * 100));
                    if (LocalSimilarityPercent > MaxSimilarityPercent) { MaxSimilarityPercent = LocalSimilarityPercent; }
                }
                return MaxSimilarityPercent;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //----------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------
        public int GetColorSimilarity(Color Color1, Color Color2)
        {
            try
            {
                int diffRed = Math.Abs(Color1.R - Color2.R);
                int diffGreen = Math.Abs(Color1.G - Color2.G);
                int diffBlue = Math.Abs(Color1.B - Color2.B);
                float pctDiffRed = (float)diffRed / 255;
                float pctDiffGreen = (float)diffGreen / 255;
                float pctDiffBlue = (float)diffBlue / 255;
                return (int)(100 - Math.Round((pctDiffRed + pctDiffGreen + pctDiffBlue) / 3 * 100));
            }
            catch (Exception)
            {
                return 0;
            }
        }
        //----------------------------------------------------------------------------------------
        //----------------------------------------------------------------------------------------
        public bool KeyDetector(ref int X, ref int Y, string Key, IList<Line> Lines, string Similarity, int KeyIndex)
        {
            try
            {
                bool resTF = false;
                X = -1; Y = -1;
                Key = Key.ToLower().Trim();
                if (KeyIndex <= 0) { KeyIndex = 1; }
                Similarity = Similarity.Trim();
                int SimilarityInt = int.Parse(Similarity);
                int KeyFounded = 0;
                Line SelectedLineKey = null;
                foreach (Line LN in Lines)
                {
                    foreach (Word WD in LN.Words)
                    {
                        if ((int)(Math.Round(CalculateSimilarity(Key.ToUpper(), WD.WordText.Trim().ToUpper()) * 100)) >= SimilarityInt)
                        {
                            KeyFounded++;
                            if (KeyFounded >= KeyIndex)
                            {
                                SelectedLineKey = LN;
                                break;
                            }
                        }
                        if (SelectedLineKey != null) { break; }
                    }
                    if (SelectedLineKey != null) { break; }
                }
                if (SelectedLineKey != null)
                {
                    double LNTop = SelectedLineKey.MinTop;
                    double LNHeight = SelectedLineKey.MaxHeight;
                    Y = (int)Math.Round((LNTop + (LNTop + LNHeight)) / 2);
                    foreach (Word WD in SelectedLineKey.Words)
                    {
                        if (X < 0)
                        {
                            X = (int)Math.Round(WD.Left);
                        }
                        else
                        {
                            if (X > WD.Left)
                            {
                                X = (int)Math.Round(WD.Left);
                            }
                        }
                    }
                    if ((Y >= 0) && (X >= 0)) { resTF = true; }
                }
                return resTF;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}