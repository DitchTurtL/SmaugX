using SmaugX.Core.Helpers;

namespace SmaugX.Core.Tests.Helpers;

internal class PasswordHasherTests
{
    [Test]
    public void TestHashPassword()
    {
        var password = "Test";
        var hashedPassword = PasswordHasher.HashPassword(password);
        Assert.That(hashedPassword, Is.Not.EqualTo(password));
    }

}
