namespace Surreal;

public class OptionalTests
{
  [Test]
  public void it_should_report_none_when_using_static_constructor()
  {
    var value = Optional.None<string>();

    value.HasValue.Should().BeFalse();
  }

  [Test]
  public void it_should_report_some_when_using_static_constructor()
  {
    var value = Optional.Some("Test");

    value.HasValue.Should().BeTrue();
  }

  [Test]
  public void it_should_record_default_value_as_no_value()
  {
    Optional<string> value = default;

    value.HasValue.Should().BeFalse();
  }

  [Test]
  public void it_should_record_implicit_value_as_has_value()
  {
    Optional<string> value = "Hello, World!";

    value.HasValue.Should().BeTrue();
  }

  [Test]
  public void it_should_produce_default_value_when_has_value_is_false()
  {
    Optional<string> value = default;

    var result = value.GetOrDefault("Hello, World!");

    result.Should().Be("Hello, World!");
  }

  [Test]
  public void it_should_produce_given_value_when_has_value_is_true()
  {
    Optional<string> value = "Hello, World!";

    var result = value.GetOrDefault("Goodbye, World!");

    result.Should().Be("Hello, World!");
  }
}
