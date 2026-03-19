using System;
using ChatSupportService;
using ChatSupportService.Models;
using Moq;
using Xunit;

namespace ChatSupportService.Tests.Unit
{
    public class AgentTests
    {
        [Theory]
        [InlineData(Seniority.Junior, 4)]
        [InlineData(Seniority.MidLevel, 6)]
        [InlineData(Seniority.Senior, 8)]
        [InlineData(Seniority.TeamLead, 5)]
        public void CanMaxConcurrencySeniorityValueTest(Seniority seniority, int expected)
        {
            // Arrange
            var agent = new Agent
            {
                Seniority = seniority
            };

            // Act
            int actual = agent.MaxConcurrency;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(int.MinValue)]
        [InlineData(-1)]
        [InlineData(999)]
        [InlineData(int.MaxValue)]
        public void CanMaxConcurrencyUnknownSeniorityTest(int rawSeniorityValue)
        {
            // Arrange
            var agent = new Agent
            {
                Seniority = (Seniority)rawSeniorityValue
            };

            // Act
            int actual = agent.MaxConcurrency;

            // Assert
            Assert.Equal(4, actual);
        }

        [Theory]
        [InlineData(0, 0.4)]    // Junior
        [InlineData(1, 0.6)]    // MidLevel
        [InlineData(2, 0.8)]    // Senior
        [InlineData(3, 0.5)]    // TeamLead
        [InlineData(-1, 0.4)]   // out-of-range negative -> default
        [InlineData(999, 0.4)]  // out-of-range large -> default
        public void CanGetSeniorityMultiplierSeniorityValueTest(int seniorityValue, double expectedMultiplier)
        {
            // Arrange
            var agent = new Agent
            {
                Seniority = (Seniority)seniorityValue
            };

            // Act
            double actual = agent.GetSeniorityMultiplier();

            // Assert
            // Use a precision to avoid floating point representation issues.
            Assert.Equal(expectedMultiplier, actual, precision: 5);
        }
    }
}