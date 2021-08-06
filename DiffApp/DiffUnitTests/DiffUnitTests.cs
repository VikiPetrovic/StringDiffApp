using Microsoft.VisualStudio.TestTools.UnitTesting;
using DiffApp;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using System.Net;

namespace DiffUnitTests
{
    [TestClass]
    public class DiffUnitTests
    {
        [TestMethod]
        public void Compare_WithEqualValues_ReturnsEqual()
        {
            DiffEntry e = new DiffEntry { Id = 1, Left = "AAAAAA==", Right = "AAAAAA==" };
            ResultModel actual = e.Compare();
            ResultModel expected = new ResultModel { diffResultType = ResultDescription.Equals, diffs = null };
            Assert.AreEqual(JsonConvert.SerializeObject(actual),
                JsonConvert.SerializeObject(expected));
        }

        [TestMethod]
        public void Compare_NullRightValue_ReturnsInvalid()
        {
            DiffEntry e = new DiffEntry { Id = 1, Left = "AAAAAA==", Right = null };
            ResultModel actual = e.Compare();
            ResultModel expected = new ResultModel { diffResultType = ResultDescription.InvalidInputValue, diffs = null };
            Assert.AreEqual(JsonConvert.SerializeObject(actual),
                JsonConvert.SerializeObject(expected));
        }

        [TestMethod]
        public void Compare_NullLeftValue_ReturnsInvalid()
        {
            DiffEntry e = new DiffEntry { Id = 1, Right = "AAAAAA==", Left = null };
            ResultModel actual = e.Compare();
            ResultModel expected = new ResultModel { diffResultType = ResultDescription.InvalidInputValue, diffs = null };
            Assert.AreEqual(JsonConvert.SerializeObject(actual),
                JsonConvert.SerializeObject(expected));
        }

        [TestMethod]
        public void Compare_DifferentLengths_ReturnsSizeDoNotMatch()
        {
            DiffEntry e = new DiffEntry { Id = 1, Right = "AAAAAA==", Left = "ABCD" };
            ResultModel actual = e.Compare();
            ResultModel expected = new ResultModel { diffResultType = ResultDescription.SizeDoNotMatch, diffs = null };
            Assert.AreEqual(JsonConvert.SerializeObject(actual),
                JsonConvert.SerializeObject(expected));
        }

        [TestMethod]
        public void Compare_WithDifferentValues_ReturnsContentDoNotMatch()
        {
            DiffEntry e = new DiffEntry { Id = 1, Right = "AAAAAA==", Left = "AQABAQ==" };
            ResultModel actual = e.Compare();

            List<ResultDifference> expected_diffs = new List<ResultDifference>();
            expected_diffs.Add(new ResultDifference() { length = 1, offset = 0 });
            expected_diffs.Add(new ResultDifference() { length = 2, offset = 2 });

            ResultModel expected = new ResultModel { diffResultType = ResultDescription.ContentDoNotMatch, diffs = expected_diffs };
            Assert.AreEqual(JsonConvert.SerializeObject(actual),
                JsonConvert.SerializeObject(expected));
        }

        [TestMethod]
        public void DoXor_WithNullArrays_ReturnsNull()
        {
            byte[] array1 = new byte[] { 0, 1, 0, 1 };
            byte[] array2 = null;

            int[] result = Helpers.DoXor(array1, array2);
            
            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void DoXor_WithDifferentLengthArrays_ReturnsNull()
        {
            byte[] array1 = new byte[] { 0, 1, 0, 1 };
            byte[] array2 = new byte[] { 0, 1, 0, 1, 1, 1 };

            int[] result = Helpers.DoXor(array1, array2);

            Assert.AreEqual(result, null);
        }

        [TestMethod]
        public void DoXor_WithOkArrays_ReturnsResult()
        {
            byte[] array1 = new byte[] { 0, 1, 0, 1 };
            byte[] array2 = new byte[] { 0, 0, 1, 1 };

            int[] result = Helpers.DoXor(array1, array2);

            CollectionAssert.AreEqual(result, new int[] { 0, 1, 1, 0 });
        }

        [TestMethod]
        public void FindDifferences_WithDifferences_ReturnsResult()
        {
            int[] input = new int[] { 1,0,1,1 };
            List<ResultDifference> result = Helpers.FindDifferences(xored_diff: input);

            List<ResultDifference> expected = new List<ResultDifference>();
            expected.Add(new ResultDifference() { length = 1, offset = 0 });
            expected.Add(new ResultDifference() { length = 2, offset = 2 });

            Assert.AreEqual(JsonConvert.SerializeObject(result),
                 JsonConvert.SerializeObject(expected));
        }

        [TestMethod]
        public void FindDifferences_WithoutDifferences_ReturnsResult()
        {
            int[] input = new int[] { 0,0,0,0,0 };
            List<ResultDifference> result = Helpers.FindDifferences(xored_diff: input);
            List<ResultDifference> expected = new List<ResultDifference>();

            Assert.AreEqual(JsonConvert.SerializeObject(result),
                 JsonConvert.SerializeObject(expected));
        }
    }
    
}
