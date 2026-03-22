using JobScoreServer.Services.Interfaces;
using System.Text.RegularExpressions;

namespace JobScoreServer.Services.Rules
{
    public class AllCapsRule : IJobEvaluationRule
    {
        // Common technical terms and acronyms that are legitimately written in all caps
        private static readonly HashSet<string> AllowedAllCapsTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Programming Languages
            "C", "R", "GO", ".NET", "ASP.NET", "C#", "C++",
            
            // Databases
            "SQL", "NOSQL", "MYSQL", "MSSQL", "POSTGRESQL",
            
            // Cloud Providers & Services
            "AWS", "GCP", "IBM", "SAS", "SAP",
            
            // Technologies & Frameworks
            "API", "REST", "JSON", "XML", "HTML", "CSS", "HTTP", "HTTPS", "FTP", "SSH",
            "CI", "CD", "ETL", "SDK", "IDE", "CLI", "GUI", "ORM", "MVC", "MVVM",
            
            // Certifications & Standards
            "ISO", "PMP", "ITIL", "TOGAF", "CISSP", "CEH",
            
            // General IT Terms
            "IT", "CRM", "ERP", "VPN", "LAN", "WAN", "DNS", "DHCP", "TCP", "IP", "UDP",
            "OS", "UI", "UX", "QA", "BA", "PM", "HR", "CEO", "CTO", "CIO", "CFO",
            
            // Methodologies
            "SCRUM", "AGILE", "LEAN", "SOA", "SLA", "KPI", "OKR", "ROI",
            
            // Other common acronyms
            "GDPR", "HIPAA", "PCI", "DSS", "AI", "ML", "NLP", "OCR", "IoT",
            "SOAP", "AJAX", "CORS", "JWT", "OAuth", "SAML", "LDAP", "AD",
            "DevOps", "MLOps", "DataOps", "SRE", "DBA", "PDF", "CSV", "TSV",
            "GitHub", "GitLab", "CI/CD", "TDD", "BDD", "DDD"
        };

        // Maximum allowed all-caps words
        private const int MaxAllCapsWords = 0;

        public int RuleId => 9;

        public Task<bool> EvaluateAsync(string content, string? title = null)
        {
            // Combine title and content to check both
            var combinedText = $"{title ?? string.Empty} {content}";

            if (string.IsNullOrWhiteSpace(combinedText))
            {
                return Task.FromResult(true);
            }

            // Split into words (removing punctuation)
            var words = Regex.Split(combinedText, @"\s+")
                .Select(w => Regex.Replace(w, @"[^\w]", "")) // Remove punctuation
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .ToList();

            var allCapsWordsCount = words.Count(word =>
            {
                // Must be at least 2 characters to avoid single letters
                if (word.Length < 2)
                    return false;

                // Check if word is all uppercase
                if (!word.All(char.IsUpper))
                    return false;

                // Check if it's in the allowed terms list
                if (AllowedAllCapsTerms.Contains(word))
                    return false;

                return true; // It's an all-caps word that's not allowed
            });

            return Task.FromResult(allCapsWordsCount <= MaxAllCapsWords);
        }
    }
}
