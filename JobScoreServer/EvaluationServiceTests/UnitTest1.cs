using JobScoreServer.Data;
using JobScoreServer.Models;
using JobScoreServer.Services;
using JobScoreServer.Services.Interfaces;
using JobScoreServer.Services.Rules;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EvaluationServiceTests
{
    public class UnitTest1
    {
        // Helper method to create a fresh database for each test
        private DBContext CreateDatabase()
        {
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            return new DBContext(options);
        }

        // Helper method to create a fake rule
        private Mock<IJobEvaluationRule> CreateMockRule(int ruleId, bool passes)
        {
            var mockRule = new Mock<IJobEvaluationRule>();
            mockRule.Setup(r => r.RuleId).Returns(ruleId);
            mockRule.Setup(r => r.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(passes);
            return mockRule;
        }

        #region Service Tests with Mock Rules

        [Fact]
        public async Task Test_Score_Calculation_With_Database_Weights()
        {
            
            using var db = CreateDatabase();
            db.Rules.Add(new Rule { Id = 1, Title = "Rule 1", Description = "Test", Weight = 15 });
            db.Rules.Add(new Rule { Id = 2, Title = "Rule 2", Description = "Test", Weight = 25 });
            await db.SaveChangesAsync();

            // rules that all fail
            var fakeRule1 = CreateMockRule(1, passes: false);
            var fakeRule2 = CreateMockRule(2, passes: false);
            var rules = new List<IJobEvaluationRule> { fakeRule1.Object, fakeRule2.Object };

            
            var service = new JobEvaluatorService(db, rules);
            var result = await service.EvaluateOnlyAsync("Title", "Content");

            // check: 100 - 15 - 25 = 60
            Assert.Equal(60, result.score);
            Assert.Equal(2, result.violations.Count);
        }

        [Fact]
        public async Task Test_Null_Content_Throws_Exception()
        {
            
            using var db = CreateDatabase();
            db.Rules.Add(new Rule { Id = 1, Title = "Test Rule", Description = "Test", Weight = 10 });
            await db.SaveChangesAsync();

            var fakeRule = CreateMockRule(1, passes: true);
            var service = new JobEvaluatorService(db, new[] { fakeRule.Object });

            // test with null - should throw ArgumentException
            await Assert.ThrowsAsync<ArgumentException>(() => service.EvaluateOnlyAsync(null, null));
        }

        [Fact]
        public async Task Test_Empty_String_Throws_Exception()
        {
            using var db = CreateDatabase();
            db.Rules.Add(new Rule { Id = 1, Title = "Test Rule", Description = "Test", Weight = 10 });
            await db.SaveChangesAsync();

            var fakeRule = CreateMockRule(1, passes: true);
            var service = new JobEvaluatorService(db, new[] { fakeRule.Object });

            // t with empty strings - should throw ArgumentException
            await Assert.ThrowsAsync<ArgumentException>(() => service.EvaluateOnlyAsync("", ""));
        }

        [Fact]
        public async Task Test_Multiple_Rules_Different_Weights()
        {
            using var db = CreateDatabase();
            db.Rules.Add(new Rule { Id = 1, Title = "Small Weight", Description = "Test", Weight = 5 });
            db.Rules.Add(new Rule { Id = 2, Title = "Medium Weight", Description = "Test", Weight = 20 });
            db.Rules.Add(new Rule { Id = 3, Title = "Large Weight", Description = "Test", Weight = 35 });
            await db.SaveChangesAsync();

            // first rule passes, second and third fail
            var rule1 = CreateMockRule(1, passes: true);
            var rule2 = CreateMockRule(2, passes: false);
            var rule3 = CreateMockRule(3, passes: false);
            var rules = new List<IJobEvaluationRule> { rule1.Object, rule2.Object, rule3.Object };

            var service = new JobEvaluatorService(db, rules);

            var result = await service.EvaluateOnlyAsync("Title", "Content");

            // 100 - 20 - 35 = 45 (only failed rules deduct)
            Assert.Equal(45, result.score);
            Assert.Equal(2, result.violations.Count); // Only 2 violations
        }

        #endregion

        #region REAL RULE TESTS - ContactEmailRule

        [Fact]
        public async Task ContactEmailRule_WithValidEmail_Passes()
        {
            var rule = new ContactEmailRule();
            var result = await rule.EvaluateAsync("Contact us at hr@company.com for details");
            Assert.True(result);
        }

        [Fact]
        public async Task ContactEmailRule_WithoutEmail_Fails()
        {
            var rule = new ContactEmailRule();
            var result = await rule.EvaluateAsync("Contact us for details");
            Assert.False(result);
        }

        #endregion

        #region REAL RULE TESTS - ReadabilityRule

        [Fact]
        public async Task ReadabilityRule_WithEnoughBullets_Passes()
        {
            var rule = new ReadabilityRule();
            var content = "Requirements:\n• Skill 1\n• Skill 2\n• Skill 3\n• Skill 4";
            var result = await rule.EvaluateAsync(content);
            Assert.True(result);
        }

        [Fact]
        public async Task ReadabilityRule_WithNotEnoughBullets_Fails()
        {
            var rule = new ReadabilityRule();
            var content = "Requirements:\n• Skill 1\n• Skill 2";
            var result = await rule.EvaluateAsync(content);
            Assert.False(result);
        }



        #endregion

        #region REAL RULE TESTS - EngagementRule

        [Fact]
        public async Task EngagementRule_WithShortLines_Passes()
        {
            var rule = new EngagementRule();
            var content = "Short line 1\nShort line 2\nShort line 3";
            var result = await rule.EvaluateAsync(content);
            Assert.True(result);
        }

        [Fact]
        public async Task EngagementRule_WithLongLine_Fails()
        {
            var rule = new EngagementRule();
            var content = new string('A', 100); // 100 chars, exceeds 75
            var result = await rule.EvaluateAsync(content);
            Assert.False(result);
        }

        #endregion

        #region REAL RULE TESTS - AllCapsRule

        [Fact]
        public async Task AllCapsRule_WithNoAllCaps_Passes()
        {
            var rule = new AllCapsRule();
            var content = "This is a normal job description with proper capitalization.";
            var result = await rule.EvaluateAsync(content);
            Assert.True(result);
        }

        [Fact]
        public async Task AllCapsRule_WithAllCapsWord_Fails()
        {
            var rule = new AllCapsRule();
            var content = "We need someone who is AMAZING and AWESOME";
            var result = await rule.EvaluateAsync(content);
            Assert.False(result);
        }

        [Fact]
        public async Task AllCapsRule_WithAllowedAcronyms_Passes()
        {
            var rule = new AllCapsRule();
            var content = "Experience with SQL, AWS, and REST APIs required";
            var result = await rule.EvaluateAsync(content);
            Assert.True(result);
        }

        #endregion

        #region INTEGRATION TEST - Real Rules with Service

        [Fact]
        public async Task Integration_RealRulesWithService_CalculatesCorrectScore()
        {
            // Setup database
            using var db = CreateDatabase();
            db.Rules.Add(new Rule { Id = 6, Title = "Contact Email", Description = "Test", Weight = 20 });
            db.Rules.Add(new Rule { Id = 9, Title = "All Caps", Description = "Test", Weight = 15 });
            await db.SaveChangesAsync();

            // Use REAL rules
            var realRules = new List<IJobEvaluationRule>
            {
                new ContactEmailRule(),  // RuleId = 6
                new AllCapsRule()        // RuleId = 9
            };

            var service = new JobEvaluatorService(db, realRules);

            // Content that FAILS contact email but PASSES all caps
            var result = await service.EvaluateOnlyAsync("Software Engineer", "We need a developer");

            // 100 - 20 (no email) = 80
            Assert.Equal(80, result.score);
            Assert.Single(result.violations); // Only 1 violation (ContactEmailRule)
        }

        [Fact]
        public async Task Integration_RealRules_AllPass()
        {
            // Setup database
            using var db = CreateDatabase();
            db.Rules.Add(new Rule { Id = 6, Title = "Contact Email", Description = "Test", Weight = 20 });
            db.Rules.Add(new Rule { Id = 9, Title = "All Caps", Description = "Test", Weight = 15 });
            await db.SaveChangesAsync();

            // Use REAL rules
            var realRules = new List<IJobEvaluationRule>
            {
                new ContactEmailRule(),
                new AllCapsRule()
            };

            var service = new JobEvaluatorService(db, realRules);

            // Content that passes ALL rules
            var result = await service.EvaluateOnlyAsync(
                "Software Engineer", 
                "We need a developer. Contact: hr@company.com"
            );

            // No deductions = 100
            Assert.Equal(100, result.score);
            Assert.Empty(result.violations);
        }

        #endregion

        #region COMPREHENSIVE INTEGRATION TESTS

        [Fact]
        public async Task Integration_AllRulesFail_ScoreZero()
        {
            // database with all rules 
            using var db = CreateDatabase();
            db.Rules.Add(new Rule { Id = 1, Title = "Readability", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 3, Title = "Inclusivity", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 4, Title = "Transparency", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 5, Title = "Engagement", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 6, Title = "Contact Email", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 7, Title = "Standardization", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 8, Title = "Tone of Voice", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 9, Title = "All Caps", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 10, Title = "Reading Time", Description = "Test", Weight = 10 });
            await db.SaveChangesAsync();

            
            var realRules = new List<IJobEvaluationRule>
            {
                new ReadabilityRule(),     
                new InclusivityRule(),      
                new TransparencyRule(),     
                new EngagementRule(),       
                new ContactEmailRule(),     
                new StandardizationRule(),  
                new ToneOfVoiceRule(),      
                new AllCapsRule(),          
                new ReadingTimeRule()      
            };

            var service = new JobEvaluatorService(db, realRules);

            // Content designed to FAIL all rules:
            // - No title seniority level (StandardizationRule fails)
            var title = "Developer Position";


            var content = @"WOW! This is an AMAZING opportunity! You'll be GREAT! FANTASTIC team!
Requirements: requirement one requirement two requirement three requirement four requirement five requirement six requirement seven requirement eight
This line is intentionally very long to exceed the seventy-five character limit for engagement rule testing purposes!";

            var result = await service.EvaluateOnlyAsync(title, content);

            // All rules fail: 100 - 90 = 10 (note: ProfessionalismRule not included, so only 9 rules)
            Assert.Equal(10, result.score); // 100 - 90 = 10 (9 rules ª 10 weight each)
            Assert.Equal(9, result.violations.Count); // All 9 rules violated
        }

        [Fact]
        public async Task Integration_AllRulesPass_PerfectScore()
        {
            // Setup database with ALL rules
            using var db = CreateDatabase();
            db.Rules.Add(new Rule { Id = 1, Title = "Readability", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 3, Title = "Inclusivity", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 4, Title = "Transparency", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 5, Title = "Engagement", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 6, Title = "Contact Email", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 7, Title = "Standardization", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 8, Title = "Tone of Voice", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 9, Title = "All Caps", Description = "Test", Weight = 10 });
            db.Rules.Add(new Rule { Id = 10, Title = "Reading Time", Description = "Test", Weight = 10 });
            await db.SaveChangesAsync();

            // Content designed to PASS all rules:
            // - Has seniority level in title (StandardizationRule passes)
            var title = "Senior Software Engineer";


            var content = @"Join our dynamic team working with modern technologies!

We are seeking a talented engineer to join our growing company.
This is an excellent opportunity to work on exciting projects.

Requirements:
• 5 years experience with C# and dotnet framework
• Strong knowledge of SQL databases and REST APIs
• Experience with AWS cloud services and deployment

Nice to have:
• Python programming experience
• Machine learning and AI knowledge

Salary: $80,000-$120,000

We offer flexible work arrangements and growth opportunities.
Our collaborative environment encourages innovation and creativity.
You will work with cutting edge technology on meaningful projects.
Contact us at hr@company.com to apply for this position.

We value diversity and welcome all qualified candidates to apply.
Benefits include health insurance and professional development.
Work on challenging projects with latest technology and tools.
Our team is supportive and values work life balance greatly.
Join us to make a real impact in the technology industry.";

            // Use ALL REAL rules
            var realRules = new List<IJobEvaluationRule>
            {
                new ReadabilityRule(),      
                new InclusivityRule(),      
                new TransparencyRule(),    
                new EngagementRule(),      
                new ContactEmailRule(),     
                new StandardizationRule(),  
                new ToneOfVoiceRule(),     
                new AllCapsRule(),         
                new ReadingTimeRule()      
            };

            var service = new JobEvaluatorService(db, realRules);
            var result = await service.EvaluateOnlyAsync(title, content);

            // All rules pass: 100 - 0 = 100
            Assert.Equal(100, result.score);
            Assert.Empty(result.violations); // No violations
        }

        #endregion
    }
}