using Xunit;

namespace csmacnz.CLIArgsParser.Tests
{
    public class CLIArgsParserTests
    {
        [Fact]
        public void EmptyString_EmptyArgsArray()
        {
            var input = "";
            var result = ArgsParser.Parse(input);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("   ")]
        [InlineData("     ")]
        [InlineData("\t")]
        [InlineData(" \t ")]
        public void Whitespace_EmptyArgsArray(string input)
        {
            var result = ArgsParser.Parse(input);
            Assert.Empty(result);
        }

        [Theory]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("b")]
        [InlineData("blue")]
        public void SingleValue_SingleLengthArgsArray(string input)
        {
            var result = ArgsParser.Parse(input);
            Assert.Collection(
                result,
                v =>
                {
                    Assert.Equal(input, v);
                });
        }

        [Theory]
        [InlineData("\"hello world\"", "hello world")]
        [InlineData("\r ", "\r")]
        [InlineData(" \r", "\r")]
        [InlineData(" \r ", "\r")]
        [InlineData("\n ", "\n")]
        [InlineData(" \n", "\n")]
        [InlineData(" \n ", "\n")]
        public void SingleCharWithWhitespace_SingleLengthArgsArray(string input, string expected)
        {
            var result = ArgsParser.Parse(input);
            Assert.Collection(
                result,
                v =>
                {
                    Assert.Equal(expected, v);
                });
        }

        [Theory]
        [InlineData(" b")]
        [InlineData("b ")]
        [InlineData(" b ")]
        [InlineData("\tb")]
        [InlineData("b\t")]
        [InlineData("\tb\t")]
        [InlineData(" blue")]
        [InlineData("blue ")]
        [InlineData(" blue ")]
        [InlineData("\tblue")]
        [InlineData("blue\t")]
        [InlineData("\tblue\t")]
        public void SingleValueWithWhitespace_SingleLengthArgsArray(string input)
        {
            var trimmedValue = input.Trim();
            var result = ArgsParser.Parse(input);
            Assert.Collection(
                result,
                v =>
                {
                    Assert.Equal(trimmedValue, v);
                });
        }

        [Fact]
        public void ComplexArgs_()
        {
            var input = " -bdf Foo  -a \"bar baz\" -l \"a\"   fizz buzz \"hello world\"\tfibonacci";
            var result = ArgsParser.Parse(input);
            Assert.Collection(
                result,
                v =>
                {
                    Assert.Equal("-bdf", v);
                },v =>
                {
                    Assert.Equal("Foo", v);
                },v =>
                {
                    Assert.Equal("-a", v);
                },v =>
                {
                    Assert.Equal("bar baz", v);
                },v =>
                {
                    Assert.Equal("-l", v);
                },v =>
                {
                    Assert.Equal("a", v);
                },v =>
                {
                    Assert.Equal("fizz", v);
                },v =>
                {
                    Assert.Equal("buzz", v);
                },v =>
                {
                    Assert.Equal("hello world", v);
                },v =>
                {
                    Assert.Equal("fibonacci", v);
                });
        }
    }
}