namespace ChatModel.Tests
{
    [TestClass]
    public sealed class UsernameTests
    {
        /// <summary>
        /// input = Admin
        /// </summary>
        [TestMethod]
        public void Admin_error()
        {
            // arrange
            string input = "Admin";
            var validator = new UserNameAttribute();

            // act
            bool result = validator.IsValid(input);

            // assert
            Assert.AreEqual(false, result);
        }

        /// <summary>
        /// input = AdMiN
        /// </summary>
        [TestMethod]
        public void AdMiN_error()
        {
            // arrange
            string input = "AdMiN";
            var validator = new UserNameAttribute();

            // act
            bool result = validator.IsValid(input);

            // assert
            Assert.AreEqual(false, result);
        }

        /// <summary>
        /// input = A
        /// </summary>
        [TestMethod]
        public void lettersCount1_error()
        {
            // arrange
            string input = "A";
            var validator = new UserNameAttribute();

            // act
            bool result = validator.IsValid(input);

            // assert
            Assert.AreEqual(false, result);
        }

        /// <summary>
        /// input = abcdefghijklmnopqrstuvwxyzasdfghj
        /// </summary>
        [TestMethod]
        public void lettersCount33_error()
        {
            // arrange
            string input = "abcdefghijklmnopqrstuvwxyzasdfghj";
            var validator = new UserNameAttribute();

            // act
            bool result = validator.IsValid(input);

            // assert
            Assert.AreEqual(false, result);
        }

        /// <summary>
        /// input = Username
        /// </summary>
        [TestMethod]
        public void GoodName_success()
        {
            // arrange
            string input = "Username";
            var validator = new UserNameAttribute();

            // act
            bool result = validator.IsValid(input);

            // assert
            Assert.AreEqual(true, result);
        }
    }
}
