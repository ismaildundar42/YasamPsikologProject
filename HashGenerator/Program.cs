string password = "Admin123";
string hash = BCrypt.Net.BCrypt.HashPassword(password);
Console.WriteLine($"Password: {password}");
Console.WriteLine($"Hash: {hash}");
Console.WriteLine($"Verify: {BCrypt.Net.BCrypt.Verify(password, hash)}");
