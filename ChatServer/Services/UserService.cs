using ChatCommon.DTO;
using MasterDB;
using MasterDB.Entity;

namespace ChatServer.Services
{
    public class UserService(MasterDBContext masterDBContext)
    {
        internal async Task CreateUser(UserDTO userDTO)
        {
            var User = new User()
            {
                UserId = Guid.NewGuid(),
                UserEmail = userDTO.UserEmail,
                UserName = userDTO.UserName
            };
            masterDBContext.User.Add(User);
            await masterDBContext.SaveChangesAsync();
        }

        internal User? GetUser(Guid userId)
        {
            var user = masterDBContext.User
                .Where(x => x.UserId == userId)
                .FirstOrDefault();
            return user;
        }
    }
}
