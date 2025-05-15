namespace ClientTests
{
    [TestClass]
    public sealed class UsernameTests
    {
        /// <summary>
        /// input =
        100-50-25----1.5-30-3D
        /// </summary>
        [TestMethod]
        public void Regex_small_success()
        {
            // arrange
            string input = "100-50-25----1.5-30-3D";
            var creator = new NcpCreator() { FolderName = input };

            creator.Ncp.NcFiles.Add(new NcFile() { Tool = new MillingTool() });


            // act
            creator.TryGetData();


            // assert
            Assert.AreEqual("100", creator.Ncp.XSize);
            Assert.AreEqual("50",
        creator.Ncp.YSize);
            Assert.AreEqual("25", creator.Ncp.ZSize);
            Assert.AreEqual("1.5 (30) 3D",
        creator.Ncp.NcFiles[0].Tool.Name);
        }
    }
}
