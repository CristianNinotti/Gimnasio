// Application/Mapping/UserProfile.cs
using AutoMapper;
using Application.Models.Request;
using Application.Models.Response;
using Domain.Entities;

namespace Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        // Request -> User (abstracto, EF hace TPH por convención)
        CreateMap<UserRequest, User>();

        // Entity -> Response
        CreateMap<User, UserResponse>();
    }
}
