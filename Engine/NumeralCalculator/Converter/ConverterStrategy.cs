using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumeralCalculator.Converter
{
    /// <summary>
    /// Strategizes the Conversion between numbers and roman numerals
    /// </summary>
    /// <remarks>
    ///     Assumes that the input provided is a valid input
    /// </remarks>
    public class ConverterStrategy : IConverterStrategy
    {
        private const int LOWER_EXCLUSIVE_BOUND = 0;
        private const int UPPER_EXCLUSIVE_BOUND = 4000;

        private readonly IDictionary<char, int> romanToArabicMap;
        private readonly IDictionary<int, string> arabicToRomanMap;

        public ConverterStrategy(IDictionary<char, int> romanToArabicMap, IDictionary<int, string> arabicToRomanMap)
        {
            if (romanToArabicMap == null || !romanToArabicMap.Any())
            {
                throw new ArgumentNullException("romanToArabicMap");
            }

            if (arabicToRomanMap == null || !arabicToRomanMap.Any())
            {
                throw new ArgumentNullException("arabicToRomanMap");
            }

            this.romanToArabicMap = romanToArabicMap;
            this.arabicToRomanMap = arabicToRomanMap;
        }

        public int ConvertToArabicNumeral(string romanNumeral)
        {
            int value = 0;

            if (string.IsNullOrEmpty(romanNumeral))
            {
                return value;
            }

            int.TryParse(romanNumeral, out value);
            if (value > 0)
            {
                return value;
            }

            if (!string.IsNullOrEmpty(romanNumeral))
            {
                int currentValue = 0;
                int previousValue = 0;

                for (int i = romanNumeral.Length - 1; i >= 0; i--)
                {
                    currentValue = this.romanToArabicMap[romanNumeral[i]];

                    if (currentValue < previousValue)
                    {
                        value -= currentValue;
                    }
                    else
                    {
                        value += currentValue;
                    }

                    previousValue = currentValue;
                }
            }

            return value;
        }

        /// <summary>
        /// Assumed range is from 1 - 3999, as per the scope that M's cannot be repeated more than three times.
        /// </summary>
        public string ConvertToRomanNumeral(int arabicNumeral)
        {
            StringBuilder romanNumeral = new StringBuilder();

            if (arabicNumeral > LOWER_EXCLUSIVE_BOUND && arabicNumeral < UPPER_EXCLUSIVE_BOUND)
            {
                int number = arabicNumeral;

                while (number > 0)
                {
                    foreach (int key in this.arabicToRomanMap.Keys.OrderByDescending(k => k))
                    {
                        while (number >= key)
                        {
                            romanNumeral.Append(this.arabicToRomanMap[key]);
                            number -= key;
                        }
                    }
                }
            }

            return romanNumeral.ToString();
        }
    }
}