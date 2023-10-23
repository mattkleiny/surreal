﻿using Surreal.IO;
using Surreal.Maths;

namespace Surreal.Common.Tests.Maths;

public class SeedTests
{
  [Test]
  public void it_should_serialize_to_binary()
  {
    var memory = Seed.Randomized.ToByteArray();

    memory.Length.Should().Be(4);
  }
}
