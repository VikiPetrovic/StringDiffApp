using System;
using System.Collections.Generic;

namespace DiffApp
{
    public class DiffEntry
    {

        public string Left { get; set; }
        public string Right { get; set; }
        public int Id { get; set; }

        public ResultModel Compare()
        {            
            // compare left and right base64 string 
            if(this.Left == null || this.Right == null)
            {
                return new ResultModel { diffResultType = ResultDescription.InvalidInputValue, diffs = null };                    
            }

            if(this.Left.Length != this.Right.Length)
            {
                return new ResultModel { diffResultType = ResultDescription.SizeDoNotMatch, diffs = null };
            }

            if(this.Left == this.Right)
            {
                return new ResultModel { diffResultType = ResultDescription.Equals, diffs = null };
            }
            else
            {
                //same length but not equal - detect differences
                return this.DetectDifferences();
            }

        }

        private ResultModel DetectDifferences()
        {            
            //decode left and right
            byte[] arrayLeft = Convert.FromBase64String(this.Left);
            byte[] arrayRight = Convert.FromBase64String(this.Right);

            // xor arrays to get different bytes
            int[] xored_diff = Helpers.DoXor(array1: arrayLeft, array2: arrayRight);

            //find differences - length, offset
            List<ResultDifference> myDiffs = Helpers.FindDifferences(xored_diff: xored_diff);            

            return new ResultModel { diffResultType = ResultDescription.ContentDoNotMatch, diffs = myDiffs };
        }

        public override bool Equals(object obj)
        {
            return this.Id.Equals(((DiffEntry)obj).Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }

    public class DiffModel
    {
        // input data for PUT - input is base64 string
        public string data { get; set; }
    }

    public class ResultModel
    {
        // result of comparison if status = OK
        public ResultDescription diffResultType { get; set; }
        public List<ResultDifference> diffs { get; set; }
    }

    public class ResultDifference
    {
        public int offset { get; set; }
        public int length { get; set; }
    }

    public enum ResultDescription
    {
        Equals,
        SizeDoNotMatch,
        ContentDoNotMatch,
        InvalidInputValue
    }
}
