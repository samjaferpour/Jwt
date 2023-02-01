using Jwt.Dtos;
using Jwt.Entities;

namespace Jwt.Common
{
    public interface IGenerateToken
    {
        TokenDto GetToken(User user);
    }
}
