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
        var map = CreateMap<UserRequest, User>();

        // 1) No sobrescribir con nulls en updates (usa la sobrecarga de 3 params por compatibilidad)
        map.ForAllMembers(opt =>
            opt.Condition((src, dest, srcMember) => srcMember != null));

        // 2) Normalizar email SOLO si viene con valor
        map.ForMember(d => d.Email, o =>
        {
            o.PreCondition(s => !string.IsNullOrWhiteSpace(s.Email));
            o.MapFrom(s => s.Email!.Trim().ToLowerInvariant());
        });

        // 3) No permitir mapear PasswordHash desde el request
        map.ForMember(d => d.PasswordHash, o => o.Ignore());

        // Entity -> Response
        CreateMap<User, UserResponse>();
    }
}
