using MagicLibrary.Application.Interfaces;
using MagicLibrary.Domain.Interfaces;
using MagicLibrary.Domain.Models;


namespace MagicLibrary.Application.Services
{
    public class UserProfileService: IUserProfileService
    {
        private readonly IUserProfileRepository _userProfileRepository;
        public UserProfileService(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }
        public List<UserProfile> ObtenerTodos()
        {
            return _userProfileRepository.ObtenerTodos();
        }

        public UserProfile? ObtenerPorId(int id)
        {
            return _userProfileRepository.ObtenerPorId(id);
        }

        public void Agregar(UserProfile perfil)
        {
            _userProfileRepository.Agregar(perfil);
        }

    }
}
