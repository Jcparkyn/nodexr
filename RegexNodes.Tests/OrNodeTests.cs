using System;
using NUnit.Framework;
using RegexNodes;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using RegexNodes.Shared;
using RegexNodes.Shared.NodeTypes;

namespace RegexNodes.Tests
{
    [TestFixture]
    class OrNodeTests
    {
        OrNode _node;
        RandomString randomString;

        [SetUp]
        public void SetUp()
        {
            _node = new OrNode();
            randomString = new RandomString();
        }

        [TestCase("Dog", "Cat", 50)]
        [TestCase(@"@#\$Dog6", " Cat", 50)]
        public void VariousStrings(string in1, string in2, int count = 50)
        {
            //_node.Input1 = new InputProcedural();
            //_node.Input1.InputNode = new ExactString(in1);
            //_node.Input2 = new InputProcedural();
            //_node.Input2.InputNode = new ExactString(in2);
            //Console.WriteLine(_node.GetValue());
            string nodeVal = _node.GetOutput();

            Assert.Multiple(()=>
            {
                for (int i = 0; i < count; i++)
                {
                    string str1 = randomString.Next(8);
                    string str2 = randomString.Next(8);
                    Assert.That(str1 + in1 + str2, Does.Match(nodeVal), nodeVal);
                    Assert.That(str1 + in2 + str2, Does.Match(nodeVal), nodeVal); 
                }
            });
        }
    }
}
