using Xunit;
using static grpc_client.Client;

namespace client_grpc.Tests
{
    public class ValidationInsertTests
    {
        [Fact]
        public void validationInsertCorrect1()
        {
            Validation validation = new Validation("Дмитрий", "Гаврилов", 23);
            Assert.True(validation.RunValidationInsert());
        }

        [Fact]
        public void validationInsertCorrect2()
        {
            Validation validation = new Validation("Dmitry", "Gavrilov", 23);
            Assert.True(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertCorrect3()
        {
            Validation validation = new Validation("Vincent", "Гаврилов", 23);
            Assert.True(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertCorrect4()
        {
            Validation validation = new Validation("Ivan", "Petro-gradov", 23);
            Assert.True(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertIncorrect1()
        {
            Validation validation = new Validation("Дми3рий", "Гаврилов", 23);
            Assert.False(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertIncorrect2()
        {
            Validation validation = new Validation("Дмитрий", "Га3рилов", 23);
            Assert.False(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertIncorrect3()
        {
            Validation validation = new Validation("Dm1try", "Gavrilov", 23);
            Assert.False(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertIncorrect4()
        {
            Validation validation = new Validation("Dmitry", "Gavr1lov", 23);
            Assert.False(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertIncorrect5()
        {
            Validation validation = new Validation("Дмитрий", "Гаврилов", 99999);
            Assert.False(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertCorrect5()
        {
            Validation validation = new Validation("Дмитрий", "Гаврилов", 0);
            Assert.True(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertIncorrect7()
        {
            Validation validation = new Validation("Дмитрий", "Гаврилов", 151);
            Assert.False(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertIncorrect8()
        {
            Validation validation = new Validation("Дмитрий", "Гаврилов", -10);
            Assert.False(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertIncorrect9()
        {
            Validation validation = new Validation("", "Гаврилов", 23);
            Assert.False(validation.RunValidationInsert());
        }
        [Fact]
        public void validationInsertIncorrect10()
        {
            Validation validation = new Validation("Dmitry", " ", 23);
            Assert.False(validation.RunValidationInsert());
        }
    }
    public class ValidationReadTest
    {
        [Fact]
        public void validationReadCorrect1()
        {
            Validation validation = new Validation(10);
            Assert.True(validation.RunValidationRead());
        }
        [Fact]
        public void validationReadCorrect2()
        {
            Validation validation = new Validation(1234567890);
            Assert.True(validation.RunValidationRead());
        }
        [Fact]
        public void validationReadIncorrect1()
        {
            Validation validation = new Validation(-10);
            Assert.False(validation.RunValidationRead());
        }
        [Fact]
        public void validationReadIncorrect2()
        {
            Validation validation = new Validation(-10);
            Assert.False(validation.RunValidationRead());
        }
        [Fact]
        public void validationReadIncorrect3()
        {
            Validation validation = new Validation(0);
            Assert.False(validation.RunValidationRead());
        }
    }
}
