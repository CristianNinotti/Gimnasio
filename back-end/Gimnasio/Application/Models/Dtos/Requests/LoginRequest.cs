﻿namespace Application.Models.Request;

public class LoginRequest
{
    public string NameAccount { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
