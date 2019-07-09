using ATM.DataAccess;
using ATM.DataAccess.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATM.BusinessLogic.Services
{
    public class ActionResultService
    {
        public IRepository<UserActionResult> _userActionRepository;

        public ActionResultService(IRepository<UserActionResult> userActionRepository)
        {
            _userActionRepository = userActionRepository;
        }

        public async Task<List<UserActionResult>> GetAllActionResults()
        {
            return await _userActionRepository.GetAllAsync();
        }

        public async Task<UserActionResult> GetActionResultById(int id)
        {
            return await _userActionRepository.GetByIdAsync(id);
        }

        public async Task AddActionResult(UserActionResult userAction)
        {
            await _userActionRepository.AddAsync(userAction);
        }

        public async Task EditActionResult(UserActionResult userAction)
        {
            await _userActionRepository.EditAsync(userAction);
        }

        public async Task DeleteActionResultById(int id)
        {
            UserActionResult userAction = await GetActionResultById(id);
            if (userAction != null)
            {
                await _userActionRepository.DeleteAsync(userAction);
            }
        }

        public bool ActionResultExists(int id)
        {
            return _userActionRepository.Exists(id);
        }
    }
}
