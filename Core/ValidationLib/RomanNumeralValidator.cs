using FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ValidationLib
{
    /// <remarks>
    /// FluentValidation will throw exception for null input value.
    /// </remarks>
    public class RomanNumeralValidator : AbstractValidator<string>, IValidator<string>
    {
        private static char[] RepeatingSymbols = { 'I', 'X', 'C', 'M' };

        private static char[] NonRepeatingSymbols = { 'D', 'L', 'V' };

        private static IDictionary<char, char[]> FollowingSymbolSet = new Dictionary<char, char[]>()
        {
            { 'I', new char[] { 'I', 'V', 'X' } },
            { 'V', new char[] { 'V', 'I' } },
            { 'X', new char[] { 'X', 'L', 'C', 'I', 'V' } },
            { 'L', new char[] { 'L', 'X', 'V', 'I' } },
            { 'D', new char[] { 'D', 'C', 'L', 'X', 'V', 'I'} }
        };

        private static IDictionary<char, char[]> SubtractSymbolSet = new Dictionary<char, char[]>()
        {
            { 'I', new char[] { 'V', 'X' } },
            { 'X', new char[] { 'L', 'C' } },
            { 'C', new char[] { 'D', 'M' } }
        };

        private static IDictionary<char, char[]> NonSubtractSymbolSet = new Dictionary<char, char[]>()
        {
            { 'V', new char[] { 'X', 'L', 'C', 'D', 'M' } },
            { 'L', new char[] { 'C', 'D', 'M' } },
            { 'D', new char[] { 'M' } }
        };

        private readonly IEnumerable<char> romanNumeralSymbols;

        public RomanNumeralValidator(IEnumerable<char> romanNumeralSymbols)
        {
            if (romanNumeralSymbols == null || !romanNumeralSymbols.Any())
            {
                throw new ArgumentNullException("romanNumeralSymbols");
            }

            this.romanNumeralSymbols = romanNumeralSymbols;

            this.RuleFor(numeral => numeral)
                .NotEmpty()
                .WithMessage("Roman numeral cannot be empty.");

            this.RuleFor(numeral => numeral)
                .Must(AreSymbolsUpperCase)
                .WithMessage("Roman numerals should be uppercase.");

            this.RuleFor(numeral => numeral)
                .Must(AreSymbolsValid)
                .WithMessage("Invalid symbol found in Roman numeral.");

            this.RuleFor(numeral => numeral)
                .Must(AreRepeatingSymbolsValid)
                .WithMessage(string.Format("Roman numeral cannot have any of the symbols in the set: ({0}) repeated more than three times in succession.",
                                           string.Join(", ", RepeatingSymbols)));

            this.RuleFor(numeral => numeral)
                .Must(AreNonRepeatingSymbolsValid)
                .WithMessage(string.Format("Roman numeral cannot have any of the symbols in the set: ({0}) repeated.",
                                            string.Join(", ", NonRepeatingSymbols)));

            foreach (char key in FollowingSymbolSet.Keys)
            {
                this.RuleFor(numeral => numeral)
                    .Must(n => this.DoesSymbolHaveValidSymbolsFollowingIt(n, key, FollowingSymbolSet[key]))
                    .WithMessage(string.Format("Roman numeral can only have symbol {0} following {1}.",
                                               string.Join(", ", FollowingSymbolSet[key]), key));
            }

            foreach (char key in SubtractSymbolSet.Keys)
            {
                this.RuleFor(numeral => numeral)
                    .Must(n => this.IsSmallValueSymbolSubtractedFromLargeValueOnlyOnce(n, key, SubtractSymbolSet[key]))
                    .WithMessage(string.Format("Roman numeral can have only one small-value symbol {0} subtracted from set ({1}).",
                                               key, string.Join(", ", SubtractSymbolSet[key])));
            }

            this.RuleFor(numeral => numeral)
                .Must(AreSymbolsThatCannotBeSubtractedValid)
                .WithMessage(string.Format("Roman numeral cannot have any of the symbols in the set:({0}) subtracted.",
                                            string.Join(", ", NonSubtractSymbolSet.Keys)));
        }

        private bool AreSymbolsUpperCase(string numeral)
        {
            foreach (char n in numeral)
            {
                if (!char.IsUpper(n))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AreSymbolsValid(string numeral)
        {
            if (numeral.Except(this.romanNumeralSymbols).Count() > 0)
            {
                return false;
            }

            return true;
        }

        private bool AreRepeatingSymbolsValid(string numeral)
        {
            if (numeral.IndexOfAny(RepeatingSymbols) != -1)
            {
                foreach (char c in RepeatingSymbols)
                {
                    int consecutiveCounter = 0;

                    foreach (char n in numeral)
                    {
                        if (n == c)
                        {
                            if (++consecutiveCounter > 3)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            consecutiveCounter = 0;
                        }
                    }
                }
            }

            return true;
        }

        private bool AreNonRepeatingSymbolsValid(string numeral)
        {
            if (numeral.IndexOfAny(NonRepeatingSymbols) != -1)
            {
                foreach (char c in NonRepeatingSymbols)
                {
                    if (numeral.IndexOf(c) != numeral.LastIndexOf(c))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool DoesSymbolHaveValidSymbolsFollowingIt(string numeral, char symbolToCheck, char[] followingSymbolSet)
        {
            int indexPosition = numeral.IndexOf(symbolToCheck);

            if (indexPosition != -1)
            {
                for (int i = indexPosition; i < numeral.Length - 1; i++)
                {
                    if (numeral[i] == symbolToCheck &&
                        !followingSymbolSet.Contains(numeral[i + 1]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsSmallValueSymbolSubtractedFromLargeValueOnlyOnce(string numeral, char symbolToCheck, char[] subtractSymbolSet)
        {
            int indexPosition = numeral.IndexOf(symbolToCheck);

            if (indexPosition != -1)
            {
                int counter = 0;

                for (int i = indexPosition; i < numeral.Length - 1; i++)
                {
                    if (numeral[i] == numeral[i + 1])
                    {
                        continue;
                    }

                    if (numeral[i] == symbolToCheck)
                    {
                        if (subtractSymbolSet.Contains(numeral[i + 1]))
                        {
                            counter++;

                            if (i + 2 < numeral.Length)
                            {
                                // "IVI", "IXI", "XLX", "XCX", "CDC", "CMC"
                                if (numeral[i] == numeral[i + 2])
                                {
                                    return false;
                                }

                                // "IVV", "IXX", "XLL", "XCC", "CDD", "CMM"
                                if (numeral[i + 1] == numeral[i + 2])
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    
                    if (counter > 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool AreSymbolsThatCannotBeSubtractedValid(string numeral)
        {
            if (numeral.Intersect(NonSubtractSymbolSet.Keys).Count() > 0)
            {
                for (int i = 0; i < numeral.Length - 1; i++)
                {
                    if (NonSubtractSymbolSet.Keys.Contains(numeral[i]) &&
                        NonSubtractSymbolSet[numeral[i]].Contains(numeral[i + 1]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}