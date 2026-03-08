using JobScoreServer.Services.Interfaces;
using System.Text.RegularExpressions;

namespace JobScoreServer.Services.Rules
{
    public class InclusivityRule : IJobEvaluationRule
    {
        // Thresholds for inclusivity checks
        private const int MaxMandatoryBullets = 7;
        private const int MinAdvantageBullets = 1;
        private const double MaxMandatoryToAdvantageRatio = 3.0;

        // Keywords for identifying mandatory requirements sections
        private static readonly string[] MandatoryKeywords =
        {
            "requirements", "required", "must have", "must-have", "qualifications",
            "mandatory", "essential", "necessary", "need to have"
        };

        // Keywords for identifying optional/advantage sections
        private static readonly string[] AdvantageKeywords =
        {
            "advantage", "advantages", "bonus", "plus", "nice to have", "nice-to-have",
            "preferred", "desirable", "optional", "would be a plus", "good to have"
        };

        // Common bullet point indicators
        private static readonly char[] BulletIndicators =
        {
            '•', '●', '○', '◦', '■', '□', '▪', '▫', '–', '—', '⋅', '*', '+'
        };

        public int RuleId => 3;

        public Task<bool> EvaluateAsync(string content, string? title = null)
        {
            // Title is not needed for inclusivity check, only uses content
            if (string.IsNullOrWhiteSpace(content))
            {
                return Task.FromResult(false);
            }

            var lines = content.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Find section boundaries
            var (mandatoryCount, advantageCount) = CountBulletsInSections(lines);

            // Check penalizing conditions
            bool isOverwhelming = mandatoryCount > MaxMandatoryBullets;
            bool isRigid = advantageCount == 0;
            bool isUnbalanced = false;

            // Check ratio only if there are advantage bullets
            if (advantageCount > 0)
            {
                double ratio = (double)mandatoryCount / advantageCount;
                isUnbalanced = ratio > MaxMandatoryToAdvantageRatio;
            }

            return Task.FromResult(!isOverwhelming && !isRigid && !isUnbalanced);
        }

        private (int mandatoryCount, int advantageCount) CountBulletsInSections(string[] lines)
        {
            int mandatoryCount = 0;
            int advantageCount = 0;
            string? currentSection = null;

            foreach (var line in lines)
            {
                var lineLower = line.ToLower().Trim();

                // Check if this line is a section header
                bool isMandatorySection = MandatoryKeywords.Any(keyword => lineLower.Contains(keyword));
                bool isAdvantageSection = AdvantageKeywords.Any(keyword => lineLower.Contains(keyword));

                if (isMandatorySection)
                {
                    currentSection = "mandatory";
                    continue;
                }
                else if (isAdvantageSection)
                {
                    currentSection = "advantage";
                    continue;
                }

                // Count bullets in the current section
                if (!string.IsNullOrWhiteSpace(currentSection))
                {
                    bool isBullet = false;

                    // Check for bullet point indicators
                    if (line.Length > 0 && BulletIndicators.Contains(line.TrimStart()[0]))
                    {
                        isBullet = true;
                    }

                    // Check for numbered lists (1., 2., etc.)
                    if (Regex.IsMatch(line.TrimStart(), @"^\d+[\.\)]\s+"))
                    {
                        isBullet = true;
                    }

                    // Check for markdown-style bullets (- or *)
                    if (Regex.IsMatch(line.TrimStart(), @"^[-*]\s+"))
                    {
                        isBullet = true;
                    }

                    if (isBullet)
                    {
                        if (currentSection == "mandatory")
                        {
                            mandatoryCount++;
                        }
                        else if (currentSection == "advantage")
                        {
                            advantageCount++;
                        }
                    }
                }
            }

            return (mandatoryCount, advantageCount);
        }
    }
}
