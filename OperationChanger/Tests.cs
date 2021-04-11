using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace OperationChanger
{
    [TestFixture]
    public class Tests
    {
        private string assemblyPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName 
                                      + "\\ProgramExample\\bin\\Debug\\";
        [Test]
        public void TestEmptyPath()
        {
            Assert.Throws<ArgumentException>(() => Changer.ChangeAddToSubstr("", ""));
        }
        
        [Test]
        public void TestImplementationExisting()
        {
            Assert.IsTrue(File.Exists(assemblyPath + "ProgramExample.exe"), $"Nothing found in path: {assemblyPath}");
        }
        
        [Test]
        public void TestExecution()
        {
            string startAssembly = assemblyPath + "ProgramExample.exe";
            string targetAssembly = assemblyPath + "ProgramExampleResult.exe";
            
            
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                Changer.ChangeAddToSubstr(startAssembly, targetAssembly);
                
                var asm = Assembly.LoadFile(targetAssembly);
                Type type = asm.GetType("ProgramExample.Program");
                var main = type.GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                main?.Invoke(Activator.CreateInstance(type), new Object[]{new string[]{}});

                string expected = "0\n2\n26\n";
                string result = sw.ToString().Replace("\r\n", "\n");
                Assert.AreEqual(expected, result);
            }
            
        }
        
    }
}