﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SafetyZoneAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafetyZoneAPI.Tests
{
    [TestClass]
    public class ServiceTests
    {
        [TestMethod] 
        public void TestThatDetermineCrimeRatingIsNotFalse() {
            CrimeDataService dataService = new CrimeDataService();
            // Test using Ashfield data
            var result = dataService.DetermineCrimeRatingIndex(265.7);
            Assert.AreEqual(2, result);
        }
        [TestMethod]
        public void TestDetermineCrimeRatingForPenrith2015()
        {
            CrimeDataService dataService = new CrimeDataService();
            // Test using Ashfield data
            var result = dataService.DetermineCrimeRatingIndex(527.4);
            Assert.AreEqual(3, result);
        }
    }
}
