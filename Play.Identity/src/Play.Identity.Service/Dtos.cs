using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Identity.Service.Dtos
{
    public record UserDto(Guid Id, string Username, string Email, decimal Gil, DateTimeOffset CreatedDate);
    public record UpdateUserDto([Required][EmailAddress] string Email, [Range(0, 1000000)] decimal Gil);
}